﻿$(function () {
    //初始化绑定默认的属性
    $.upLoadDefaults = $.upLoadDefaults || {};
    $.upLoadDefaults.property = {
        multiple: false, //是否多文件
        water: false, //是否加水印
        thumbnail: false, //是否生成缩略图
        sendurl: "../../api/Upload", //发送地址
        filetypes: "jpg,jpge,png,gif", //文件类型
        filesize: "2048", //文件大小
        btntext: "浏览...", //上传按钮的文字
        swf: "uploader.swf", //SWF上传控件相对地址

        control: ".upload-path", //指定控件
        weburl: "", //网站地址，用于图片正常显示
        filepath: "", //网站文件路径
        controlImg: ""//图片控件
    };
    //初始化上传控件
    $.fn.InitUploader = function (b) {
        var fun = function (parentObj) {
            var p = $.extend({}, $.upLoadDefaults.property, b || {});
            var btnObj = $('<div class="upload-btn">' + p.btntext + '</div>').appendTo(parentObj);
            //初始化属性
            //p.sendurl += "?type=UpLoadFile&filetypes=" + p.filetypes;
            p.sendurl += "?action=UpLoadFile&filetypes=" + p.filetypes;
            if (p.water) {
                p.sendurl += "&IsWater=1";
            }
            if (!p.multiple) {
                p.sendurl += "&DelFilePath=" + parentObj.siblings(p.control).val();
            }

            //初始化WebUploader
            var uploader = WebUploader.create({
                //runtimeOrder: 'flash',
                auto: true, //自动上传
                swf: p.weburl + p.filepath + p.swf, //SWF路径
                server: p.weburl + p.filepath + p.sendurl, //上传地址
                pick: {
                    id: btnObj,
                    multiple: p.multiple
                },
                accept: {
                    /*title: 'Images',*/
                    extensions: p.filetypes
                    /*mimeTypes: 'image/*'*/
                },
                //compress: {
                //    width: 1000,
                //    height: 1000,
                //    // 图片质量，只有type为`image/jpeg`的时候才有效。
                //    quality: 90,
                //    // 是否允许放大，如果想要生成小图的时候不失真，此选项应该设置为false.
                //    allowMagnify: false,
                //    // 是否允许裁剪。
                //    crop: false,
                //    // 是否保留头部meta信息。
                //    preserveHeaders: true,
                //    // 如果发现压缩后文件大小比原来还大，则使用原来图片
                //    // 此属性可能会影响图片自动纠正功能
                //    noCompressIfLarger: false,
                //    //支部字节，如果图片大小小于此值，不会采用压缩。
                //    compressSize: 0
                //},
                compress: false,
                formData: {
                    'DelFilePath': '' //定义参数
                },
                fileVal: 'Filedata', //上传域的名称
                fileSingleSizeLimit: p.filesize * 1024 //文件大小
            });

            //当validate不通过时，会以派送错误事件的形式通知
            uploader.on('error', function (type) {
                switch (type) {
                    case 'Q_EXCEED_NUM_LIMIT':
                        alert("错误：上传文件数量过多！");
                        break;
                    case 'Q_EXCEED_SIZE_LIMIT':
                        alert("错误：文件总大小超出限制！");
                        break;
                    case 'F_EXCEED_SIZE':
                        alert("错误：文件大小超出限制！");
                        break;
                    case 'Q_TYPE_DENIED':
                        alert("错误：禁止上传该类型文件！");
                        break;
                    case 'F_DUPLICATE':
                        alert("错误：请勿重复上传该文件！");
                        break;
                    default:
                        alert('错误代码：' + type);
                        break;
                }
            });

            //当有文件添加进来的时候
            uploader.on('fileQueued', function (file) {
                //如果是单文件上传，把旧的文件地址传过去
                if (!p.multiple) {
                    uploader.options.formData.DelFilePath = parentObj.siblings(p.control).val();
                }
                //防止重复创建
                if (parentObj.children(".upload-progress").length == 0) {
                    //创建进度条
                    var fileProgressObj = $('<div class="upload-progress"></div>').appendTo(parentObj);
                    var progressText = $('<span class="txt">正在上传，请稍候...</span>').appendTo(fileProgressObj);
                    var progressBar = $('<span class="bar"><b></b></span>').appendTo(fileProgressObj);
                    var progressCancel = $('<a class="close" title="取消上传">关闭</a>').appendTo(fileProgressObj);
                    //绑定点击事件
                    progressCancel.click(function () {
                        uploader.cancelFile(file);
                        fileProgressObj.remove();
                    });
                }
            });

            //文件上传过程中创建进度条实时显示
            uploader.on('uploadProgress', function (file, percentage) {
                var progressObj = parentObj.children(".upload-progress");
                progressObj.children(".txt").html(file.name);
                progressObj.find(".bar b").width(percentage * 100 + "%");
            });

            //当文件上传出错时触发
            uploader.on('uploadError', function (file, reason) {
                uploader.removeFile(file); //从队列中移除
                alert(file.name + "上传失败，错误代码：" + reason);
            });

            //当文件上传成功时触发
            uploader.on('uploadSuccess', function (file, data) {
                if (data.status == '0') {
                    var progressObj = parentObj.children(".upload-progress");
                    progressObj.children(".txt").html(data.msg);
                }
                if (data.status == '1') {
                    //如果是单文件上传，则赋值相应的表单
                    if (!p.multiple) {
                        parentObj.siblings(".upload-name").val(data.name);
                        parentObj.siblings(p.control).val(data.path);
                        parentObj.siblings(".upload-size").val(data.size);
                        if (p.controlImg != "")
                            $(p.controlImg).attr("src", p.weburl + data.path);
                    } else {
                        addImage(parentObj, data.path, data.thumb, p.weburl);
                    }
                    var progressObj = parentObj.children(".upload-progress");
                    progressObj.children(".txt").html("上传成功");
                }
                uploader.removeFile(file); //从队列中移除
            });

            //不管成功或者失败，文件上传完成时触发
            uploader.on('uploadComplete', function (file) {
                var progressObj = parentObj.children(".upload-progress");
                //progressObj.children(".txt").html("上传完成");
                //如果队列为空，则移除进度条
                if (uploader.getStats().queueNum == 0) {
                    setTimeout(function () {
                        progressObj.remove();
                    }, 3500);
                }
            });
        };
        return $(this).each(function () {
            fun($(this));
        });
    }
});

