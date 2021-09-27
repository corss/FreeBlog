var index = parent.layer.getFrameIndex(window.name);
(function () {
    window.layers = {
        lalert: function (content, t, icon, js) {
            icon = icon == undefined ? 0 : icon;
            var title = t;
            if (t > 0) { title = t + "秒后关闭"; } else { t = 0; }
            var alerts = parent.layer.alert(content, { icon: icon, title: title, time: t * 1000, scrollbar: false, end: function () { eval(js); } });
            if (t > 0) {
                var title = function () { t--; parent.layer.title(t + "秒后关闭", alerts); }
                var inter = setInterval(title, 1000);
                setTimeout(function () { clearInterval(inter); }, t * 1000);
            }
        },
        //type 1(text) or 2(iframe),title标题,width宽,height高,content内容,js事件,move是否拖拽,scrollbar是否禁用浏览器滚动条
        open: function (type, title, width, height, content, js, move, scrollbar) {
            type = type == undefined ? 1 : type;
            move = (move == undefined || move == true) ? ".layui-layer-title" : false;
            scrollbar = scrollbar == undefined ? false : scrollbar;
            var indexs= layer.open({
                type: type, title: title, area: [width + 'px', height + 'px'], fix: true, move: move, maxmin: false, scrollbar: scrollbar, content: content,
                end: function () { eval(js); }
            });
            return indexs;
        },
        alertOK: function (content, t, js) { layers.lalert(content, t, 1, js); },
        alertNO: function (content, t, js) { layers.lalert(content, t, 2, js); },
        Info: function (content, t, js) { layers.lalert(content, t, 0, js); },
        confirm: function (content, title, js) { parent.layer.confirm(content, { icon: 3, title: title }, function (index) { parent.layer.close(index); eval(js); }); },
        url: function (title, width, height, content, js, move, scrollbar) { return layers.open(2, title, width, height, content, js, move, scrollbar); },
        text: function (title, width, height, content, js, move, scrollbar) { return layers.open(1, title, width, height, content, js, move, scrollbar); },
       
        close: function () { var index = parent.layer.getFrameIndex(window.name); parent.layer.close(index); },
        //单个iframe使用
        sxs: function () { var ifr = parent.document.getElementById('ifm'); if (ifr != null) { var win = ifr.window || ifr.contentWindow; win.sx(); } else { sx(); } },
        sxs2: function () { var ifr = parent.document.getElementById('ifm'); if (ifr != null) { var win = ifr.window || ifr.contentWindow; win.sx(); } else { sx(); } },
        //回调,用于现在多个iframe使用
        callback: function (js) { var id = parent.$(".ui-tabs-active").attr("aria-controls"); var ifr = parent.$("#" + id + " #ifm")[0]; if (ifr != null) { var win = ifr.window || ifr.contentWindow; win.eval(js); } },
        SonCallback: function (ifrId, js) { if(ifrId!=0){ var win = parent.window["layui-layer-iframe" + ifrId]; win.eval(js); }else{parent.eval(js);} }
    };
})();