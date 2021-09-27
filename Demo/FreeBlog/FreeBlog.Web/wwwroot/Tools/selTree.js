/**
 * zTree插件：下拉框选择
 * 
 * @author  ZhangJun
 * @url     http://www.314p.com
 * @name    jquery.selTree.js
 * @since   2017-6-24 17:30:00
 */
(function ($) {
    //初始化绑定默认的属性
    $.treeDefaults = $.treeDefaults || {};
    $.treeDefaults.property = {
        TreeId: "zTree",//树ID
        TreeMenuId: "zTreeMenu",//树的外框ID
        TreeKey: "",
        TreeVal: "",
        BgColor: "#fff",
        borderColor: "#ddd",
        radioType: "all",//all整棵树单选，level同一级单选
        chkboxTypeY: "ps",//勾选时 p关联父 s关联子
        chkboxTypeN: "ps",//取消勾选时 p关联父 s关联子
        IsRadio: true,//是否单选
        ParentRelative: false,//父级是否相对定位
        zIndex:19910212,
        Date: [],//数据元
    };
    $.fn.selTree = function (options) {
        var tree = function (parentObj) {
            var p = $.extend({}, $.treeDefaults.property, options || {}),
                check = { enable: true, chkStyle: "radio", radioType: p.radioType };
            if (!p.IsRadio) {
                check = { enable: true, chkboxType: { "Y": p.chkboxTypeY, "N": p.chkboxTypeN } };
            }
            var treeClick = function (e, treeId, treeNode) {
                var zTree = $.fn.zTree.getZTreeObj(p.TreeId);
                zTree.checkNode(treeNode, !treeNode.checked, null, true);
                return false;
            }, treeCheck = function (e, treeId, treeNode) {
                var zTree = $.fn.zTree.getZTreeObj(p.TreeId),
                nodes = zTree.getCheckedNodes(true);
                var key = [], val = [];
                for (var i = 0, l = nodes.length; i < l; i++) {
                    key.push(nodes[i].name);
                    val.push(nodes[i].id);
                }
                $(p.TreeKey).val(key.join(","));
                $(p.TreeVal).val(val.join(","));
            }, showMenu = function (t) {
                $("#" + p.TreeMenuId).slideDown("fast");
                var Offset = parentObj.offset();
                if (!p.ParentRelative)
                    $("#" + p.TreeMenuId).css({ left: Offset.left + "px", top: Offset.top + parentObj.outerHeight() + "px" }).slideDown("fast");
                $("body").bind("mousedown", onBodyDown);
            }, hideMenu = function () {
                $("#" + p.TreeMenuId).fadeOut("fast");
                $("body").unbind("mousedown", onBodyDown);
            }, onBodyDown = function (event) {
                if (!(event.target.id == parentObj.attr("id") || event.target.id == p.TreeMenuId || $(event.target).parents("#" + p.TreeMenuId).length > 0)) {
                    hideMenu();
                }
            }, setting = {
                check: check,
                view: { dblClickExpand: false, showIcon: false },
                data: { simpleData: { enable: true } },
                callback: { onClick: treeClick, onCheck: treeCheck }
            }
            $(parentObj).after('<div id="' + p.TreeMenuId + '" style="position: absolute;display: none;border-radius: 4px;background-color:' + p.BgColor + ';border:1px solid ' + p.borderColor + ';z-index:' + p.zIndex + ';max-height:300px;overflow-y: scroll;"><ul id="' + p.TreeId + '" class="ztree"></ul></div>')
                        .on("click", function () { showMenu(this); });
            $.fn.zTree.init($("#" + p.TreeId), setting, p.Date);
        }
        return $(this).each(function () { tree($(this)); });
    }
})(jQuery);