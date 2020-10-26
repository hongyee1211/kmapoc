#pragma checksum "C:\Ashley\Code\kmapoc\CognitiveSearch.UI\Views\Category\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9a42c48e7f9ab7d92edd1df5cc9ee1f0b8686237"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Category_Index), @"mvc.1.0.view", @"/Views/Category/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9a42c48e7f9ab7d92edd1df5cc9ee1f0b8686237", @"/Views/Category/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"cd1c8ddb33f15e20aee94317c529990ec82e9754", @"/Views/_ViewImports.cshtml")]
    public class Views_Category_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<CognitiveSearch.UI.Models.CategoryViewModel>
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
            WriteLiteral("\r\n");
#nullable restore
#line 6 "C:\Ashley\Code\kmapoc\CognitiveSearch.UI\Views\Category\Index.cshtml"
  
    ViewData["Title"] = "Ratings";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<script>\r\n    categories = ");
#nullable restore
#line 11 "C:\Ashley\Code\kmapoc\CognitiveSearch.UI\Views\Category\Index.cshtml"
            Write(Html.Raw(Json.Serialize(Model.categories)));

#line default
#line hidden
#nullable disable
            WriteLiteral(@";
    console.log(categories);
    currentPage = 0;

    function updateTable(query = null) {
        var table = $(""#ratings-table-body"")
        table.empty();
        rowsHtml = '';
        rowCount = 0;
        for (i in categories) {
            category = categories[i]
            rowCount += 1
            rowsHtml += `<tr class=""ratings-table-rows"">
                                <th scope=""row"">${rowCount}</th>
                                <td>${category.category}</td>
                                <td>${category.annotation}</td>
                                <td>${category.name}</td>
                                <td class=""ratings-table-column-align ratings-table-delete"" onclick=""removeCategory('${category.id}');""><span class=""fa fa-trash""></span></td>
                            </tr>`
        }
        table.html(rowsHtml);
    }

    function removeCategory(id) {
        $.post(""/Category/DeleteAnnotation"",
            {
                tagId: id,
            ");
            WriteLiteral(@"},
            function (data, status) {
                if (status == ""success"") {
                    categories = data
                    updateTable()
                }
            }
        );
    };

    function handleFilter() {
        var input = $(""#rating-filter-input"").val();
        updateTable(input)
    }

    $(function () {
        updateTable()
    });
</script>
<link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"">
");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "9a42c48e7f9ab7d92edd1df5cc9ee1f0b86862375940", async() => {
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
<div>
    <div class=""content ratings-content"">
        <table class=""table"">
            <thead class=""ratings-table-header"">
                <tr>
                    <th scope=""col"">#</th>
                    <th scope=""col"">Facet</th>
                    <th scope=""col"">Annotation</th>
                    <th scope=""col"">User</th>
                    <th class=""ratings-table-column-align"" scope=""col""></th>
                </tr>
            </thead>
            <tbody id=""ratings-table-body"">
            </tbody>
        </table>
    </div>
</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<CognitiveSearch.UI.Models.CategoryViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591