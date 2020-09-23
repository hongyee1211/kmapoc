using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CognitiveSearch.UI.Infrastructure;
using Microsoft.Graph;
using Constants = CognitiveSearch.UI.Infrastructure.Constants;
using System.Diagnostics;

namespace CognitiveSearch.UI.Services.GraphOperations
{
    public class GraphApiOperationService: IGraphApiOperations
    {
        private readonly HttpClient httpClient;
        private readonly WebOptions webOptions;
        private GraphServiceClient graphServiceClient;

        public GraphApiOperationService(HttpClient httpClient, IOptions<WebOptions> webOptionValue)
        {
            this.httpClient = httpClient;
            webOptions = webOptionValue.Value;
        }

        public async Task<dynamic> GetUserInformation(string accessToken)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(Constants.BearerAuthorizationScheme,
                                              accessToken);
            var response = await httpClient.GetAsync($"{webOptions.GraphApiUrl}/beta/me");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic me = JsonConvert.DeserializeObject(content);

                return me;
            }

            throw new
                HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<string> GetPhotoAsBase64Async(string accessToken)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(Constants.BearerAuthorizationScheme,
                                              accessToken);

            var response = await httpClient.GetAsync($"{webOptions.GraphApiUrl}/beta/me/photo/$value");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                byte[] photo = await response.Content.ReadAsByteArrayAsync();
                string photoBase64 = Convert.ToBase64String(photo);

                return photoBase64;
            }
            else
            {
                return null;
            }
        }

        public async Task<IDictionary<string, string>> EnumerateTenantsIdAndNameAccessibleByUser(IEnumerable<string> tenantIds, Func<string, Task<string>> getTokenForTenant)
        {
            Dictionary<string, string> tenantInfo = new Dictionary<string, string>();
            foreach (string tenantId in tenantIds)
            {
                string displayName;
                try
                {
                    string accessToken = await getTokenForTenant(tenantId);
                    httpClient.DefaultRequestHeaders.Remove("Authorization");
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var httpResult = await httpClient.GetAsync(GraphTenantInfoUrl);
                    var json = await httpResult.Content.ReadAsStringAsync();
                    OrganizationResult organizationResult = JsonConvert.DeserializeObject<OrganizationResult>(json);
                    displayName = organizationResult.value.First().displayName;
                }
                catch
                {
                    displayName = "you need to sign-in (or have the admin consent for the app) in that tenant";
                }

                tenantInfo.Add(tenantId, displayName);
            }
            return tenantInfo;
        }

        public async Task<List<Group>> GetMyMemberOfGroupsAsync(string accessToken)
        {
            List<Group> groups = new List<Group>();
            PrepareAuthenticatedClient(accessToken);
            // Get groups the current user is a direct member of.
            IUserMemberOfCollectionWithReferencesPage memberOfGroups = await graphServiceClient.Me.MemberOf.Request().GetAsync();
            if (memberOfGroups?.Count > 0)
            {
                foreach (var directoryObject in memberOfGroups)
                {
                    // We only want groups, so ignore DirectoryRole objects.
                    if (directoryObject is Group)
                    {
                        Group group = directoryObject as Group;
                        groups.Add(group);
                    }
                }
            }

            // If paginating
            while (memberOfGroups.NextPageRequest != null)
            {
                memberOfGroups = await memberOfGroups.NextPageRequest.GetAsync();

                if (memberOfGroups?.Count > 0)
                {
                    foreach (var directoryObject in memberOfGroups)
                    {
                        // We only want groups, so ignore DirectoryRole objects.
                        if (directoryObject is Group)
                        {
                            Group group = directoryObject as Group;
                            groups.Add(group);
                        }
                    }
                }
            }

            return groups;
        }

        private void PrepareAuthenticatedClient(string accessToken)
        {
            try
            {
                graphServiceClient = new GraphServiceClient(webOptions.GraphApiUrl,
                                                                     new DelegateAuthenticationProvider(
                                                                         async (requestMessage) =>
                                                                         {
                                                                             await Task.Run(() =>
                                                                             {
                                                                                 requestMessage.Headers.Authorization = new AuthenticationHeaderValue(Constants.BearerAuthorizationScheme, accessToken);
                                                                                 
                                                                             });
                                                                         }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not create a graph client {ex}");
            }
        }

        // Use the graph to get information (name) for a tenant 
        // See https://docs.microsoft.com/en-us/graph/api/organization-get?view=graph-rest-beta
        protected string GraphTenantInfoUrl { get; } = "https://graph.microsoft.com/beta/organization";
    }

    /// <summary>
    /// Result for a call to graph/organizations.
    /// </summary>
    class OrganizationResult
    {
        public Organization[] value { get; set; }
    }

    /// <summary>
    /// We are only interested in the organization display name
    /// </summary>
    class Organization
    {
        public string displayName { get; set; }
    }
}