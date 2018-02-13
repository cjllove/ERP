$.myDropdown = function (options) {
    var defualts = {
        target: false,
        grid: false,
        customEvent: false,
        callBackColName: false,
        callBackColValue: false,
        target_hidden: false,
        offset_position: false

    };
    var opts = $.extend({}, defualts, options);
    var $target = $('#' + opts.target);
    var $grid = $('#' + opts.grid);

    init();
    function init() {
        if (opts.grid && F(opts.grid)) {
            F(opts.grid).hide();
        }
        F(opts.grid).draggable = false;
        if (opts.offset_position) {
            $grid.css('top', $target.find('input').offset().top + $target.height());
            $grid.css('left', $target.find('input').offset().left);
        } else {
            $grid.css('top', $target.find('input').offset().top + $target.height());
            $grid.css('left', $target.find('input').offset().left);
        }

        $grid.css('position', 'fixed');
        $grid.css('z-index', '9999');

        F(opts.grid).on('rowclick', itemMouseDownEvent);

        $target.find('.f-field-triggerbox-icons').eq(0).unbind('click');
        $target.find('.f-field-triggerbox-icons').eq(0).on('click', clickEvent);

        $target.find('input').eq(0).unbind('keydown');
        $target.find('input').eq(0).on('keydown', function (e) {
            if (e.which == 13) {
                e.preventDefault();
                enterSelectEvent(e);
            } else if (e.which == 40) { //向下
                e.preventDefault();
                selectNext();
            } else if (e.which == 38) { //向上
                e.preventDefault();
                selectPrev();
            }
            else {
                enterEvent(e);
            }
        });
    }

    //上下键选择grid内容
    function selectNext() {

        if (!F(opts.grid).getSelectedRow()) {
            F(opts.grid).selectRow(0);
        } else {
            //console.log(F(opts.grid).getSelectedRow(true))  
            F(opts.grid).selectRow(F(opts.grid).getSelectedRow(true).index + 1);
        }
    }

    function selectPrev() {
        if (!F(opts.grid).getSelectedRow()) {
            F(opts.grid).selectRow(F(opts.grid).getEndRowIndex())
        } else {
            //console.log(F(opts.grid).getSelectedRow(true))
            F(opts.grid).selectRow(F(opts.grid).getSelectedRow(true).index - 1);
        }
    }

    function itemMouseDownEvent(event, rowId, rowIndex) {
        var tName = F(opts.grid).getCellData(rowId, opts.callBackColName).value;
        var tValue = F(opts.grid).getCellData(rowId, opts.callBackColValue).value;

        $target.find("input").val(tName);
        $('#' + opts.grid).hide();
        if (opts.target_hidden) {
            F(opts.target_hidden).setValue(tValue);
        }
        event.stopPropagation();
    }

    //回车选择Grid选中行
    function enterSelectEvent(e) {
        e.stopPropagation();
        if (F(opts.grid).getSelectedRow()) {
            itemMouseDownEvent(e, F(opts.grid).getSelectedRow(), F(opts.grid).getSelectedRow(true).index)
        } else {
            enterEvent(e);
        }
    }



    function enterEvent(e) {
        e.stopPropagation();
        //e.preventDefault();

        if (opts.customEvent) {
            F.customEvent(opts.customEvent);
        }
        $('#' + opts.grid).show();
        bindDoc();
    }

    //点击页面关闭grid
    function bindDoc() {
        $(document).unbind('click');
        $(document).on('click', function (e) {
            if (
                $grid.find($($(e.currentTarget).context.activeElement)).length > 0
                ||
                $target.find($($(e.currentTarget).context.activeElement)).length > 0
                ) {
                return;
            }
            F(opts.grid).hide();

            $(document).unbind('click');
        });
    }

    function clickEvent(e) {

        if (opts.customEvent) {
            F.customEvent(opts.customEvent);
        }
        $('#' + opts.grid).toggle();
        e.stopPropagation();
        bindDoc();
    }
};