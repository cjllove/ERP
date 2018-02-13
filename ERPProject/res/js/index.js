function notify(msg) {
    F.notify({
        message: msg,
        messageIcon: 'information',
        target: '_top',
        header: false,
        displayMilliseconds: 3 * 1000,
        positionX: 'center',
        positionY: 'center'
    });
}

// 点击菜单样式
function onMenuStyleCheckChange(event, item, checked) {
    var menuStyle = item.getAttr('data-tag');

    F.cookie('MenuStyle_Pro', menuStyle, {
        expires: 100 // 单位：天
    });
    top.window.location.reload();
}

// 点击语言
function onMenuLangCheckChange(event, item, checked) {
    var lang = item.getAttr('data-tag');

    F.cookie('Language_Pro', lang, {
        expires: 100 // 单位：天
    });
    top.window.location.reload();
}


function refreshTab(tabId) {
    var mainTabStrip = F(mainTabStripClientID);
    var targetTab = mainTabStrip.getTab(tabId);

    if (targetTab) {
        targetTab.refreshIFrame();
    }
}

F.ready(function () {
    if (browser && browser.mobile) {
        location.href = "/pad/index.aspx";
    }
    var menuLis = $('.mymenu');
    menuLis.click(function (e) {
        var cnode = $(this);
        menuLis.removeClass('ui-state-active');
        cnode.addClass('ui-state-active');
        //alert(cnode.attr("id").substring(42));
        F.customEvent('TreeInit_' + cnode.attr("id").substring(42));
    });

    var mainTabStrip = F(mainTabStripClientID);
    var mainMenu = F(leftPanelClientID);

    //创建选项卡前的回调函数:根据数据信息设置页面自动全屏
    function addTabCallback(tabConfig) {
        var topPanel = F(topPanelClientID);
        var leftPanel = F(leftPanelClientID);
        var currentTool = F('RegionPanel_mainTabStrip_toolMaximize');
        var str = new Array();
        str = tabConfig.id.split("_");
        if (str[3] == "N") {
            topPanel.collapse();
            leftPanel.collapse();
            currentTool.setIconFont('compress');
        }
        if (str[4] == "N") {
            var str = location.href; //取得整个地址栏
            var num = str.indexOf("#")
            str = str.substr(1, num); //取得所有参数   stringvar.substr(start [, length ]
            //在另外新建窗口中打开窗口
            window.open(tabConfig.iframeUrl + str);
        }
    }

    // 初始化主框架中的树(或者Accordion+Tree)和选项卡互动，以及地址栏的更新
    // treeMenu： 主框架中的树控件实例，或者内嵌树控件的手风琴控件实例
    // mainTabStrip： 选项卡实例
    // createToolbar： 创建选项卡前的回调函数（接受tabConfig参数）
    // updateLocationHash: 切换Tab时，是否更新地址栏Hash值
    // refreshWhenExist： 添加选项卡时，如果选项卡已经存在，是否刷新内部IFrame
    // refreshWhenTabChange: 切换选项卡时，是否刷新内部IFrame
    F.initTreeTabStrip(mainMenu, mainTabStrip, addTabCallback, true, false, false);
});

// 点击标题栏工具图标 - 刷新
function onToolRefreshClick(event) {
    var mainTabStrip = F(mainTabStripClientID);

    var activeTab = mainTabStrip.getActiveTab();
    if (activeTab.iframe) {
        var iframeWnd = activeTab.getIFrameWindow();
        iframeWnd.location.reload();
    }
}

function onToolMaximizeClick(event) {
    var topPanel = F(topPanelClientID);
    var leftPanel = F(leftPanelClientID);

    var currentTool = this;
    if (currentTool.iconFont.indexOf('expand') >= 0) {
        topPanel.collapse();
        leftPanel.collapse();
        currentTool.setIconFont('compress');

    } else {
        topPanel.expand();
        leftPanel.expand();
        currentTool.setIconFont('expand');
    }
}

function MaximizeEnent() {
    var topPanel = F(topPanelClientID);
    var leftPanel = F(leftPanelClientID);
    var currentTool = F('RegionPanel_mainTabStrip_toolMaximize');

    topPanel.collapse();
    leftPanel.collapse();
    currentTool.setIconFont('compress');
}

