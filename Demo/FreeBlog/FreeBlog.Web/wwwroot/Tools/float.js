﻿/* 
scrollx({
    id: 'aa',
    l: 1200,
    t: 300
})
id 你要滚动的内容的id 
l 横坐标的位置 不写为紧贴右边 
t 你要放在页面的那个位置默认是贴着底边 0是贴着顶边 
f 1表示固定 不写或者0表示滚动 
*/
function scrollx(p) {var d = document, dd = d.documentElement, db = d.body, w = window, o = d.getElementById(p.id), ie6 = /msie 6/i.test(navigator.userAgent), style, timer;if (o) {o.style.cssText += ";position:" + (p.f && !ie6 ? 'fixed' : 'absolute') + ";" + (p.l == undefined ? 'right:0;' : 'left:' + p.l + 'px;') + (p.t != undefined ? 'top:' + p.t + 'px' : 'bottom:0');if (p.f && ie6) {o.style.cssText += ';left:expression(documentElement.scrollLeft + ' + (p.l == undefined ? dd.clientWidth - o.offsetWidth : p.l) + ' + "px");top:expression(documentElement.scrollTop +' + (p.t == undefined ? dd.clientHeight - o.offsetHeight : p.t) + '+ "px" );';dd.style.cssText += ';background-image: url(about:blank);background-attachment:fixed;';} else {if (!p.f) {w.onresize = w.onscroll = function () {clearInterval(timer);timer = setInterval(function () {/*双选择为了修复chrome 下xhtml解析时dd.scrollTop为 0*/ var st = (dd.scrollTop || db.scrollTop), c;c = st - o.offsetTop + (p.t != undefined ? p.t : (w.innerHeight || dd.clientHeight) - o.offsetHeight);if (c != 0) {o.style.top = o.offsetTop + Math.ceil(Math.abs(c) / 10) * (c < 0 ? -1 : 1) + 'px';} else {clearInterval(timer);}}, 10)}}}}}