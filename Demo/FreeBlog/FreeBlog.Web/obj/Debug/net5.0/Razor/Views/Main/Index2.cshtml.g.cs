#pragma checksum "D:\工作代码2021\2021\中院深化全国文明单位创建管理系统\参考\FreeBlog\FreeBlog.Web\Views\Main\Index2.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b582fe5d7c90ba0f218e3a4726ebd3856d3d040a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Main_Index2), @"mvc.1.0.view", @"/Views/Main/Index2.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b582fe5d7c90ba0f218e3a4726ebd3856d3d040a", @"/Views/Main/Index2.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c7755a82dba6de3ec1aff7ba0d9b59c8ef0a372", @"/Views/_ViewImports.cshtml")]
    public class Views_Main_Index2 : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/Tools/layui/css/layui.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/Tools/layui/layui.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            WriteLiteral("\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "b582fe5d7c90ba0f218e3a4726ebd3856d3d040a3810", async() => {
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
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "b582fe5d7c90ba0f218e3a4726ebd3856d3d040a4924", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"

<style>
    iframe {
        width: 100%;
        height: 100%;
    }

    .layui-body {
        overflow: visible;
    }

    #page-caozuo {
        height: 40px;
        width: 100%;
    }

    #menu .layui-nav .layui-nav-item a {
        padding: 0 20px 0 10px;
    }

    .layui-layout-admin .layui-body {
        position: fixed;
        top: 60px;
        /*bottom: 44px;*/
        bottom: 0;
        left: 200px;
    }
</style>