/*图片相册处理事件
=====================================================*/
//添加图片相册
function addImage(targetObj, originalSrc, thumbSrc, webUrl) {
    //插入到相册UL里面
    var newLi = $('<li>'
    + '<input type="hidden" name="hid_photo_name" value="0|' + originalSrc + '" />'
    + '<div class="img-box" onclick="setFocusImg(this);">'
    + '<img src="' + webUrl + thumbSrc + '" bigsrc="' + webUrl + originalSrc + '" />'
    + '</div>'
    + '<a href="javascript:;" onclick="delImg(this);">删除</a>'
    + '</li>');
    newLi.appendTo(targetObj.siblings(".photo-list").children("ul"));

    //默认第一个为相册封面
    var focusPhotoObj = targetObj.siblings(".focus-photo");
    if (focusPhotoObj.val() == "") {
        focusPhotoObj.val(thumbSrc);
        newLi.children(".img-box").addClass("selected");
    }
}
//设置相册封面
function setFocusImg(obj) {
    var focusPhotoObj = $(obj).parents(".photo-list").siblings(".focus-photo");
    focusPhotoObj.val($(obj).children("img").eq(0).attr("src"));
    $(obj).parent().siblings().children(".img-box").removeClass("selected");
    $(obj).addClass("selected");
}
//删除图片LI节点
function delImg(obj) {
    var parentObj = $(obj).parent().parent();
    var focusPhotoObj = parentObj.parent().siblings(".focus-photo");
    var smallImg = $(obj).siblings(".img-box").children("img").attr("src");
    $(obj).parent().remove(); //删除的LI节点
    //检查是否为封面
    if (focusPhotoObj.val() == smallImg) {
        focusPhotoObj.val("");
        var firtImgBox = parentObj.find("li .img-box").eq(0); //取第一张做为封面
        firtImgBox.addClass("selected");
        focusPhotoObj.val(firtImgBox.children("img").attr("src")); //重新给封面的隐藏域赋值
    }
}