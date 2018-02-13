
//新的图片放大
function fancyCallback(e) {
    e.preventDefault();
    $this = $(e.currentTarget);
    var optList = new Array();
    var rel = $this.attr('rel');
    var opt = {};
    var url = $this.find('img').attr('src');//TODO 改图片
    var title = $this.find('img').attr('title');
    opt.href = url;
    opt.title = title;
    opt.helpers = {
        title: {
            type: 'inside'
        },
        buttons: {}
    },
    optList.push(opt);
    if (typeof (rel) != 'undefined' && rel != null && rel != '') {
        var _index = -1;
        var outerSrc = $this.find('img').eq(0).attr('src');
        $('.fancybox[rel=' + rel + ']').each(function (i) {
            var innerSrc = $(this).find('img').eq(0).attr('src');

            if (innerSrc == outerSrc) {
                _index = i;
            }
            if (_index != -1 && innerSrc != outerSrc) {
                var opt = {};
                var url = $(this).find('img').attr('src');//TODO 改图片
                var title = $(this).find('img').attr('title');
                opt.href = url;
                opt.title = title;
                opt.helpers = {
                    title: {
                        type: 'inside'
                    },
                    buttons: {}
                },
            optList.push(opt);
            }
        });
        var _index2 = -1;
        $('.fancybox[rel=' + rel + ']').each(function (i) {
            var innerSrc = $(this).find('img').eq(0).attr('src');
            if (innerSrc == outerSrc) {
                _index2 = i;
            }
            if (_index2 == -1 && innerSrc != outerSrc) {
                var opt = {};
                var url = $(this).find('img').attr('src');//TODO 改图片
                var title = $(this).find('img').attr('title');
                opt.href = url;
                opt.title = title;
                opt.helpers = {
                    title: {
                        type: 'inside'
                    },
                    buttons: {}
                },
            optList.push(opt);
            }
        });
    }
    top.$.fancybox(optList);

}
