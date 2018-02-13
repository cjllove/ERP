/*
上传组件，可根据需要进行扩展
需要jquery
@Author 鞠怡
@Date 2015/03/16
*/

(function ($) {
    var $wrap = $('#uploader'),

            // 图片容器
            $queue = $('<ul class="filelist"></ul>')
                .appendTo($wrap.find('.queueList')),

            // 状态栏，包括进度和控制按钮
            $statusBar = $wrap.find('.statusBar'),

            // 文件总体选择信息。
            $info = $statusBar.find('.info'),

            // 上传按钮
            $upload = $wrap.find('.uploadBtn'),

            // 没选择文件之前的内容。
            $placeHolder = $wrap.find('.placeholder'),

            $progress = $statusBar.find('.progress').hide(),

            // 添加的文件数量
            fileCount = 0,

            // 添加的文件总大小
            fileSize = 0,

            // 优化retina, 在retina下这个值是2
            ratio = window.devicePixelRatio || 1,

            // 缩略图大小
            thumbnailWidth = 110 * ratio,
            thumbnailHeight = 110 * ratio,

            // 可能有pedding, ready, uploading, confirm, done.
            state = 'pedding',

            // 所有文件的进度信息，key为file id
            percentages = {},
            // 判断浏览器是否支持图片的base64
            isSupportBase64 = (function () {
                var data = new Image();
                var support = true;
                data.onload = data.onerror = function () {
                    if (this.width != 1 || this.height != 1) {
                        support = false;
                    }
                }
                data.src = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";
                return support;
            })(),

            // 检测是否已经安装flash，检测flash的版本
            flashVersion = (function () {
                var version;

                try {
                    version = navigator.plugins['Shockwave Flash'];
                    version = version.description;
                } catch (ex) {
                    try {
                        version = new ActiveXObject('ShockwaveFlash.ShockwaveFlash')
                                .GetVariable('$version');
                    } catch (ex2) {
                        version = '0.0';
                    }
                }
                version = version.match(/\d+/g);
                return parseFloat(version[0] + '.' + version[1], 10);
            })(),

            supportTransition = (function () {
                var s = document.createElement('p').style,
                    r = 'transition' in s ||
                            'WebkitTransition' in s ||
                            'MozTransition' in s ||
                            'msTransition' in s ||
                            'OTransition' in s;
                s = null;
                return r;
            })(),

            // WebUploader实例
            uploader;

    if (!WebUploader.Uploader.support('flash') && WebUploader.browser.ie) {

        // flash 安装了但是版本过低。
        if (flashVersion) {
            (function (container) {
                window['expressinstallcallback'] = function (state) {
                    switch (state) {
                        case 'Download.Cancelled':
                            F.util.alert('您取消了更新！')
                            break;

                        case 'Download.Failed':
                            F.util.alert('安装失败')
                            break;

                        default:
                            F.util.alert('安装已成功，请刷新！');
                            break;
                    }
                    delete window['expressinstallcallback'];
                };

                var swf = './expressInstall.swf';
                // insert flash object
                var html = '<object type="application/' +
                        'x-shockwave-flash" data="' + swf + '" ';

                if (WebUploader.browser.ie) {
                    html += 'classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" ';
                }

                html += 'width="100%" height="100%" style="outline:0">' +
                    '<param name="movie" value="' + swf + '" />' +
                    '<param name="wmode" value="transparent" />' +
                    '<param name="allowscriptaccess" value="always" />' +
                '</object>';

                container.html(html);

            })($wrap);

            // 压根就没有安转。
        } else {
            $wrap.html('<a href="http://www.adobe.com/go/getflashplayer" target="_blank" border="0"><img alt="get flash player" src="http://www.adobe.com/macromedia/style_guide/images/160x41_Get_Flash_Player.jpg" /></a>');
        }

        return;
    } else if (!WebUploader.Uploader.support()) {
        F.util.alert('Web Uploader 不支持您的浏览器！');
        return;
    }
    var resetUploader = function ($this) {
        
        if (uploader) {
            var files = uploader.getFiles('inited');
            for (var i = 0; i < files.length;i++) {
                uploader.removeFile(files[i]);
            }
            var files1 = uploader.getFiles('complete');
            for (var i = 0; i < files1.length; i++) {
                uploader.removeFile(files1[i]);
            }

            var files2 = uploader.getFiles('error');
            for (var i = 0; i < files2.length; i++) {
                uploader.removeFile(files2[i]);
            }

            var files3 = uploader.getFiles('interrupt');
            for (var i = 0; i < files3.length; i++) {
                uploader.removeFile(files3[i]);
            }
            var files4 = uploader.getFiles('queued');
            for (var i = 0; i < files4.length; i++) {
                uploader.removeFile(files4[i]);
            }
            var files5 = uploader.getFiles('progress');
            for (var i = 0; i < files5.length; i++) {
                uploader.removeFile(files5[i]);
            }

            fileCount = 0

            // 添加的文件总大小
            fileSize = 0
            //setTimeout(function () {
            //uploader.reset();
            //updateTotalProgress();
                //$this.html('');
                //$this.html(methods.opts.html);
            //}, 100);
           
            
            //updateTotalProgress();
            _urlList = [];

        }
    }

    var rebindUploader = function ($this) {
        uploader.options.formData = methods.opts.formData;//$.extend({}, uploader.options, methods.opts);
    }
    var _urlList = [];
    var initUploader = function () {
        uploader = WebUploader.create(methods.opts);
        uploader.on('dndAccept', function (items) {
            var denied = false,
                len = items.length,
                i = 0,
                // 修改js类型
                unAllowed = 'text/plain;application/javascript ';

            for (; i < len; i++) {
                // 如果在列表里面
                if (~unAllowed.indexOf(items[i].type)) {
                    denied = true;
                    break;
                }
            }

            return !denied;
        });

        uploader.on('dialogOpen', function () {
            console.log('here');
        });

        // uploader.on('filesQueued', function() {
        //     uploader.sort(function( a, b ) {
        //         if ( a.name < b.name )
        //           return -1;
        //         if ( a.name > b.name )
        //           return 1;
        //         return 0;
        //     });
        // });

        // 添加“添加文件”的按钮，
        uploader.addButton({
            id: '#filePicker2',
            label: '继续添加'
        });

        uploader.on('ready', function () {
            window.uploader = uploader;
        });

        uploader.onUploadProgress = function (file, percentage) {
            //console.log('onUploadProgress')
            var $li = $('#' + file.id),
                $percent = $li.find('.progress span');

            $percent.css('width', percentage * 100 + '%');
            percentages[file.id][1] = percentage;
            updateTotalProgress();
        };

        uploader.onFileQueued = function (file) {
            //console.log('onFileQueued')

            fileCount++;
            fileSize += file.size;

            if (fileCount === 1) {
                $placeHolder.addClass('element-invisible');
                $statusBar.show();
            }

            //console.log(fileCount)
            //if (this.getFiles().length > this.fileNumLimit) {
            //    alert('error');
            //    return;
            //}


            addFile(file);
            setState('ready');
            updateTotalProgress();

            if (methods.opts.fileQueuedCallBack) {
                methods.opts.fileQueuedCallBack.call(this);
            }
            
        };
        

        uploader.on('uploadSuccess', function (file, data) {
            try {
                var url = data;
                _urlList.push( url)
                //F('hfdTest').setValue(F('hfdTest').getValue() + ',' + url);
            } catch (ex) { console.log(ex)}
            
        })

        uploader.onFileDequeued = function (file) {
            //console.log('onFileDequeued')
            fileCount--;
            fileSize -= file.size;

            if (!fileCount) {
                setState('pedding');
            }

            removeFile(file);
            //console.log('removefile')
            //console.log(file)
            updateTotalProgress();

            //

        };

        uploader.on('all', function (type) {
            var stats;
            switch (type) {
                case 'uploadFinished':
                    setState('confirm');
                    break;

                case 'startUpload':
                    console.log('startUpload')
                    setState('uploading');
                    break;

                case 'stopUpload':
                    setState('paused');
                    break;

            }
        });

        uploader.onError = function (code) {
            console.error(code)
            //alert('Eroor: ' + code);
        };

        $upload.on('click', function () {
            if ($(this).hasClass('disabled')) {
                return false;
            }

            if (state === 'ready') {
                uploader.upload();
            } else if (state === 'paused') {
                uploader.upload();
            } else if (state === 'uploading') {
                uploader.stop();
            }
        });

        $info.on('click', '.retry', function () {
            uploader.retry();
        });

        $info.on('click', '.ignore', function () {
            alert('todo');
        });

        $upload.addClass('state-' + state);
        updateTotalProgress();
        updateStatus();
    }
    // 实例化
    //uploader = WebUploader.create({
    //    pick: {
    //        id: '#filePicker',
    //        label: '点击选择图片'
    //    },
    //    formData: {//TODO 传递参数
    //        owner: 456
    //    },
    //    dnd: '#dndArea',
    //    paste: '#uploader',
    //    swf: '/res/webuploader/Uploader.swf',
    //    chunked: false,
    //    chunkSize: 512 * 1024,
    //    server: '/captcha/FileUploader.ashx',
    //    // runtimeOrder: 'flash',

    //    // accept: {
    //    //     title: 'Images',
    //    //     extensions: 'gif,jpg,jpeg,bmp,png',
    //    //     mimeTypes: 'image/*'
    //    // },

    //    // 禁掉全局的拖拽功能。这样不会出现图片拖进页面的时候，把图片打开。
    //    disableGlobalDnd: true,
    //    fileNumLimit: 10,
    //    fileSizeLimit: 10 * 1024 * 1024,    // 10 M
    //    fileSingleSizeLimit: 1 * 1024 * 1024    // 1 M
    //});

    // 拖拽时不接受 js, txt 文件。
    

    // 当有文件添加进来时执行，负责view的创建
    function addFile(file) {
        var $li = $('<li id="' + file.id + '">' +
                '<p class="title">' + file.name + '</p>' +
                '<p class="imgWrap"></p>' +
                '<p class="progress"><span></span></p>' +
                '</li>'),

            $btns = $('<div class="file-panel">' +
                '<span class="cancel">删除</span>' +
                '<span class="rotateRight">向右旋转</span>' +
                '<span class="rotateLeft">向左旋转</span></div>').appendTo($li),
            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>'),

            showError = function (code) {
                switch (code) {
                    case 'exceed_size':
                        text = '文件大小超出';
                        break;

                    case 'interrupt':
                        text = '上传暂停';
                        break;

                    default:
                        text = '上传失败，请重试';
                        break;
                }

                $info.text(text).appendTo($li);
            };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            // @todo lazyload
            $wrap.text('预览中');
            uploader.makeThumb(file, function (error, src) {
                var img;

                if (error) {
                    $wrap.text('不能预览');
                    return;
                }

                if (isSupportBase64) {
                    img = $('<img src="' + src + '">');
                    $wrap.empty().append(img);
                } else {
                    //TODO change this
                    $.ajax('../../server/preview.php', {
                        method: 'POST',
                        data: src,
                        dataType: 'json'
                    }).done(function (response) {
                        if (response.result) {
                            img = $('<img src="' + response.result + '">');
                            $wrap.empty().append(img);
                        } else {
                            $wrap.text("预览出错");
                        }
                    });
                }
            }, thumbnailWidth, thumbnailHeight);

            percentages[file.id] = [file.size, 0];
            file.rotation = 0;
        }

        file.on('statuschange', function (cur, prev) {
            if (prev === 'progress') {
                $prgress.hide().width(0);
            } else if (prev === 'queued') {
                $li.off('mouseenter mouseleave');
                $btns.remove();
            }

            // 成功
            if (cur === 'error' || cur === 'invalid') {
                console.log(file.statusText);
                showError(file.statusText);
                percentages[file.id][1] = 1;
            } else if (cur === 'interrupt') {
                showError('interrupt');
            } else if (cur === 'queued') {
                $info.remove();
                $prgress.css('display', 'block');
                percentages[file.id][1] = 0;
            } else if (cur === 'progress') {
                $info.remove();
                $prgress.css('display', 'block');
            } else if (cur === 'complete') {
                $prgress.hide().width(0);
                $li.append('<span class="success"></span>');
            }

            $li.removeClass('state-' + prev).addClass('state-' + cur);
        });

        $li.on('mouseenter', function () {
            $btns.stop().animate({ height: 30 });
        });

        $li.on('mouseleave', function () {
            $btns.stop().animate({ height: 0 });
        });

        $btns.on('click', 'span', function () {
            var index = $(this).index(),
                deg;

            switch (index) {
                case 0:
                    uploader.removeFile(file);
                    return;

                case 1:
                    file.rotation += 90;
                    break;

                case 2:
                    file.rotation -= 90;
                    break;
            }

            if (supportTransition) {
                deg = 'rotate(' + file.rotation + 'deg)';
                $wrap.css({
                    '-webkit-transform': deg,
                    '-mos-transform': deg,
                    '-o-transform': deg,
                    'transform': deg
                });
            } else {
                $wrap.css('filter', 'progid:DXImageTransform.Microsoft.BasicImage(rotation=' + (~~((file.rotation / 90) % 4 + 4) % 4) + ')');
                // use jquery animate to rotation
                // $({
                //     rotation: rotation
                // }).animate({
                //     rotation: file.rotation
                // }, {
                //     easing: 'linear',
                //     step: function( now ) {
                //         now = now * Math.PI / 180;

                //         var cos = Math.cos( now ),
                //             sin = Math.sin( now );

                //         $wrap.css( 'filter', "progid:DXImageTransform.Microsoft.Matrix(M11=" + cos + ",M12=" + (-sin) + ",M21=" + sin + ",M22=" + cos + ",SizingMethod='auto expand')");
                //     }
                // });
            }


        });

        $li.appendTo($queue);
    }

    // 负责view的销毁
    function removeFile(file) {
        var $li = $('#' + file.id);

        delete percentages[file.id];
        updateTotalProgress();
        $li.off().find('.file-panel').off().end().remove();
    }

    function updateTotalProgress() {
        var loaded = 0,
            total = 0,
            spans = $progress.children(),
            percent;

        $.each(percentages, function (k, v) {
            total += v[0];
            loaded += v[0] * v[1];
        });

        percent = total ? loaded / total : 0;


        spans.eq(0).text(Math.round(percent * 100) + '%');
        spans.eq(1).css('width', Math.round(percent * 100) + '%');
        updateStatus();
    }

    function updateStatus() {
        var text = '', stats;

        if (state === 'ready') {
            text = '选中' + fileCount + '张图片，共' +
                    WebUploader.formatSize(fileSize) + '。';
        } else if (state === 'confirm') {
            stats = uploader.getStats();
            if (stats.uploadFailNum) {
                text = '已成功上传' + stats.successNum + '张照片至XX相册，' +
                    stats.uploadFailNum + '张照片上传失败，<a class="retry" href="#">重新上传</a>失败图片或<a class="ignore" href="#">忽略</a>'
            }

        } else {
            stats = uploader.getStats();
            //text = '共' + fileCount + '张（' +
            //        WebUploader.formatSize(fileSize) +
            //        '），已上传' + stats.successNum + '张';
            text = '上传成功';
            if (stats.uploadFailNum) {
                text += '，失败' + stats.uploadFailNum + '张';
            }
        }

        $info.html(text);
    }

    function setState(val) {
        var file, stats;

        if (val === state) {
            return;
        }

        $upload.removeClass('state-' + state);
        $upload.addClass('state-' + val);
        state = val;

        switch (state) {
            case 'pedding':
                $placeHolder.removeClass('element-invisible');
                $queue.hide();
                $statusBar.addClass('element-invisible');
                uploader.refresh();
                break;

            case 'ready':
                $placeHolder.addClass('element-invisible');
                $('#filePicker2').removeClass('element-invisible');
                $queue.show();
                $statusBar.removeClass('element-invisible');
                uploader.refresh();
                break;

            case 'uploading':
                $('#filePicker2').addClass('element-invisible');
                $progress.show();
                $upload.text('暂停上传');
                break;

            case 'paused':
                $progress.show();
                $upload.text('继续上传');
                break;

            case 'confirm':
                $progress.hide();
                $('#filePicker2').removeClass('element-invisible');
                $upload.text('开始上传');

                stats = uploader.getStats();
                if (stats.successNum && !stats.uploadFailNum) {
                    setState('finish');
                    return;
                }
                break;
            case 'finish':
                stats = uploader.getStats();
                if (stats.successNum) {

                    F.util.alert('上传成功');
                } else {
                    // 没有成功的图片，重设
                    state = 'done';
                    location.reload();
                }
                break;
        }

        //updateStatus();
    }

    

    var methods = {
        defaults: {
            pick: {
                id: '#filePicker',
                label: '点击选择图片'
            },
            formData: {// 传递参数
                owner: false
            },
            dnd: '#dndArea',
            paste: '#uploader',
            swf: '/res/webuploader/Uploader.swf',
            chunked: false,
            chunkSize: 512 * 1024,
            server: '/captcha/FileUploader.ashx',
            // runtimeOrder: 'flash',

            // accept: {
            //     title: 'Images',
            //     extensions: 'gif,jpg,jpeg,bmp,png',
            //     mimeTypes: 'image/*'
            // },

            // 禁掉全局的拖拽功能。这样不会出现图片拖进页面的时候，把图片打开。
            disableGlobalDnd: true,
            fileNumLimit: 8,
            fileSizeLimit: 8 * 1024 * 1024,    // 10 M
            fileSingleSizeLimit: 1 * 1024 * 1024    // 1 M
        },
        opts: {},
        // 在字面量对象中定义每个单独的方法
        init: function (arg) {
            var options = arg[0]
            

            // 为了更好的灵活性，对来自主函数，并进入每个方法中的选择器其中的每个单独的元素都执行代码
            return this.each(function () {
                //console.log(methods.opts.isInit)
                if (methods.opts.isInit) {
                    return;
                }
                methods.opts = $.extend({}, methods.defaults, options);
                
                methods.opts.isInit = true;
                // 为每个独立的元素创建一个jQuery对象
                methods.opts.html = $(this).html();
                // 执行代码
                initUploader();
                // 例如： privateFunction();
            });
        },
        data: function (args) {
            return _urlList;
        },
        rebind:function(args){
            var options = args[1];
            methods.opts.formData = options.formData;
            //methods.opts = $.extend({}, methods.opts, options);
            return this.each(function () {
                var $this = $(this)
                rebindUploader($this);
            })
        },
        destroy: function () {

            // 对选择器每个元素都执行方法
            return this.each(function () {
                var $this = $(this);
                resetUploader($this);
                //$this.html('');
                // 执行代码
            });
        },
        //公有方法
        render: function (arg) {
            return this.each(function () {
                //console.log(methods.opts)
                var params = arg[1];
                // var text = params['text'];
                //methods.opts = $.extend({}, methods.defaults, params);
                methods.opts = $.extend({}, methods.opts, params);
                //console.log(opts)
                var $this = $(this);
               // getData($this);
            });
        }
    };

    $.fn.myUpload = function () {
        var method = arguments[0];
        if (methods[method]) {
            // 如果方法存在，存储起来以便使用
            // 注意：我这样做是为了等下更方便地使用each（）
            method = methods[method];
            // 如果方法不存在，检验对象是否为一个对象（JSON对象）或者method方法没有被传入
        } else if (typeof (method) == 'object' || !method) {
            // 如果我们传入的是一个对象参数，或者根本没有参数，init方法会被调用
            method = methods.init;
        } else {
            // 如果方法不存在或者参数没传入，则报出错误。需要调用的方法没有被正确调用
            $.error('Method ' + method + ' does not exist on jQuery.myMenu');
            return this;
        }
        return method.call(this, arguments);
    }
})(jQuery);