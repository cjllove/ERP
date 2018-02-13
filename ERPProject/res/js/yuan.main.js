﻿/*********************************************************************************
* 系 统 名 ：威高讯通院内物流管理系统HSC  V3.0
* 程 序 ID ：yuan.main.js
* 功能概要 ：系统主界面函数
*
* 开发组织 ：威高讯通信息科技有限公司 产品研发部
*                      Copyright(C) 2015 WEGO System Service Co.Ltd
*                                                        All rights reserved.
*********************************************************************************
*   本程序为公司机密信息，未经公司书面许可授权，不得向第三方披露程序的相关信息。
*                                                   威高讯通信息科技有限公司
*********************************************************************************
* VERSION   DATE        BY             CHANGE/COMMENT
* 0.00      2015/08/08  c          新作成
*********************************************************************************/

F.ready(function () {

    var treeMenu = F(DATA.treeMenu),
        regionPanel = F(DATA.regionPanel),
        regionLeft = F(DATA.regionLeft),
        mainTabStrip = F(DATA.mainTabStrip);
        //txtUser = F(DATA.txtUser);


    // 欢迎信息和在线用户数
    //txtUser.setText('<span class="label">欢迎您：</span><span>' + DATA.userName + '</span>');

    function leftPad(source, count, prefix) {
        source += '';
        if (source.length < count) {
            for (var i = source.length; i < count; i++) {
                source = prefix + source;
            }
        }
        return source;
    }

    function leftPadTime(source) {
        return leftPad(source, '2', '0');
    }
	  

    function addTabCallback(tabConfig) {
        var str = new Array();
        str = tabConfig.id.split("_");
        if (str[1] == "N") {
            regionLeft.collapse();
            //regionLeft.collapsed = true;
        } else {
            regionLeft.expand();
            regionLeft.collapsed = false;
        }
    }


    // 初始化主框架中的树(或者Accordion+Tree)和选项卡互动，以及地址栏的更新
    // treeMenu： 主框架中的树控件实例，或者内嵌树控件的手风琴控件实例
    // mainTabStrip： 选项卡实例
    // addTabCallback： 创建选项卡前的回调函数（接受tabConfig参数）
    // updateLocationHash: 切换Tab时，是否更新地址栏Hash值
    // refreshWhenExist： 添加选项卡时，如果选项卡已经存在，是否刷新内部IFrame
    // refreshWhenTabChange: 切换选项卡时，是否刷新内部IFrame
    F.util.initTreeTabStrip(treeMenu, mainTabStrip, addTabCallback, true, false, false);

    // 公开添加示例标签页的方法
    window.addExampleTab = function (id, url, text, icon, refreshWhenExist) {
        // 动态添加一个标签页
        // mainTabStrip： 选项卡实例
        // id： 选项卡ID
        // url: 选项卡IFrame地址 
        // text： 选项卡标题
        // icon： 选项卡图标
        // addTabCallback： 创建选项卡前的回调函数（接受tabConfig参数）
        // refreshWhenExist： 添加选项卡时，如果选项卡已经存在，是否刷新内部IFrame
        F.util.addMainTab(mainTabStrip, id, url, text, icon, null, refreshWhenExist);
    };

    window.removeActiveTab = function () {
        var activeTab = mainTabStrip.getActiveTab();
        mainTabStrip.removeTab(activeTab.id);
    };
});