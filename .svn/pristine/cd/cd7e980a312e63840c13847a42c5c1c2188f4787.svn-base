
/*
@Function 键盘导航
@Requirements jQuery / tooltipster
@Auther 鞠怡
@params
    options:
    {
        condition  //控制提示信息是否显示的逻辑，字符串  默认'true' , eval(condition) = true / false, 

        ,map //按钮和快捷键的map关系,例 map = [{dom:'#button1',key:'N',position:'top'}]; map.position可自定义某按钮提示位置

        ,theme //主题，默认'tooltipster-shadow', 四种可选，需要包含res/css/theme下对应的主题样式

        ,position //全部按钮弹出提示位置，字符串，默认'bottom'，可为 top/bottom/left/right，优先级比map.position低
    }
*/
$.nav = function (options) {
    var defualts = {
        condition: "true"
        , map: []
        , theme: 'tooltipster-shadow'
        , position: 'bottom'
    };

    var opts = $.extend({}, defualts, options);


    _pressing = false;

    for (var i = 0; i < opts.map.length; i++) {
        bindTooltipster(opts, i);
    }

    if (parent.document != document) {
        $(parent.document).on('keydown', function (e) {
            keydownCallBack(e);
        });

        $(parent.document).on('keyup', function (e) {
            keyupCallBack(e);
        });
    }
    

    $(document).on('keydown', function (e) {
        keydownCallBack(e);
    });

    

    $(document).on('keyup', function (e) {
        keyupCallBack(e);
    });

    function keydownCallBack(e) {
        for (var i = 0; i < opts.map.length; i++) {
            myKeydown(e, opts, i);
        }
        if (e.altKey && !_pressing) {
            e.preventDefault();
            _pressing = true;
        }
    }

    function keyupCallBack(e) {
        for (var i = 0; i < opts.map.length; i++) {
            myKeyup(e, opts, i);
        }
        if (e.which == 18) {
            e.preventDefault();
            _pressing = false;
        }
    }

    function bindTooltipster(options, index) {
        var obj = $(options.map[index].dom);
        obj.tooltipster({ position: options.map[index].position || options.position, theme: options.theme });
        obj.tooltipster("content", options.map[index].key);
        obj.tooltipster('enable');
    }


    function myKeydown(e, options, index) {
        var obj = $(options.map[index].dom);
        if (eval(options.condition) && obj.css('display')!='none') {
            if (e.altKey && e.which == options.map[index].key.charCodeAt()) { //
                e.preventDefault();
                obj.children().eq(0).click();
            }
            if (e.altKey && !_pressing) {
               
                obj.tooltipster("show");
            }
        }
    }

    function myKeyup(e, options, index) {
        var obj = $(options.map[index].dom);
        if (e.which == 18) { //alt
            e.preventDefault();
            try{
                obj.tooltipster('hide');
            }catch(ex){
                        
            }
        }
    }
};

