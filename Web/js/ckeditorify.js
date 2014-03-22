/*global $, CKEDITOR */
/*exported ckeditorify */
var ckeditorify = function (options) {
	"use strict";
	var target = $(options.target);
	if (target.is("textarea")) {
		/*jshint camelcase:false */
		target.ckeditor({
			resize_dir: 'both',
			width: options.width || 960,
			height: options.height || 500,
			extraPlugins: 'divarea',
			allowedContent: true
		});
		/*jshint camelcase:true */
	}
};
