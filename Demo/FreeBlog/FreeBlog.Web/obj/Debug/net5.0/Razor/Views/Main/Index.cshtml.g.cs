#pragma checksum "D:\工作代码2021\2021\中院深化全国文明单位创建管理系统\参考\FreeBlog\FreeBlog.Web\Views\Main\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4462552487f5c030ac1b9c41c94648bd0777c04d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Main_Index), @"mvc.1.0.view", @"/Views/Main/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4462552487f5c030ac1b9c41c94648bd0777c04d", @"/Views/Main/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c7755a82dba6de3ec1aff7ba0d9b59c8ef0a372", @"/Views/_ViewImports.cshtml")]
    public class Views_Main_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<style scoped>
    .ivu-card-body {
        padding: 0;
    }

    .layout {
        /*border: 1px solid #d7dde4;*/
        background: #f5f7f9;
        position: relative;
        /*border-radius: 4px;*/
        overflow: hidden;
    }

    .layout-logo {
        /*width: 100px;*/
        height: 30px;
        /*background: #5b6270;
            border-radius: 3px;*/
        float: left;
        position: relative;
        /*top: 15px;*/
        left: 20px;
        color: #fff;
        font-size: 22px;
    }

    .layout-nav {
        /*width: 420px;*/
        width: 200px;
        margin: 0 auto;
        /*margin-right: 20px;*/
        margin-right: 0;
    }

    .layout-header-bar {
        background: #fff;
        box-shadow: 0 1px 1px rgba(0,0,0,.1);
    }

    .menu-item span {
        display: inline-block;
        overflow: hidden;
        width: 90px;
        text-overflow: ellipsis;
        white-space: nowrap;
        vertical-align: bottom;
        tra");
            WriteLiteral(@"nsition: width .2s ease .2s;
    }

    .menu-item i {
        transform: translateX(0px);
        transition: font-size .2s ease, transform .2s ease;
        vertical-align: middle;
        font-size: 16px;
    }

    .collapsed-menu span {
        width: 0px;
        transition: width .2s ease;
    }

    .collapsed-menu i {
        transform: translateX(5px);
        transition: font-size .2s ease .2s, transform .2s ease .2s;
        vertical-align: middle;
        font-size: 22px;
    }
</style>
");
#nullable restore
#line 69 "D:\工作代码2021\2021\中院深化全国文明单位创建管理系统\参考\FreeBlog\FreeBlog.Web\Views\Main\Index.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<input type=\"hidden\" id=\"hfMenu\"");
            BeginWriteAttribute("value", " value=\"", 1609, "\"", 1630, 1);
#nullable restore
#line 70 "D:\工作代码2021\2021\中院深化全国文明单位创建管理系统\参考\FreeBlog\FreeBlog.Web\Views\Main\Index.cshtml"
WriteAttributeValue("", 1617, ViewBag.Menu, 1617, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" />
<div id=""app"" class=""layout"">
    <Header>
        <i-Menu mode=""horizontal"" theme=""dark"" active-name=""1"">
            <div class=""layout-logo"">通用后台管理系统</div>
            <div class=""layout-nav"">
                <Submenu name=""3000"">
                    <template slot=""title"">
                        <Icon type=""md-person""></Icon>
                        ");
#nullable restore
#line 79 "D:\工作代码2021\2021\中院深化全国文明单位创建管理系统\参考\FreeBlog\FreeBlog.Web\Views\Main\Index.cshtml"
                   Write(ViewBag.NickName);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                    </template>
                    <Menu-Item name=""3-1"" v-on:click.native=""modifyPassword()""><Icon type=""ios-construct""></Icon>修改密码</Menu-Item>
                    <Menu-Item name=""3-2"" v-on:click.native=""signOut()""><Icon type=""md-exit""></Icon>退出登录</Menu-Item>
                </Submenu>
");
            WriteLiteral(@"            </div>
        </i-Menu>
    </Header>
    <Layout>
        <Sider :style=""{position: 'fixed', height: '100vh', left: 0, overflow: 'auto'}"" collapsible :collapsed-width=""78"" v-model=""isCollapsed"">
            <i-Menu theme=""dark"" width=""auto"" :open-names=""['0']"" active-name=""0"" accordion=""true"" :class=""menuitemClasses"">
");
            WriteLiteral(@"                <Submenu v-bind:name=""index"" v-for=""(item,index) in list"">
                    <template slot=""title"">
                        <Icon v-bind:type=""item.icon""></Icon>
                        <span>{{item.name}}</span>
                    </template>
                    <Menu-Item v-bind:name=""index+'-'+index2"" v-for=""(item2,index2) in item.childMenus"" v-on:click.native=""gourl(item2.url,item2.name)""><span>{{item2.name}}</span></Menu-Item>
                </Submenu>
            </i-Menu>
        </Sider>
        <Layout :style=""{marginLeft:isCollapsed?'78px': '200px'}"">
            <Header :style=""{background: '#fff', boxShadow: '0 2px 3px 2px rgba(0,0,0,.1)'}""></Header>
            <Content :style=""{padding: '0 16px 16px'}"">
                <Breadcrumb :style=""{margin: '16px 0'}"">
                    <Breadcrumb-Item v-on:click.native=""gourl('/Main/Welcome','')"">Home</Breadcrumb-Item>
");
            WriteLiteral(@"                    <Breadcrumb-Item>{{localhost}}</Breadcrumb-Item>
                </Breadcrumb>
                <Card>
                    <!--<div id=""Content"" style=""height: 600px"">-->
                    <!--Content-->
                    <!--<router-link to=""HtmlPage.html"">Go to Foo</router-link>-->
                    <!--<router-view></router-view>-->
                    <!--</div>-->
                    <iframe v-bind:src=""src"" frameborder=""0"" style=""width:100%"" id=""myiframe"" scrolling=""yes""></iframe> <!--onload=""changeFrameHeight()""-->
                </Card>
            </Content>
        </Layout>
    </Layout>
</div>
<script>
    let that;
    let app = new Vue({
        el: '#app',
        data: {
            value: [20, 50],
            src: '/Main/Welcome',
            localhost: '',
            list: [],
            isCollapsed: false,
        },
        created: function () {
            that = this;
            that.list = JSON.parse($(""#hfMenu"").val()); // 菜单数据");
            WriteLiteral(@"
        },
        mounted: function () {
            that.changeFrameHeight();   // 设置高度
        },
        methods: {
            changeFrameHeight() {
                const deviceHeight = document.documentElement.clientHeight;
                document.getElementById(""myiframe"").height = (Number(deviceHeight) - 137) + 'px';   // 减去头部和外框的高度
            },
            gourl: function (url, name) {
                if (that.src == url) {
                    that.src = url + ""?t=1"";    // 用于点击同页面进行刷新
                } else {
                    that.src = url;
                }
                that.localhost = name;
            },
            info() {
                this.$Message.info('这是一条普通的提醒');
            },
            modifyPassword() {
                layers.url('修改密码', 450, 300, ""/User/ModifyPassword/"");
            },
            signOut() {
                window.location.replace('/Index/SignOut')
            }
        },
        computed: {
            menuitemClasses: f");
            WriteLiteral(@"unction () {
                return [
                    'menu-item',
                    this.isCollapsed ? 'collapsed-menu' : ''
                ]
            }
        },
    });
    window.onresize = function () {
        app.changeFrameHeight();
    }
</script>");
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