/*
gird 导航
*/
(function ($) {
    $.fn.extend({
        nav: function (options, callback) {
            return this.each(function () {
                var opts = $.extend({}, defualts, options);
                var defualts = {
                    updownCallBack: defaultUpdownCallBack
                    , leftrightCallBack: defaultLeftrightCallBack
                    , enterCallBack: false
                    , condition: 'true'
                    , colMap:[]
                };
                var opts = $.extend({}, defualts, options);
                var obj = $(this);
                _pressing = false;

                //if (window.parent.document != document) {
                //    $(window.parent.document).on('keydown', function (e) {
                //        keydownCallBack(e);
                //    });
                //    $(window.parent.document).on('keyup', function (e) {
                //        keyupCallBack(e);
                //    });
                //}
               
                
                $(document).on('keydown', function (e) {
                    keydownCallBack(e);
                });
                $(document).on('keyup', function (e) {
                    keyupCallBack(e);
                });

                function defaultUpdownCallBack(e) {
                    var id = 'id';
                    try {
                        var lastSelected = F(obj.attr(id)).getSelectionModel().getLastSelected();
                        if (lastSelected) {
                            var index = F(obj.attr(id)).getStore().indexOf(lastSelected);
                            try {
                                if (e.which == 40) {
                                    F(obj.attr(id)).getSelectionModel().select(index + 1);
                                } else if (e.which == 38) {
                                    F(obj.attr(id)).getSelectionModel().select(index - 1);
                                }
                            } catch (ex) { }
                        } else {
                            F(obj.attr(id)).getSelectionModel().select(0);
                        }
                    } catch (ex) {}
                }

                function defaultLeftrightCallBack(e) {
                    if (e.which == 39) {//左方向键 + alt

                        navigate(obj.attr('id'), false, opts.colMap);
                    }
                    if (e.which == 37) {//左方向键 + alt\

                        navigate(obj.attr('id'), true, opts.colMap);
                    }
                }

                function navigate(griddom, minus, colMap) {
                    if (colMap == null || typeof (colMap) == 'undefined') {
                        return false;
                    } else if (colMap.length == 0) {
                        return false;
                    }
                    function selectCells(griddom, colMap, rowIndex, initCell, minus) {
                        if (!minus) {
                            for (var j = 0; j < colMap.length; j++) {
                                //if (initCell !=0 && F(griddom).columns[initCell].id == colMap[colMap.length - 1]) {
                                //    selectCells(griddom, colMap, rowIndex+1 , 0);
                                //} else {
                                for (var i = initCell + 1; i < F(griddom).columns.length; i++) {
                                    if (F(griddom).columns[i].id == colMap[j]) {

                                        $('#' + griddom).find(F(griddom).getView().itemSelector).eq(rowIndex).find('td' + '.x-grid-cell-' + F(griddom).columns[i].id).click();
                                        return true;

                                    }
                                }
                                //}

                            }
                        } else {
                            for (var j = colMap.length - 1; j >= 0; j--) {
                                //if (initCell !=0 && F(griddom).columns[initCell].id == colMap[colMap.length - 1]) {
                                //    selectCells(griddom, colMap, rowIndex+1 , 0);
                                //} else {
                                for (var i = initCell - 1; i >=0; i--) {
                                    if (F(griddom).columns[i].id == colMap[j]) {

                                        $('#' + griddom).find(F(griddom).getView().itemSelector).eq(rowIndex).find('td' + '.x-grid-cell-' + F(griddom).columns[i].id).click();
                                        return true;

                                    }
                                }
                                //}

                            }
                        }

                    }
                    var initCell = F(griddom).f_getSelectedCell()[1] || 0;
                    var lastSelected = F(griddom).f_getSelectedCell()[0] || 0;//F(griddom).getStore().indexOf(F(griddom).getSelectionModel().getLastSelected());
                    if (lastSelected != null && typeof (lastSelected) != 'undefined' && lastSelected != -1) {
                        rowIndex = lastSelected;
                        if (minus) {
                            if (F(griddom).columns[initCell].id == colMap[0]) {
                                if (rowIndex > 0) {
                                    rowIndex = rowIndex - 1;
                                    initCell = F(griddom).columns.length;
                                } else {
                                    rowIndex = $('#' + griddom).find(F(griddom).getView().itemSelector).length - 1;
                                    initCell = F(griddom).columns.length;
                                }
                            }
                        } else if (rowIndex >= 0 && rowIndex < $('#' + griddom).find(F(griddom).getView().itemSelector).length - 1 && initCell != 0 && F(griddom).columns[initCell].id == colMap[colMap.length - 1]) {
                            rowIndex = rowIndex + 1;
                            initCell = 0;
                        } else if (rowIndex == $('#' + griddom).find(F(griddom).getView().itemSelector).length - 1 && F(griddom).columns[initCell].id == colMap[colMap.length - 1]) {
                            rowIndex = 0;
                            initCell = 0;
                        }
                    } else {
                        rowIndex = 0;
                    }
                    try {
                        selectCells(griddom, colMap, rowIndex, initCell, minus);
                        //$('#' + griddom).find(F(griddom).getView().itemSelector).eq(rowIndex).find('td' + '.x-grid-cell-BZSL').click();
                    } catch (ex) {
                        return false;
                    }
                }

                function keydownCallBack(e) {
                    if (eval(opts.condition)) {
                        if (e.altKey && (e.which == 38 || e.which == 40)) { //38 up ,40 down
                            e.preventDefault();
                            if (opts.updownCallBack) {
                                opts.updownCallBack(e);
                            }
                        }
                        if (e.altKey && (e.which == 37 || e.which == 39)) { //37 left, 39 right
                            e.preventDefault();
                            if (opts.leftrightCallBack) {
                                opts.leftrightCallBack(e);
                            }
                        }
                        if (e.altKey && e.which == 13) { //13 enter
                            e.preventDefault();
                            if (opts.enterCallBack) {
                                opts.enterCallBack(e);
                            }
                        }
                        if (e.altKey && !_pressing) {
                            e.preventDefault();
                            _pressing = true;
                        }
                    }
                }

                function keyupCallBack(e) {
                    if (e.which == 18 ) { //alt
                        e.preventDefault();
                        _pressing = false;
                    }
                }

                if (callback) {
                    callback();
                }
            });
        }
    });
})(jQuery);