/*
购物车——商品搜索组件js
@Author 鞠怡
@Date 2015/07/28
*/
(function($){
	//私有方法
	var add = function($this){
		var ul = $('<ul></ul>');
		$this.append(ul);

		// var li = $('<li>'+text+'</li>');
		// var span = $('<span class="close">Y</span>');
		// span.on('click',function(e){
		// 	li.remove();
		// });
		// li.append(span);
		// if(ul){
		// 	ul.append(li);
		// }

	}

	var methods = {
        defaults:{
        	closeEvent:function(){
        		console.log('closed!');
        	},
        	openEvent:function(){
        		console.log('opened!');

        	}
        },
        opts:{},
// 在字面量对象中定义每个单独的方法
        init: function(arg) {
 			var options = arg[0]
            

// 为了更好的灵活性，对来自主函数，并进入每个方法中的选择器其中的每个单独的元素都执行代码
            return this.each(function() {
            	methods.opts = $.extend({}, methods.defaults, options);
// 为每个独立的元素创建一个jQuery对象
                var $this = $(this);
                add($this);
// 执行代码
                
// 例如： privateFunction();
            });
        },
        destroy: function() {
            
// 对选择器每个元素都执行方法
            return this.each(function() {
                
// 执行代码
            });
        },
        //公有方法
        add:function(arg){
        	return this.each(function() {
        		//console.log(methods.opts)
	        	var params = arg[1];
	        	var text = params['text'];
	        	var opts = $.extend({},methods.opts,params);
	        	//console.log(opts)
	        	var $this = $(this);
	        	var ul = $this.find('ul');
				var li = $('<li>'+text+'</li>');
				var span = $('<span class="close">X</span>');
				span.on('click', function (e) {
				    opts.closeEvent.call(li);
					li.remove();
					
				});
				//span.attr('data-value', text);
				li.append(span);
				if (ul) {
				    opts.openEvent.call(li);
				    ul.append(li);
				}
			});
        }
    };     
	$.fn.gwSelect = function() {    
		var method = arguments[0];
		if(methods[method]) {
// 如果方法存在，存储起来以便使用
// 注意：我这样做是为了等下更方便地使用each（）
            method = methods[method];
// 如果方法不存在，检验对象是否为一个对象（JSON对象）或者method方法没有被传入
        } else if( typeof(method) == 'object' || !method ) {
// 如果我们传入的是一个对象参数，或者根本没有参数，init方法会被调用
            method = methods.init;
        } else {
// 如果方法不存在或者参数没传入，则报出错误。需要调用的方法没有被正确调用
            $.error( 'Method ' +  method + ' does not exist on jQuery.gwSelect' );
            return this;
        }
        return method.call(this,arguments);


	}
})(jQuery);     