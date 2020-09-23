﻿using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using CognitiveSearch.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

namespace CognitiveSearch.UI.Services.PowerBIEmbed
{
    public class EmbedService : IEmbedService
    {
        private IConfiguration _configuration { get; set; }
        private string AuthorityUrl { get; set; }
        private string ResourceUrl { get; set; } 
        private string ApplicationId { get; set; } 
        private string ApiUrl { get; set; } 
        private string WorkspaceId { get; set; } 
        private string ReportId { get; set; } 
        private string AuthenticationType { get; set; } 
        private string ApplicationSecret { get; set; } 
        private string Tenant { get; set; } 
        private string Username { get; set; } 
        private string Password { get; set; } 

        public EmbedConfig EmbedConfig
        {
            get { return m_embedConfig; }
        }

        private EmbedConfig m_embedConfig;
        private TileEmbedConfig m_tileEmbedConfig;
        private TokenCredentials m_tokenCredentials;

        public EmbedService(IConfiguration configuration)
        {
            m_tokenCredentials = null;
            m_embedConfig = new EmbedConfig();
            m_tileEmbedConfig = new TileEmbedConfig();

            try
            {
                _configuration = configuration;
                AuthorityUrl = configuration.GetSection("authorityUrl")?.Value;
                ResourceUrl = configuration.GetSection("resourceUrl")?.Value;
                ApplicationId = configuration.GetSection("applicationId")?.Value;
                ApiUrl = configuration.GetSection("apiUrl")?.Value;
                WorkspaceId = configuration.GetSection("workspaceId")?.Value;
                ReportId = configuration.GetSection("reportId")?.Value;
                AuthenticationType = configuration.GetSection("AuthenticationType").Value;
                ApplicationSecret = configuration.GetSection("applicationSecret").Value;
                Tenant = configuration.GetSection("tenant").Value;
                Username = configuration.GetSection("pbiUsername").Value;
                Password = configuration.GetSection("pbiPassword").Value;
            }
            catch (Exception e)
            {
                // If you get an exceptio here, most likely you have not set your
                // credentials correctly in appsettings.json
                throw new ArgumentException(e.Message.ToString());
            }
        }

        public async Task<bool> EmbedReport(string username, string roles)
        {
            //get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                return false;
            }

            try
            {
                //create power bi client object to call power bi APIs
                using (var client = new PowerBIClient(new Uri(ApiUrl), m_tokenCredentials))
                {
                    var reports = await client.Reports.GetReportsInGroupAsync(Guid.Parse(WorkspaceId));

                    if (reports.Value.Count() == 0)
                    {
                        m_embedConfig.ErrorMessage = "No reports found in the workspace";
                        return false;
                    }

                    Report report;
                    report = reports.Value.FirstOrDefault();

                    var datasets = await client.Datasets.GetDatasetInGroupAsync(Guid.Parse(WorkspaceId), report.DatasetId);
                    m_embedConfig.IsEffectiveIdentityRequired = datasets.IsEffectiveIdentityRequired;
                    m_embedConfig.IsEffectiveIdentityRolesRequired = datasets.IsEffectiveIdentityRolesRequired;
                    GenerateTokenRequest generateTokenRequestParameters;
                    // This is how you create embed token with effective identities
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        var rls = new EffectiveIdentity(username, new List<string> { report.DatasetId });
                        if (!string.IsNullOrWhiteSpace(roles))
                        {
                            var rolesList = new List<string>();
                            rolesList.AddRange(roles.Split(','));
                            rls.Roles = rolesList;
                        }
                        // Generate Embed Token with effective identities.
                        generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view", identities: new List<EffectiveIdentity> { rls });
                    }
                    else
                    {
                        // Generate Embed Token for reports without effective identities.
                        generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                    }

                    var tokenResponse = await client.Reports.GenerateTokenInGroupAsync(Guid.Parse(WorkspaceId), report.Id, generateTokenRequestParameters);

                    if (tokenResponse == null)
                    {
                        m_embedConfig.ErrorMessage = "Failed to generate embed token.";
                        return false;
                    }

                    // Generate Embed Configuration.
                    m_embedConfig.EmbedToken = tokenResponse;
                    m_embedConfig.EmbedUrl = report.EmbedUrl;
                    m_embedConfig.Id = report.Id.ToString();
                }
            }
            catch (HttpOperationException exc)
            {
                m_embedConfig.ErrorMessage = string.Format("Status: {0} ({1})\r\nResponse: {2}\r\nRequestId: {3}", exc.Response.StatusCode, (int)exc.Response.StatusCode, exc.Response.Content, exc.Response.Headers["RequestId"].FirstOrDefault());
                return false;
            }

