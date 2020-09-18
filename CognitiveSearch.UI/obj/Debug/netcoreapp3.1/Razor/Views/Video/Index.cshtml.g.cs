#pragma checksum "C:\Ashley\Code\kmapoc\CognitiveSearch.UI\Views\Video\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a4d7405a8e8d74b67ab9ee717cf843af182a5fa2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Video_Index), @"mvc.1.0.view", @"/Views/Video/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Ashley\Code\kmapoc\CognitiveSearch.UI\Views\_ViewImports.cshtml"
using CognitiveSearch.UI;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Ashley\Code\kmapoc\CognitiveSearch.UI\Views\_ViewImports.cshtml"
using CognitiveSearch.UI.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a4d7405a8e8d74b67ab9ee717cf843af182a5fa2", @"/Views/Video/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"cd1c8ddb33f15e20aee94317c529990ec82e9754", @"/Views/_ViewImports.cshtml")]
    public class Views_Video_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/bootstrap/dist/css/bootstrap.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<!-- Copyright (c) Microsoft Corporation. All rights reserved.\r\n     Licensed under the MIT License. -->\r\n\r\n");
#nullable restore
#line 4 "C:\Ashley\Code\kmapoc\CognitiveSearch.UI\Views\Video\Index.cshtml"
  
    ViewData["Title"] = "Video Indexer";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<script src=""https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js""></script>
<script src=""https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js""></script>
<script>

    function myFunction(accountId, id) {
        var pivotsHTML = """";

        if (accountId != """") {
            pivotsHTML += '<iframe width=""852"" height=""480"" src=""https://www.videoindexer.ai/embed/player/' + accountId + '/' + id + '/?&locale=en&location=Trial"" frameborder=""0"" allowfullscreen></iframe>';
            pivotsHTML += '<iframe width=""580"" height=""600"" src=""https://www.videoindexer.ai/embed/insights/' + accountId + '/' + id + '/?&locale=en&location=Trial"" frameborder=""0"" allowfullscreen></iframe>';
            pivotsHTML += '<script src=""https://breakdown.blob.core.windows.net/public/vb.widgets.mediator.js""/>';
        }

        $('#modalVideoContent').html(pivotsHTML);

    }

    $(function () {
        var _mySearchButton = document.getElementById(""mySearchButton"");
        _mySearch");
            WriteLiteral(@"Button.onclick = getData;

        function getData() {
            var _mySearchField = document.getElementById(""mySearchField"");
            var pivotsHTML = """";

            $.ajax({
                url: ""https://api.videoindexer.ai/Auth/trial/Accounts/ee451cff-6729-4f57-86d3-e6f779d295a5/AccessToken?allowEdit=False"",
                type: ""GET"",
                dataType: ""json"",
                headers: {
                    'Ocp-Apim-Subscription-Key': 'f0d1a0262df74dc994cb8efa87f28dba'
                },
                success: function (data) {
                    $(document).ready(function () {
                        $.ajax({
                            url: `https://api.videoindexer.ai/trial/Accounts/ee451cff-6729-4f57-86d3-e6f779d295a5/Videos/Search`,
                            headers: {
                                'Ocp-Apim-Subscription-Key': 'f0d1a0262df74dc994cb8efa87f28dba'
                            },
                            type: 'GET',
                      ");
            WriteLiteral(@"      dataType: 'json',
                            data: {
                                'query': _mySearchField.value,
                                'accessToken': data
                            },
                            success: function (data) {
                                if (data.results.length > 0) {
                                    for (var i = 0; i < data.results.length; i++) {
                                        pivotsHTML += '<div id=""VideoContent"" class=""video_content"">';
                                        pivotsHTML += '<div id=""viName"" class=""viName"">';
                                        pivotsHTML += '<button onclick=""myFunction(\'' + data.results[i].accountId + '\',\'' + data.results[i].id + '\')"" id=""idMyModal"" type=""button"" class=""btn btn-info btn-lg"" data-toggle=""modal"" data-target=""#myModal"" style=""background-color: transparent; border: none;""><h4>' + data.results[i].name + '</h4></button>';
                                        pivotsHTML += '<");
            WriteLiteral(@"/div>';

                                        if (data.results[i].searchMatches.length > 0) {
                                            pivotsHTML += '<div id=""viSearchMatchesContent"" class=""viSearchMatchesContent"">';
                                            for (var j = 0; j < data.results[i].searchMatches.length; j++) {
                                                pivotsHTML += '<div id=""viSearchMatches"" class=""viSearchMatches"">';
                                                var startTime = data.results[i].searchMatches[j].startTime.substring(0, 8);
                                                var videoText = data.results[i].searchMatches[j].text.replace(data.results[i].searchMatches[j].exactText, ""<b>"" + data.results[i].searchMatches[j].exactText + '</b>');
                                                pivotsHTML += '<div style=""background-color: #00a19c; padding:3px 5px;"">' + startTime + '</div>';
                                                pivotsHTML += '<div style=""width:");
            WriteLiteral(@" 5%;""></div>';
                                                pivotsHTML += '<div style=""width: 80%; padding:3px 0px;"">' + videoText + '</div>';
                                                pivotsHTML += '</div>';
                                            }
                                            pivotsHTML += '</div>';
                                        }

                                        pivotsHTML += '</div>';

                                        $('#VideoContentContainer').html(pivotsHTML);
                                    }
                                } else {
                                    pivotsHTML = """";
                                    $('#VideoContentContainer').html(pivotsHTML);
                                }
                            }
                        });
                    })
                }
            });
        }
    });
</script>

");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "a4d7405a8e8d74b67ab9ee717cf843af182a5fa29372", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"

<div id=""results-container"" class=""row content-results"">
    <div class=""col-md-2"">
        <div id=""search-input-group"" class=""input-group"">
            <input class=""form-control advancedAutoComplete"" id=""mySearchField"" name=""search"" placeholder=""Search.."" type=""text"">
            <span class=""input-group-btn"">
                <button id=""mySearchButton"" class=""btn btn-default""><span class=""ms-Icon ms-Icon--Search""></span></button>
            </span>
        </div>

    </div>
    <div class=""col-md-10"">
        <div id=""VideoContentContainer""></div>
        <!--Modal -->
        <div class=""modal fade"" id=""myModal"" role=""dialog"">
            <div class=""modal-dialog"">
                <div class=""modal-content"">
                    <div id=""details-modal-body"" class=""modal-body"">
                        <div class=""row"" style=""height: 100%;"">
                            <div class=""modal-control-group"">
                                <span id=""close-control"" class=""modal-control"" da");
            WriteLiteral(@"ta-dismiss=""modal""><i class=""ms-Icon ms-Icon--Clear""></i></span>
                            </div>
                            <div id=""modalVideoContent"" style=""margin-top: 40px; margin-left: 6px; margin-right: 6px;"">
                            </div>
                            <input id=""result-id"" type=""hidden"" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>



");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
