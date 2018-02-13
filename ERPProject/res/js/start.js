

var sMax;	// 最大数量的星星即最大评分值
var holder; // 鼠标停留的评分控件
var preSet; // 保存了评分值（通过单击来进行评分）
var rated; //是否评分过，并保存了结果（注意此值一旦设为空，就不能再评分）

// 鼠标停留事件
function rating(num, id, sMaxs, sStart) {
    if (!rated()) {
        s = num.id.replace("_", '');
        a = 0;
        for (i = sStart; i <= sMaxs; i++) {
            if (i <= s) {
                document.getElementById("_" + i).className = "on";
                document.getElementById("rateStatus" + id).innerHTML = num.title;
                holder = a + 1;
                a++;
            } else {
                document.getElementById("_" + i).className = "off";
                //document.getElementById("rateStatus" + id).innerHTML = "评分...";
            }
        }
    }
}
function off(me, num, sMaxs, sStart2) {
    if (!rated()) {
        for (i = sStart2; i <= sMaxs; i++) {
            if (parseInt(document.getElementById("rateNow" + num).innerHTML) + sStart2 - 1 < i) {
                document.getElementById("_" + i).className = "off";

            }
            else {
                document.getElementById("_" + i).className = "on";
            }
        }
        if (document.getElementById("rateNow" + num).innerHTML == "0") {

            document.getElementById("rateStatus" + num).innerHTML = "评分...";
        }
        else {
            document.getElementById("rateStatus" + num).innerHTML = document.getElementById(("_" + document.getElementById("rateNow" + num).innerHTML)).title;
        }
    }
}
function down(me, num, sMaxs, sStart2) {
    if (!rated()) {
        document.getElementById("rateStatus" + num).innerHTML = me.title;
        document.getElementById("rateNow" + num).innerHTML = me.id.replace("_", '') - sStart2 + 1;
        //rating(me);
    }
}
function rateIt(me, num, sMax, sStart) {
    if (!rated()) {
        document.getElementById("rateStatus" + num).innerHTML = me.title;
        rating(me);
    }
}
function rated() {
    return preSet;
}
function load(num, id, sMaxs, sStart, sState) {
    for (i = sStart; i <= sMaxs; i++) {
        if (i < num + sStart) {
            document.getElementById("_" + i).className = "on";
        } else {
            document.getElementById("_" + i).className = "off";
        }
        if (num > 0) {
            document.getElementById("rateStatus" + id).innerHTML = document.getElementById("_" + num).title;
            document.getElementById("rateNow" + id).innerHTML = num;
        }
        else {
            document.getElementById("rateStatus" + id).innerHTML = "评分...";
            document.getElementById("rateNow" + id).innerHTML = 0;
        }
    }
    if (sState == "Y") {
        preSet = true;
    }
    else {
        preSet = false;
    }
}