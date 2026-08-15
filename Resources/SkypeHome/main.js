var main = function(){
	var _translationElements = [];
	var _language = "en";
	var _context;
	var _defaultLanguage = "en";
	var _languageScriptChange = function(callback){
		// $.ajax({
		// 	url: "languages/"+_language+".js?"+(Math.ceil(Math.random()*1000)),
		// 	dataType: 'script',
		// 	success:function(){
		// 		//if its arabic or hebrew, add the class "rtl" to the html tag and add rtl css file
		// 		if(_language == "he" || _language == "ar"){
		// 			$("html")
		// 				.addClass("rtl")
		// 				.attr("dir","rtl");
		// 			$("head")	
		// 				.append("<link rel='stylesheet' type='text/css' href='languages/rtl.css' id='rtlCSS'/>");
		// 		}
		// 		else{
		// 			$("html")
		// 				.removeClass("rtl")
		// 				.removeAttr("dir","");
		// 			$("head")
		// 				.find("#rtlCSS").remove();
		// 		}
		// 		$("html")
		// 			.attr("lang",_language);
		// 		if(callback)
		// 			callback();			
		// 	},
		// 	error: function() {
		// 		if(callback)
		// 			callback();
		// 	}
		// });

		//if its arabic or hebrew, add the class "rtl" to the html tag and add rtl css file
		if(_language == "he" || _language == "ar"){
			$("html")
				.addClass("rtl")
				.attr("dir","rtl");
		}
		else{
			$("html")
				.removeClass("rtl")
				.removeAttr("dir","")
				.scrollLeft(0);
		}
		$("html")
			.attr("lang",_language);
		
		if(callback)
			callback();
	
	};
	
	return{
		init:function(){
			var thisObj = this;
			_context = $("body");
			if(SH.API && SH.API.getClient().language)
				_language = SH.API.getClient().language;
			else if (window.external.language)
				_language = window.external.language;
			this.setTranslationElements();
			_languageScriptChange(function(){
				thisObj.translate();
				thisObj.getContext().show();
			});
			this.attachEvents();
		},
		setTranslationElements:function(){
			_translationElements.push({id:"header",dom:$("#content .header")});
			_translationElements.push({id:"p1",dom:$("#content .p1")});
			_translationElements.push({id:"p2",dom:$("#content .p2")});
			_translationElements.push({id:"list1li1",dom:$("#content .list1li1")});
			_translationElements.push({id:"list1li2",dom:$("#content .list1li2")});
		},
		translate:function(){
			for(var i=0,len=_translationElements.length;i<len;i++){
				var trans = 
				(typeof NC_Languages[_language] =="undefined" || typeof NC_Languages[_language][_translationElements[i].id] =="undefined") ? 
				NC_Languages[_defaultLanguage][_translationElements[i].id] :
				NC_Languages[_language][_translationElements[i].id];
				_translationElements[i].dom.text(trans);
			}
		},
		attachEvents:function(){
			var thisObj = this;
			//listen to language changes
			if(SH.API)
				SH.API.setLanguageChangeListener(
					function(language){
						_language = language;
						//get the new script for this language and then rerun the translator
						_languageScriptChange(
							function(){
								thisObj.translate();
							}
						);
					}
				);			
		},
		getContext:function(){
			return _context;
		}
	};
}();
$(document).ready(function(){
	main.init();
});