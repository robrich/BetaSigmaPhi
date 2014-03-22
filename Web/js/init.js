/*global Modernizr:false, moment:false, $:false */
(function () {
	"use strict";
	
	Modernizr.load({
		test: (window.JSON && JSON.stringify && JSON.parse),
		nope: "//cdnjs.cloudflare.com/ajax/libs/json3/3.2.4/json3.min.js"
	});
	Modernizr.load({
		test: Modernizr.input.placeholder,
		nope: ['//cdnjs.cloudflare.com/ajax/libs/jquery-placeholder/2.0.7/jquery.placeholder.min.js'],
		complete: function () {
			if (!Modernizr.input.placeholder) {
				$('input, textarea').placeholder();
			}
		}
	});
	$.log.url = '/error/log';
	// Hook $.ajax's error details
	$.logAjax({
		filter: function (e) {
			if (e && e.log) {
				var resp = e.log.responseText;
				// FRAGILE: string matching portions of the login box and hard-coding the login url
				if (resp && resp.indexOf("<html") > -1 && resp.indexOf("/Login") > -1) {
					// They're unauthenticated getting to an authenticated page
					var parser = $("<a />").get(0);
					parser.href = window.location.href;
					var url = parser.pathname + parser.search + parser.hash;
					window.location.href = "/Login?ReturnUrl=" + encodeURI(url);
					return false; // No need to log it
				}
			}
		}
	});
	$.logBrowser(); // Hook browser's error details
}());
