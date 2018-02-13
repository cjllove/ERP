/*
购物车——数字输入框组件js
@Author 鞠怡
@Date 2015/07/29
*/

(function ($) {
    //私有方法
    //var privateFunction = function($this){}



    var changeSize = function ($this) {
        if ($this.parent().find('.input-btn').length > 0) {
            return;
        }

        if ($this.outerWidth() - methods.opts.height * 2 > 0) {
            $this.width($this.outerWidth() - methods.opts.height * 2);
        }
        
        //$this.height(methods.opts.height);
    }
    var attachButton = function ($this) {
        if($this.parent().find('.input-btn').length>0){
            return;
        }
        var btnMinus = $('<span class="input-btn">-</span>');
        var btnAdd = $('<span class="input-btn">+</span>');

        btnMinus
		.css('border-right', 'none')
		.width(methods.opts.height ).height(methods.opts.height-2 )
		.css('line-height', methods.opts.height + 'px')
        .css('box-sizing', 'content-box')

        $this.css('float', 'left');

        btnAdd
		.css('border-left', 'none')
		.width(methods.opts.height ).height(methods.opts.height -2)
		.css('line-height', methods.opts.height + 'px')
        .css('box-sizing', 'content-box')
        btnMinus.on('selectstart', function () { return false });
        btnAdd.on('selectstart', function () { return false });
        var max = methods.opts.max;
        var min = methods.opts.min;
        btnMinus.on('click', function (e) {
            e.preventDefault();
            if ($this.val() != '' && $this.val() != null && (typeof (min) != 'undefined' && parseFloat($this.val()-1) > min)) {
                //console.log($this.val())
                //console.log(parseFloat($this.val()))
                //console.log(parseFloat($this.val()) - 1);
                //console.log(1.4-1)

                $this.val(parseFloat($this.val()).sub(1));
                if (methods.opts.callBack) {
                    methods.opts.callBack.call($this);
                }
                //if (upCart)
                //{ upCart($this.context.id, $this.val()) }
            }
        });

        btnAdd.on('click', function (e) {
            e.preventDefault();
            if ($this.val() != '' && $this.val() != null && (max == 0 || (max && parseFloat($this.val()) < max))) {
                $this.val(parseFloat($this.val()) .add(1));
                if (methods.opts.callBack) {
                    methods.opts.callBack.call($this);
                }
                //if (upCart)
                //{ upCart($this.context.id, $this.val()) }
            }
        });
        //btnAdd.css('width',methods.opts.height+'px').css('height',methods.opts.height+'px');
        // btnMinus.height(methods.opts.height);
        // btnAdd.width(methods.opts.height);
        // btnAdd.height(methods.opts.height);
        $this.before(btnMinus).after(btnAdd);

        $this.on('change', function (e) {
            var value = $(e.currentTarget).val();
            try {
                //attr('data-isdecimal')  : 是否允许小数 Y/N
                if ($(e.currentTarget).attr('data-isdecimal') && $(e.currentTarget).attr('data-isdecimal') == "N") {
                    if (value.indexOf(".") > -1 || value.indexOf("。") > -1) {
                        value = value.replace("。", ".");
                        value = parseInt(value);
                        $(e.currentTarget).val(value);
                        throw new Error("不允许输入小数");
                    }
                }
                value = value.replace("。", ".");
                $(e.currentTarget).val(value);
                //console.log(!isNaN(value))
                if (isNaN(value)) {
                    $(e.currentTarget).val(methods.opts.min);
                    throw new Error(value + "为非法数字!");
                }
            } catch (ex) {

                alert(ex.message);
                if (e.stopPropagation) { //W3C阻止冒泡方法  
                    e.stopPropagation();
                } else {
                    e.cancelBubble = true; //IE阻止冒泡方法  
                }
            } finally {
                if (methods.opts.callBack) {
                    methods.opts.callBack.call($this);
                }
            }
        });
    }
    var bindEvents = function ($this) {
        if (!$this.val()) {
            $this.val(methods.opts.defaultVal);
        }

        var max = methods.opts.max;
        var min = methods.opts.min;
        $this.on('input', function (e) {
            var curValue = $(e.currentTarget).val();
            if (curValue > max) {
                $(e.currentTarget).val(max);
            }
            if (curValue < min) {
                $(e.currentTarget).val(min);
            }
        })

    }

    /**
 ** 减法函数，用来得到精确的减法结果
 ** 说明：javascript的减法结果会有误差，在两个浮点数相减的时候会比较明显。这个函数返回较为精确的减法结果。
 ** 调用：accSub(arg1,arg2)
 ** 返回值：arg1加上arg2的精确结果
 **/
    function accSub(arg1, arg2) {
        var r1, r2, m, n;
        try {
            r1 = arg1.toString().split(".")[1].length;
        }
        catch (e) {
            r1 = 0;
        }
        try {
            r2 = arg2.toString().split(".")[1].length;
        }
        catch (e) {
            r2 = 0;
        }
        m = Math.pow(10, Math.max(r1, r2)); //last modify by deeka //动态控制精度长度
        n = (r1 >= r2) ? r1 : r2;
        return ((arg1 * m - arg2 * m) / m).toFixed(n);
    }

    // 给Number类型增加一个mul方法，调用起来更加方便。
    Number.prototype.sub = function (arg) {
        return accSub(this,arg );
    };

    /**
 ** 加法函数，用来得到精确的加法结果
 ** 说明：javascript的加法结果会有误差，在两个浮点数相加的时候会比较明显。这个函数返回较为精确的加法结果。
 ** 调用：accAdd(arg1,arg2)
 ** 返回值：arg1加上arg2的精确结果
 **/
    function accAdd(arg1, arg2) {
        var r1, r2, m, c;
        try {
            r1 = arg1.toString().split(".")[1].length;
        }
        catch (e) {
            r1 = 0;
        }
        try {
            r2 = arg2.toString().split(".")[1].length;
        }
        catch (e) {
            r2 = 0;
        }
        c = Math.abs(r1 - r2);
        m = Math.pow(10, Math.max(r1, r2));
        if (c > 0) {
            var cm = Math.pow(10, c);
            if (r1 > r2) {
                arg1 = Number(arg1.toString().replace(".", ""));
                arg2 = Number(arg2.toString().replace(".", "")) * cm;
            } else {
                arg1 = Number(arg1.toString().replace(".", "")) * cm;
                arg2 = Number(arg2.toString().replace(".", ""));
            }
        } else {
            arg1 = Number(arg1.toString().replace(".", ""));
            arg2 = Number(arg2.toString().replace(".", ""));
        }
        return (arg1 + arg2) / m;
    }

    //给Number类型增加一个add方法，调用起来更加方便。
    Number.prototype.add = function (arg) {
        return accAdd(arg, this);
    };
    var methods = {
        defaults: {
            min: 1,
            max: 10,   //max为0则不约束
            defaultVal: 1
        },
        opts: {},
        init: function (arg) {
            var options = arg[0]

            return this.each(function () {
                methods.opts = $.extend({}, methods.defaults, options);
                var $this = $(this);
                if (methods.opts.width == methods.defaults.width) {
                    methods.opts.width = $this.outerWidth();
                }

                if (methods.opts.height == methods.defaults.height) {
                    methods.opts.height = $this.outerHeight();
                }
                changeSize($this);
                attachButton($this);
                bindEvents($this);
                //privateFunction($this);
            });
        },
        //公共方法
        destroy: function () {

            return this.each(function () {

            });
        },
    }
    $.fn.gwNumberBox = function () {
        var method = arguments[0];
        if (methods[method]) {
            method = methods[method];
        } else if (typeof (method) == 'object' || !method) {
            method = methods.init;
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.gwSelect');
            return this;
        }
        return method.call(this, arguments);
    }

})(jQuery)