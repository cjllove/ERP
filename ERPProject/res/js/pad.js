//form 数据转换为json对象
function formToJson(formid) {
    var json = {};
    F(formid).el.find('.f-field-body-cell-inner').find('input').each(function (i) {
        var $this = $(this);
        if ($this.attr('id') == null) {
            return;
        }
        var idArray = $this.attr('id').replace('-inputEl', '').split("_");
        var colId = "";
        if (idArray.length > 1) {
            colId = idArray[idArray.length - 1].substr(3);
        }
        if (colId != null && colId != "") {
            if ($this.hasClass('f-field-datepicker')) {
                json[colId] = $this.val();
            } else {
                json[colId] = F($this.attr('id').replace('-inputEl', '')).getValue();
            }
        }
    })
    return json;
}

function lockForm(formid) {
    F(formid).el.find('.f-field-body-cell-inner').find('input').each(function (i) {
        var $this = $(this);
        if ($this.attr('id') == null) {
            return;

        }
        var thisid = $this.attr('id').replace('-inputEl', '');
        if (F(thisid)) {
            
            F(thisid).enable();
            F(thisid).setReadonly(true);
        }
        
    });
}

//转换null字符
function convertEmptyStr(str) {
    if (str != null) {
        return str;
    } else {
        return "";
    }
}
//转换日期字符串
function splitDate(dateStr) {
    if (dateStr == null) {
        return "";
    }
    if (dateStr.indexOf('/') > -1) {
        dateStr = dateStr.replace(/\//ig, '-');
    }
    if (dateStr.indexOf('T') > -1) {
        return convertEmptyStr(dateStr).split('T')[0];
    } else if (dateStr.indexOf(' ') > -1) {
        return convertEmptyStr(dateStr).split(' ')[0];
    }

    
    
}

//替换date控件为ipad自带的
function replaceDateControl(controlId) {
    F(controlId).el.find('input').attr('type', 'date');
    F(controlId).el.find(".f-field-triggerbox-icons").hide();
}

//给form赋值=PubFunc.FormDataSet()
function setFormValues(formId,data) {
    F(formId).el.find('input').each(function (i) {
        var $this = $(this);
        if ($this.attr('id') == null) {
            return;
        }else{
            var idArray = $this.attr('id').replace('-inputEl', '').split("_");
            var colId = "";
            if (idArray.length > 1) {
                colId = idArray[idArray.length - 1].substr(3);
            }
            if (data.length > 0) {
                var mydata = data[0];
                for (var a in mydata) {
                    if (a == colId) {
                        if ($this.hasClass("f-field-checkbox")) {
                            if (mydata[a] == "Y") {
                                F($this.attr('id').replace('-inputEl', '')).setValue(true)
                            } else {
                                F($this.attr('id').replace('-inputEl', '')).setValue(false)
                            }
                        } else {
                            F($this.attr('id').replace('-inputEl', '')).setValue(mydata[a]);
                        }
                        
                    }
                }
            }
            
        }
    })
}

//生成guid
function s4() {
    return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
};
function guid() {
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}


function toggleMask(isHide) {
    //console.log($('#f_loading_mask'))
    if ($('#f_loading').length > 0 && $('#f_loading_mask').length > 0) {
        if (isHide) {
            $('#f_loading').hide()
            //$('#f_loading_mask').hide()
            return;
        }
        //console.log($('#f_loading').css('display') == 'none' || $('#f_loading_mask').css('display') == 'none')
        //console.log($('#f_loading').css('display') )
        if ( $('#f_loading').css('display') == 'none' || $('#f_loading_mask').css('display') == 'none') {
            $('#f_loading').show()
            //$('#f_loading_mask').show()
        } else  {
            $('#f_loading').hide()
            //$('#f_loading_mask').hide()
        }

    }
}


var supportsOrientationChange = "onorientationchange" in window,
    orientationEvent = supportsOrientationChange ? "orientationchange" : "resize";