<input type=""hidden"" id=""hfMenu""");
            BeginWriteAttribute("value", " value=\"", 612, "\"", 633, 1);
#nullable restore
#line 34 "D:\工作代码2021\2021\中院深化全国文明单位创建管理系统\参考\FreeBlog\FreeBlog.Web\Views\Main\Index2.cshtml"
WriteAttributeValue("", 620, ViewBag.Menu, 620, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n<div class=\"layui-layout layui-layout-admin\">\r\n    <div class=\"layui-header\">\r\n        <div class=\"layui-logo\">后台管理系统</div>\r\n\r\n        <ul class=\"layui-nav layui-layout-left\">\r\n");
            WriteLiteral(@"        </ul>
        <ul class=""layui-nav layui-layout-right"">
            <li class=""layui-nav-item"">
                <a href=""javascript:;"">
                    <img src=""images/DefaultHead.jpg"" class=""layui-nav-img"">
                    张三
                </a>
                <dl class=""layui-nav-child"">
                    <dd><a");
            BeginWriteAttribute("href", " href=\"", 1724, "\"", 1731, 0);
            EndWriteAttribute();
            WriteLiteral(">基本资料</a></dd>\r\n                    <dd><a");
            BeginWriteAttribute("href", " href=\"", 1774, "\"", 1781, 0);
            EndWriteAttribute();
            WriteLiteral(">安全设置</a></dd>\r\n                </dl>\r\n            </li>\r\n            <li class=\"layui-nav-item\"><a");
            BeginWriteAttribute("href", " href=\"", 1881, "\"", 1888, 0);
            EndWriteAttribute();
            WriteLiteral(">退出</a></li>\r\n        </ul>\r\n    </div>\r\n    <div id=\"menu\" class=\"layui-side layui-bg-black\">\r\n    </div>\r\n    <div class=\"layui-body\">\r\n        <iframe");
            BeginWriteAttribute("src", " src=\"", 2042, "\"", 2048, 0);
            EndWriteAttribute();
            WriteLiteral(" frameborder=\"0\"></iframe>\r\n    </div>\r\n");
            WriteLiteral(@"</div>


<script>
    // 导航菜单的间隔像素
    var menuCell = 1;

    layui.use('element', function () {
        var element = layui.element;
        var $ = layui.jquery;

        data = [
            {
                ""name"": ""信息管理"",
                ""url"": """",
                ""icon"": ""layui-icon-snowflake"",
                ""childMenus"": [
                    {
                        ""name"": ""页面1"",
                        ""url"": ""https://www.layui.com/admin/std/dist/views/home/homepage2.html"",
                        ""icon"": ""layui-icon-snowflake"",
                        ""childMenus"": null
                    },
                    {
                        ""name"": ""页面2"",
                        ""url"": ""https://www.layui.com/admin/std/dist/views/home/homepage2.html"",
                        ""icon"": ""layui-icon-snowflake"",
                        ""childMenus"": null
                    }
                ]
            },
            {
                ""name"": ""一级导航"",
                ""u");
            WriteLiteral(@"rl"": ""www.manager.com"",
                ""icon"": ""layui-icon-snowflake"",
                ""childMenus"": [
                    {
                        ""name"": ""二级导航"",
                        ""url"": ""pages/table.html"",
                        ""icon"": ""layui-icon-snowflake"",
                        ""childMenus"": [
                            {
                                ""name"": ""三级导航"",
                                ""url"": ""pages/table.html"",
                                ""icon"": ""layui-icon-snowflake"",
                                ""childMenus"": [
                                    {
                                        ""name"": ""table页面"",
                                        ""url"": ""pages/table.html"",
                                        ""icon"": ""layui-icon-snowflake"",
                                        ""childMenus"": null
                                    }
                                ]
                            },
                            {
            ");
            WriteLiteral(@"                    ""name"": ""table页面"",
                                ""url"": ""pages/table.html"",
                                ""icon"": ""layui-icon-snowflake"",
                                ""childMenus"": null
                            }
                        ]
                    }
                ]
            }
        ];

        data = JSON.parse($(""#hfMenu"").val());
        getMenu(data);
        element.init();


        // 页面切换
        $(document).on('click', '#menu a', function () {
            var thisPage = $(this).attr(""data-url"");
            if (typeof (thisPage) != ""undefined"") {
                if ($('.layui-body iframe').attr('src') == thisPage) return;
                $('.layui-body iframe').attr('src', thisPage)
            }
        });
    });

    function getMenu(data) {
        //console.log(""data: "", data);
        //      data = JSON.parse(data);
        var liStr = """";
        // 遍历生成主菜单
        for (var i = 0; i < data.length; i++) {
         ");
            WriteLiteral(@"   //console.log(""--> ""+JSON.stringify(data[i]));
            // 判断是否存在子菜单
            if (data[i].childMenus != null && data[i].childMenus.length > 0) {
                //console.log(""--> "" + JSON.stringify(data[i].childMenus));
                liStr += ""<li class=\""layui-nav-item\""><a class=\""\"" href=\""javascript:;\""><i class='layui-icon "" + data[i].icon + ""'></i> "" + data[i].name + ""</a>\n"" +
                    ""<dl class=\""layui-nav-child\"">\n"";
                // 遍历获取子菜单
                for (var k = 0; k < data[i].childMenus.length; k++) {
                    liStr += getChildMenu(data[i].childMenus[k], 0);
                }
                liStr += ""</dl></li>"";
            } else {
                liStr += ""<li class=\""layui-nav-item\""><a class=\""\"" href=\""javascript:;\"" data-url=\"""" + data[i].url + ""\""><i class='layui-icon "" + data[i].icon + ""'></i> "" + data[i].name + ""</a></li>"";
            }
        };
        //console.log("">>>> "" + liStr);
        $(""#menu"").html(""<ul class=\""lay");
            WriteLiteral(@"ui-nav layui-nav-tree\""  lay-filter=\""test\"">\n"" + liStr + ""</ul>"");

    }

    // 递归生成子菜单
    function getChildMenu(subMenu, num) {
        //console.log(""num: "" + num);
        num++;
        var subStr = """";
        if (subMenu.childMenus != null && subMenu.childMenus.length > 0) {
            subStr += ""<dd><ul><li class=\""layui-nav-item\"" ><a style=\""text-indent: "" + num * menuCell + ""em\"" class=\""\"" href=\""javascript:;\""><i class='layui-icon "" + subMenu.icon + ""'></i> "" + subMenu.name + ""</a>"" +
                ""<dl class=\""layui-nav-child\"">\n"";
            for (var j = 0; j < subMenu.childMenus.length; j++) {
                subStr += getChildMenu(subMenu.childMenus[j], num);
            }
            subStr += ""</dl></li></ul></dd>"";
        } else {
            subStr += ""<dd><a style=\""text-indent:"" + num * menuCell + ""em\"" href=\""javascript:;\"" data-url=\"""" + subMenu.url + ""\""><i class='layui-icon "" + subMenu.icon + ""'></i> "" + subMenu.name + ""</a></dd>"";
        }
        retu");
            WriteLiteral("rn subStr;\r\n    }\r\n</script>");
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