            return true;
        }

        private string GetAppSettingsErrors()
        {
            // Application Id must have a value.
            if (string.IsNullOrWhiteSpace(ApplicationId))
            {
                return "ApplicationId is empty. please register your application as Native app in https://dev.powerbi.com/apps and fill client Id in web.config.";
            }

            // Application Id must be a Guid object.
            Guid result;
            if (!Guid.TryParse(ApplicationId, out result))
            {
                return "ApplicationId must be a Guid object. please register your application as Native app in https://dev.powerbi.com/apps and fill application Id in web.config.";
            }

            // Workspace Id must have a value.
            if (string.IsNullOrWhiteSpace(WorkspaceId))
            {
                return "WorkspaceId is empty. Please select a group you own and fill its Id in web.config";
            }

            // Workspace Id must be a Guid object.
            if (!Guid.TryParse(WorkspaceId, out result))
            {
                return "WorkspaceId must be a Guid object. Please select a workspace you own and fill its Id in web.config";
            }

            if (AuthenticationType.Equals("MasterUser"))
            {
                // Username must have a value.
                if (string.IsNullOrWhiteSpace(Username))
                {
                    return "Username is empty. Please fill Power BI username in web.config";
                }

                // Password must have a value.
                if (string.IsNullOrWhiteSpace(Password))
                {
                    return "Password is empty. Please fill password of Power BI username in web.config";
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ApplicationSecret))
                {
                    return "ApplicationSecret is empty. please register your application as Web app and fill appSecret in web.config.";
                }

                // Must fill tenant Id
                if (string.IsNullOrWhiteSpace(Tenant))
                {
                    return "Invalid Tenant. Please fill Tenant ID in Tenant under web.config";
                }
            }

            return null;
        }

        private async Task<AuthenticationResult> DoAuthentication()
        {
            AuthenticationResult authenticationResult = null;
            if (AuthenticationType.Equals("MasterUser"))
            {
                var authenticationContext = new AuthenticationContext(AuthorityUrl);

                // Authentication using master user credentials
                var credential = new UserCredential(Username);
                authenticationResult = authenticationContext.AcquireTokenAsync(ResourceUrl, ApplicationId, credential).Result;
            }
            else
            {
                // For app only authentication, we need the specific tenant id in the authority url
                var tenantSpecificURL = AuthorityUrl.Replace("common", Tenant);
                var authenticationContext = new AuthenticationContext(tenantSpecificURL);

                // Authentication using app credentials
                var credential = new ClientCredential(ApplicationId, ApplicationSecret);
                authenticationResult = await authenticationContext.AcquireTokenAsync(ResourceUrl, credential);
            }

            return authenticationResult;
        }

        private async Task<bool> GetTokenCredentials()
        {
            // var result = new EmbedConfig { Username = username, Roles = roles };
            var error = GetAppSettingsErrors();
            if (error != null)
            {
                m_embedConfig.ErrorMessage = error;
                return false;
            }

            // Authenticate using created credentials
            AuthenticationResult authenticationResult = null;
            try
            {
                authenticationResult = await DoAuthentication();
            }
            catch (AggregateException exc)
            {
                m_embedConfig.ErrorMessage = exc.InnerException.Message;
                return false;
            }

            if (authenticationResult == null)
            {
                m_embedConfig.ErrorMessage = "Authentication Failed.";
                return false;
            }

            m_tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");
            return true;
        }

    }
}