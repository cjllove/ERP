/*
@Function FineUIPro grid 行disable
@Requirements jQuery / FineUIPro / 需要在PageManager中打开EnableAjax !
@Auther 鞠怡
@params
    options:
    {
        grid: grid id
        hfdDisabled: 存放需要disable行号的隐藏域id,该隐藏域中的行号用","隔开
    }
*/
$.disablegrid = function (options) {
    var defualts = {
        grid: false,
        hfdDisabled:false
    };

    var opts = $.extend({}, defualts, options);
    F.ready(function () {
        F(opts.grid).getSelectionModel().on('beforeselect', beforeSelect);
        F.util.ajaxReady(setDisableClass);
        //F(GridCFGSupplier).getSelectionModel().on('beforeselect', beforeSelectYiyuan);
    });

    function setDisableClass() {
        var disabledArray = getDisabledArray();
        $('#' + opts.grid).find(F(opts.grid).getView().itemSelector).each(function (i) {
            $(this).removeClass('ui-state-disabled');
        });
        for (var i = 0; i < disabledArray.length; i++) {
            if (disabledArray[i] != null && typeof (disabledArray[i]) != null && disabledArray[i].length > 0) {
                Ext.get(F(opts.grid).getView().getNode(parseInt(disabledArray[i]))).addCls('ui-state-disabled');
            }
        }

    }

    function beforeSelect(a, b, c) {
        var disabledStr = getDisabledStr();
        if (disabledStr != null && typeof (disabledStr) != 'undefined') {
            var disabledArray = getDisabledArray();
            for (var i = 0; i < disabledArray.length; i++) {
                if (c.toString() == disabledArray[i]) {
                    return false;
                }
            }
        }
    }
    function getDisabledStr() {
        var disabledStr = F(opts.hfdDisabled).value;
        return disabledStr;
    }

    function getDisabledArray() {
        var disabledStr = getDisabledStr();
        if (disabledStr != null && typeof (disabledStr) != 'undefined') {
            var disabledArray = disabledStr.split(',');
            return disabledArray;
        } else {
            return [];
        }
    }
};