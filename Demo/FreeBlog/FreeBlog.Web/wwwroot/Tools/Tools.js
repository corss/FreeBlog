function no_head(t) { $(t).attr("onerror", "").attr("src", "../images/no_head.jpg"); }
//JS获取url参数
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
};
/* 获取系统平台 */
function GetBrowserPlatform() {
    //let na = navigator;
    let ua = navigator.userAgent.toLowerCase();
    let BrowserPlatform = "";
    if (ua.match(/micromessenger/i)) { BrowserPlatform = "微信"; }
    else if (ua.match(/windows/i)) { BrowserPlatform = "Windows"; }
    else if (ua.match(/android/i)) { BrowserPlatform = "Android"; }
    else if (ua.match(/iphone/i)) { BrowserPlatform = "iPhone"; }
    else if (ua.match(/ipad/i)) { BrowserPlatform = "iPad"; }
    else if (ua.match(/ipod/i)) { BrowserPlatform = "iPod"; }
    else if (ua.match(/ios/i)) { BrowserPlatform = "IOS"; }
    /*
     na.platform
     na.appVersion
     */
    return BrowserPlatform;
}
/* 获取浏览器版本 */
function getExplore() {
    let Sys = {};
    let ua = navigator.userAgent.toLowerCase();
    let s;
    (s = ua.match(/rv:([\d.]+)\) like gecko/)) ? Sys.ie = s[1] :
        (s = ua.match(/msie ([\d\.]+)/)) ? Sys.ie = s[1] :
            (s = ua.match(/edge\/([\d\.]+)/)) ? Sys.edge = s[1] :
                (s = ua.match(/firefox\/([\d\.]+)/)) ? Sys.firefox = s[1] :
                    (s = ua.match(/(?:opera|opr).([\d\.]+)/)) ? Sys.opera = s[1] :
                        (s = ua.match(/chrome\/([\d\.]+)/)) ? Sys.chrome = s[1] :
                            (s = ua.match(/version\/([\d\.]+).*safari/)) ? Sys.safari = s[1] : 0;
    // 根据关系进行判断
    if (Sys.ie) return ('IE ' + Sys.ie);
    if (Sys.edge) return ('EDGE ' + Sys.edge);
    if (Sys.firefox) return ('Firefox ' + Sys.firefox);
    if (Sys.chrome) return ('Chrome ' + Sys.chrome);
    if (Sys.opera) return ('Opera ' + Sys.opera);
    if (Sys.safari) return ('Safari ' + Sys.safari);
    return 'Unkonwn';
}

//全选
function ckAll(t) { $("input[name='cks']").prop("checked", t); }
$(function () {
    setTableStyle();
});
function setTableStyle() {
    $(".table tbody tr:odd").addClass("odd");
    $(".table tbody tr").mouseover(function () { $(this).addClass("on"); })
        .mouseout(function () { $(this).removeClass("on"); })
        .click(function () { $(".table tbody tr").removeClass("on_ck"); $(this).addClass("on_ck"); });
    // 移除选中
    $(".table tbody tr").removeClass("on_ck");
}

/*****支部选择器*****/
function SelCompany() {
    var ifrId = parent.layer.getFrameIndex(window.name);
    if (Number(ifrId) > 0) {
        var index = parent.layer.open({
            type: 2, title: "选择支部", area: [1000 + 'px', 700 + 'px'], fix: true, maxmin: false, content: "../public/SelCompany.aspx?index=" + ifrId,
        });
    }
    else {
        var index = layers.url("选择支部", 1000, 700, "../public/SelCompany.aspx");
    }
}
function GetCompany(c, ID, Title) {
    $("#hfSearchCompanyID").val(ID);
    $("#hfCompany").val(Title);
    $("#txtCompany").val(Title);
}
function CompanyClear(t) {
    $("#hfSearchCompanyID").val("");
    $("#hfCompany").val("");
    $("#txtCompany").val("");
}

/*获取文件名*/
function getFileName(o) {
    var pos = o.lastIndexOf("\\");
    return o.substring(pos + 1);
}

/*获取表格勾选ID*/
function getTableSelectID(id) {
    if (!id) {
        id = "";
        // 获取勾选的ID
        $("input[name='cks']").each(function () { if ($(this).prop("checked")) { id += $(this).val() + ","; } });
    }
    return id;
}

/*列表新绑定数据后-取消勾选、设置样式*/
function tableBindStyle() {
    // 新绑定数取消原有勾选
    $("input[name='cks']").each(function () { $(this).removeAttr("checked"); });
    $("#ck").removeAttr("checked");
    // 设置表格样式
    setTimeout(function () {
        setTableStyle();
    }, 100);
}

/*查询条件：获取int类型的值*/
function getIntValue(value) {
    if (value == 0)
        return "";
    return value;
}
/*查询条件：获取Bool类型的值*/
function getBoolValue(value) {
    if (value == 0)
        return "";
    return value == 1 ? true : false;
}