///注释内容
// 添加示例标签页
// id： 选项卡ID
// iframeUrl: 选项卡IFrame地址 
// title： 选项卡标题
// icon： 选项卡图标
// createToolbar： 创建选项卡前的回调函数（接受tabOptions参数）
// refreshWhenExist： 添加选项卡时，如果选项卡已经存在，是否刷新内部IFrame
// iconFont： 选项卡图标字体		 
//function addExampleTab(tabOptions) {
//    if (typeof (tabOptions) === 'string') {
//        tabOptions = {
//            id: arguments[0],
//            iframeUrl: arguments[1],
//            title: arguments[2],
//            icon: arguments[3],
//            createToolbar: arguments[4],
//            refreshWhenExist: arguments[5],
//            iconFont: arguments[6]
//        };
//    }
//    F.addMainTab(F(mainTabStripClientID), tabOptions);
//}
//function onButtonSearch() {
//    addExampleTab({
//        id: '6121', iframeUrl: '/ERPCart/GoodsSearch.aspx?fid=6121', title: '购物车订货', closable: true, refreshWhenExist: false
//    });
//}
//function onButtonCart() {
//    addExampleTab({
//        id: '6122', iframeUrl: '/ERPCart/GoodsCart.aspx?fid=6122', title: '购物车结算', closable: true, refreshWhenExist: false
//    });
//}

function changeCartState() {
    F.customEvent('gwc');
}



// 展开左侧面板
function expandLeftPanel() {
    var leftPanel = F(leftPanelClientID);
    var menuStyle = F.cookie('MenuStyle_Pro') || 'tree';

    if (menuStyle === 'tree' || menuStyle === 'tree_minimode') {
        // 获取左侧树控件实例
        var leftMenuTree = leftPanel.items[0];

        leftMenuTree.miniMode = false;

        leftPanel.el.removeClass('minimodeinside');
        leftPanel.setWidth(260);


        ///F(leftPanelToolGearClientID).show();
        //F(leftPanelBottomToolbarClientID).show();
        //F(leftPanelToolCollapseClientID).setIconFont('chevron-circle-left');

        // 重新加载树菜单
        leftMenuTree.loadData();
        leftPanel.items[0].el.removeClass('f-tree-headerstyle');
        $('.f-tree-node').removeClass('ui-widget-header');
        $(leftPanel.el).find('.f-panel-title-text').css('visibility', 'visible');
        $(leftPanel.el).find('.f-icon-chevron-circle-right').removeClass('f-icon-chevron-circle-right').addClass('f-icon-chevron-circle-left');
    } else {
        leftPanel.expand();
    }
}


// 展开左侧面板
function collapseLeftPanel() {
    var leftPanel = F(leftPanelClientID);
    var menuStyle = F.cookie('MenuStyle_Pro') || 'tree';

    if (menuStyle === 'tree' || menuStyle === 'tree_minimode') {
        // 获取左侧树控件实例
        var leftMenuTree = leftPanel.items[0];

        leftMenuTree.miniMode = true;
        leftMenuTree.miniModePopWidth = 300;

        leftPanel.el.addClass('minimodeinside');
        leftPanel.setWidth(50);


        //F(leftPanelToolGearClientID).hide();
        //F(leftPanelBottomToolbarClientID).hide();
        //F(leftPanelToolCollapseClientID).setIconFont('chevron-circle-right');

        // 重新加载树菜单
        leftMenuTree.loadData();
        //默认
        $(leftPanel.el).find('.f-tree-minimode-icon.ui-icon').each(function (i) {
            if (!$(this).hasClass('f-tree-custom-iconfont')) {
                $(this).addClass('f-icon-file').addClass('f-tree-custom-iconfont');
            }
        })
        $(leftPanel.el).find('.f-panel-title-text').css('visibility', 'hidden');
        $(leftPanel.el).find('.f-icon-chevron-circle-left').removeClass('f-icon-chevron-circle-left').addClass('f-icon-chevron-circle-right');
        //f-icon-chevron-circle-right
        //

    } else {
        leftPanel.collapse();
    }
}

// 自定义展开折叠工具图标
function onLeftPanelToolCollapseClick(event) {
    var leftPanel = F(leftPanelClientID);
    var menuStyle = F.cookie('MenuStyle_Pro') || 'tree';

    if (menuStyle === 'tree' || menuStyle === 'tree_minimode') {
        // 获取左侧树控件实例
        var leftMenuTree = leftPanel.items[0];

        // 设置 miniMode 模式
        if (leftMenuTree.miniMode) {
            expandLeftPanel();
        } else {
            collapseLeftPanel();
        }
    } else {
        leftPanel.toggleCollapse();
    }
}
