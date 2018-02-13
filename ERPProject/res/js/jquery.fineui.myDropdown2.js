// target                         必填参数，triggerbox的ClientID
// callBackColName       选填参数 ， 默认值"NAME" ,名称字段名
// callBackColValue        选填参数， 默认值"CODE" ,值字段名
// page                           选填参数，默认值空（当前页），数据服务页面
// height                        选填参数，默认值300, 弹出高度
// width                          选填参数，默认值220, 弹出宽度
// colWidth                    选填参数，默认值200, Grid列宽度
$.myDropdown2 = function (options) {
    var defualts = {
        target: false, //必填参数，目标下拉框(triggerBox)ID
        callBackColName: "NAME", //可选参数，名称列ID
        callBackColValue: "CODE", //可选参数，值列ID
        page: '', //可选参数，用于取数据的回发页面
        height: 300,
        width: 220,
        colWidth:200
    };
    var opts = $.extend({}, defualts, options);
    var $target = $('#' + opts.target);
    //var $grid = $('#' + opts.grid);
    var _grid = null;
    var _idFieldName = null;
    init();
    function init() {
        var idFieldName = opts.target + "_id";
        _idFieldName = idFieldName;
        var gridName = opts.target + "_grid";
        $(document.body).find('form').eq(0).append('<div id="' + idFieldName + '_wrapper" class="f-inline-block"></div>');
        $(document.body).find('form').eq(0).append('<div id="' + gridName + '_wrapper"><div id="' + gridName + '_tpls" class="f-grid-tpls f-hidden"></div></div>');
        var f2 = new F.TextBox({
            f_state: {},
            id: idFieldName,
            name: idFieldName,
            renderTo: '#' + idFieldName + '_wrapper',
            hidden: true
        });

        var f4 = new F.GridColumn({
            hidden: true,
            f_state: {},
            id: gridName + '_ctl01',
            text: '',
            field: opts.callBackColValue,
            columnId: opts.callBackColValue,
            index: 0,
            width: opts.colWidth
        });
        var f5 = new F.GridColumn({
            f_state: {},
            id: gridName+'_ctl00',
            text: '',
            field: opts.callBackColName,
            columnId: opts.callBackColName,
            index: 1,
            width: opts.colWidth
        });

        var f3_columns = [f4, f5];
        var f3_fields = [opts.callBackColValue,opts.callBackColName];
        var f3 = new F.Grid({
            //f_state: f3_state,
            dataUrl: opts.page+'?querymydd=1',
            id: gridName,
            name: gridName,
            renderTo: '#' + gridName + '_wrapper',
            width: opts.width,
            height: opts.height,
            autoScroll: true,
            header: false,
            showSelectedCell: false,
            multiSelect: true,
            columns: f3_columns,
            fields: f3_fields,
            //data: f3_state.F_Rows,
            gridHeader: false
        });
        f3.hide();
        f3.draggable = false;
        var $grid = $('#'+gridName);
        
        //if (opts.grid && F(opts.grid)) {
        //    F(opts.grid).hide();
        //}
        //F(opts.grid).draggable = false;

        $grid.css('top', $target.find('input').offset().top + $target.height());
        $grid.css('left', $target.find('input').offset().left);
        $grid.css('position', 'fixed');
        $grid.css('z-index', '9999');

        f3.on('rowclick', itemMouseDownEvent);

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
        _grid = f3;
    }

    //上下键选择grid内容
    function selectNext() {

        if (!_grid.getSelectedRow()) {
            _grid.selectRow(0);
        } else {
            //console.log(F(opts.grid).getSelectedRow(true))  
            _grid.selectRow(_grid.getSelectedRow(true).index + 1);
        }
    }

    function selectPrev() {
        if (!_grid.getSelectedRow()) {
            _grid.selectRow(_grid.getEndRowIndex())
        } else {
            //console.log(F(opts.grid).getSelectedRow(true))
            _grid.selectRow(_grid.getSelectedRow(true).index - 1);
        }
    }

    function itemMouseDownEvent(event, rowId, rowIndex) {
        var tName = _grid.getCellData(rowId, opts.callBackColName).value;
        var tValue = _grid.getCellData(rowId, opts.callBackColValue).value;

        $target.find("input").val(tName);
        $('#' + _grid.id).hide();
        if (_idFieldName) {
            F(_idFieldName).setValue(tValue);
        }
        event.stopPropagation();
    }

    //回车选择Grid选中行
    function enterSelectEvent(e) {
        e.stopPropagation();
        if (_grid.getSelectedRow()) {
            itemMouseDownEvent(e, _grid.getSelectedRow(), _grid.getSelectedRow(true).index)
        } else {
            enterEvent(e);
        }
    }



    function enterEvent(e) {
        e.stopPropagation();
        //e.preventDefault();
        setTimeout(function () {
            _grid.loadDataUrl(getUrl(opts.page));
        }, 0)
        
        $('#' + _grid.id).show();
        bindDoc();
    }

    //点击页面关闭grid
    function bindDoc() {
        $grid = $('#' + _grid.id);
        $(document).unbind('click');
        $(document).on('click', function (e) {
            if (
                $grid.find($($(e.currentTarget).context.activeElement)).length > 0
                ||
                $target.find($($(e.currentTarget).context.activeElement)).length > 0
                ) {
                return;
            }
            _grid.hide();

            $(document).unbind('click');
        });
    }

    function getUrl(page) {
        var sp = "?";
        if (page.indexOf("?") > -1) {
            sp = "&";
        }
        return page + sp + "querymydd=1&text=" + F(opts.target).getValue()+"&target="+opts.target;
    }

    function clickEvent(e) {

        //if (opts.customEvent) {
        //    F.customEvent(opts.customEvent);
        //}
        setTimeout(function () {
            _grid.loadDataUrl(getUrl(opts.page));
        },0)
        
        $('#' + _grid.id).toggle();
        e.stopPropagation();
        bindDoc();
    }
};