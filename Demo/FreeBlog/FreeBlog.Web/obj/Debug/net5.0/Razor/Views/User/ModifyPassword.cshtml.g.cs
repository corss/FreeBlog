#pragma checksum "D:\工作代码2021\2021\中院深化全国文明单位创建管理系统\参考\FreeBlog\FreeBlog.Web\Views\User\ModifyPassword.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "facff5b60b3aebaa2e5d2ed2a71a9c174a68b6d1"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_User_ModifyPassword), @"mvc.1.0.view", @"/Views/User/ModifyPassword.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"facff5b60b3aebaa2e5d2ed2a71a9c174a68b6d1", @"/Views/User/ModifyPassword.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c7755a82dba6de3ec1aff7ba0d9b59c8ef0a372", @"/Views/_ViewImports.cshtml")]
    public class Views_User_ModifyPassword : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<div id=\"app\" class=\"Edit\" style=\"padding: 10px;\">\r\n    <asp:HiddenField ID=\"hfID\"");
            BeginWriteAttribute("Value", " Value=\"", 82, "\"", 90, 0);
            EndWriteAttribute();
            WriteLiteral(@" runat=""server""></asp:HiddenField>
    <table width=""100%"" class=""form"">
        <tr class=""row "">
            <td align=""right"" width=""80"">
                <label class=""form-label"" for=""oldPassword"">
                    <span class="" c-red"">*</span>原密码:
                </label>
            </td>
            <td>
                <input type=""password"" class=""input-text"" maxlength=""20"" autocomplete=""off"" v-model=""oldPassword"" />
            </td>
        </tr>
        <tr class=""row "">
            <td align=""right"" width=""80"">
                <label class=""form-label"" for=""newPassword"">
                    <span class="" c-red"">*</span>新密码:
                </label>
            </td>
            <td>
                <input type=""password"" class=""input-text"" maxlength=""20"" autocomplete=""off"" v-model=""newPassword"" />
            </td>
        </tr>
        <tr class=""row "">
            <td align=""right"" width=""80"">
                <label class=""form-label"" for=""newPassword2"">
            ");
            WriteLiteral(@"        <span class="" c-red"">*</span>再次输入新密码:
                </label>
            </td>
            <td>
                <input type=""password"" class=""input-text"" maxlength=""20"" autocomplete=""off"" v-model=""newPassword2"" />
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <a id=""EditSubmit"" class=""btn btn-primary radius"" v-on:click=""EditSubmit"">&nbsp;&nbsp;确认修改&nbsp;&nbsp;</a>
                &nbsp; <a onclick=""layers.close(layers.index);"" class=""btn btn-default radius"">&nbsp;&nbsp;取消&nbsp;&nbsp;</a>
            </td>
        </tr>
    </table>
</div>

<script type=""text/javascript"">
    let that;
    let app = new Vue({
        el: '#app',
        data: {
            oldPassword: '',
            newPassword: '',
            newPassword2: ''
        },
        methods: {
            EditSubmit: function () {
                let that = this;
                if (!that.oldPassword) {
                    layers.Info(""原密码不能为空"");
    ");
            WriteLiteral(@"            } else if (!that.newPassword) {
                    layers.Info(""新密码不能为空"")
                } else if (!that.newPassword2) {
                    layers.Info(""再次输入密码不能为空"")
                } else if (that.newPassword.length < 6) {
                    layers.Info(""新密码不能小于6位数"")
                } else if (that.newPassword != that.newPassword2) {
                    layers.Info(""两次输入的密码不一致"")
                } else {
                    $.ajax({
                        type: ""post"",
                        url: ""/User/ModifyPassword"",
                        dataType: ""json"",
                        async: true,
                        data: { oldPassword: that.oldPassword, newPassword: that.newPassword },
                        success: function (e) {
                            if (e.success) {
                                layers.alertOK(e.msg, 5, 'sxs();layers.close();');
                            }
                            else {
                                layers.Info");
            WriteLiteral("(e.msg);\r\n                            }\r\n                        }\r\n                    });\r\n                }\r\n            },\r\n        }\r\n    });\r\n    function sxs() {\r\n        parent.location.href = \"/Index/Login\";\r\n    }\r\n</script>\r\n\r\n\r\n");
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
