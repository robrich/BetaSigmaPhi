/*global window:true, jQuery:true, alert:false */
(function (window,document,$,undefined) {
	"use strict";
	// Set $.log.url before use

	var urlParser = document.createElement('a'); // https://gist.github.com/2428561
	var getLocalUrl = function (url) {
		var results = url;
		if (url) {
			try {
				urlParser.href = url;
				if (urlParser.hostname) {
					// Only if a valid url
					if (urlParser.hostname === window.location.hostname) {
						// Only if url is in the same domain as the current page
						results = urlParser.pathname + urlParser.search + urlParser.hash;
						if (!results) {
							results = url;
						}
					}
				}
			} catch (err) {
				// FRAGILE: ASSUME: Failure to parse url isn't a deal breaker for logging
				results = url;
			}
		}
		return results;
	};

	// {text: "show this", type: severity}
	$.logAlert = function (options) {
		// TODO: use a growl-like interface instead of just alerts
		alert(options.text);
	};

	// content can be object or string, if it has a user property, it'll display that and not the full message to the user
	// opts: {user: true, system: true, noticeType: 'error'}
	// user: tell the user, system: log to server, noticeType: severity passed to logAlert()
	var log = function (content, opts) {
		var mess = content, // message to system
			usermess = null, // message to user
			url = $.log.url,
			data = null,
			userComplete = false,
			tellUser = null;

		var options = $.extend({
			user: true, // Tell the user
			system: true, // Tell the system
			noticeType: 'error' // Type type to pass to $.logAlert: notice, error, success, the class of the pop-up
		}, opts || {});

		if (!url && options.system) {
			options.system = false; // Can't tell the server because no server url specified
		}

		if (!options.user && !options.system) {
			return; // We've successfully done nothing
		}

		if (!content) {
			return; // No need to tell everyone nothing
		}

		if (typeof content !== 'string') {
			mess = JSON.stringify(content);
			if (!mess) {
				return; // No need to tell everyone nothing
			}
		}

		if (options.user) {
			// Form the user message
			usermess = mess; // default to JSON serialized content
			if (content && content.user) {
				usermess = content.user;
			}
			if(typeof usermess !== 'string') {
				usermess = JSON.stringify(usermess);
			}
			if (!usermess) {
				options.user = false; // Don't tell them nothing
			}
		}
		if (options.user) {
			// Tell the user
			if (options.system) {
				// Give the service a 1 second head start
				tellUser = function () {
					if (!userComplete) {
						userComplete = true;
						$.logAlert({ text: usermess, type: options.noticeType });
					}
				};
				setTimeout(tellUser, 1000);
			} else {
				// Just tell the user now
				$.logAlert({ text: usermess, type: options.noticeType });
			}
		}

		if (options.system) {
			// Tell the server
			if (!(url)) {
				throw "No log url specified, can't log this error: " + JSON.stringify(content);
			}

			data = {
				message: mess,
				errorUrl: getLocalUrl(window.location.href),
				referrerUrl: null
			};
			if (document.referrer) {
				data.referrerUrl = document.referrer;
			}

			$.ajax(url, {
				type: "POST",
				data: JSON.stringify(data),
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				success: function (results) {
					if (tellUser && results && results.mess) {
						if (!userComplete) {
							// Race to append to the message that'll display in the existing dialog
							usermess += ", " + results.mess;
							tellUser();
						} else {
							// Tell the user in a new message
							$.logAlert({ text: results.mess, type: options.noticeType });
						}
					}
				},
				error: function (/*xhr, status, error*/) {
					// Handle so we don't loop
					if (tellUser) {
						if (!userComplete) {
							// Race to append to the message that will display in the existing dialog
							usermess += ", Error saving to log";
							tellUser();
						} else {
							// Tell the user in a new message
							$.logAlert({ text: "Error saving to log", type: options.noticeType });
						}
					}
				},
				complete: function () {
					// It failed or otherwise tanked, tell the user if we haven't already
					if (tellUser && !userComplete) {
						tellUser();
					}
				}
			});
		}
	};

	var logBrowser = function (settings) {
		var origOnerror = window.onerror, rootSettings, filter;
		if (settings) {
			rootSettings = $.extend({}, settings);
		}
		if(rootSettings && rootSettings.filter && typeof rootSettings.filter === 'function') {
			filter = rootSettings.filter;
			delete rootSettings.filter;
		}
		window.onerror = function (message, url, lineNumber) {
			var proceed, logContent, localSettings;
			if (rootSettings) {
				localSettings = $.extend({},rootSettings);
			}
			logContent = {
				message: message || "window.onerror",
				url: getLocalUrl(url),
				lineNumber: lineNumber,
				source: "window.onerror"
			};
			if (filter) {
				proceed = filter({args:arguments, log:logContent, settings:localSettings});
				// like jQuery return falsey to disable, return truthy or undefined to proceed
			}
			if (proceed === undefined || proceed) {
				log(logContent, localSettings);
			}
			if (origOnerror) {
				origOnerror.apply(window, arguments);
			}
		};
	};
	var logAjax = function (settings) {
		var rootSettings, filter;
		if (settings) {
			rootSettings = $.extend({}, settings);
		}
		if(rootSettings && rootSettings.filter && typeof rootSettings.filter === 'function') {
			filter = rootSettings.filter;
			delete rootSettings.filter;
		}
		$(document).ajaxError(function (e, xhr, ajaxData/*, exception*/) {
			var proceed, logContent, localSettings;
			if (rootSettings) {
				localSettings = $.extend({},rootSettings);
			}
			logContent = {
				message: "Exception in $.ajax()",
				user: "A system error occurred in $.ajax(), try your request again or contact I.T. to report the problem"
			};
			if (ajaxData && ajaxData.url) {
				if (ajaxData.url === $.log.url) {
					return; // Don't recurse
				}
				logContent.url = ajaxData.url;
			}
			if (xhr) {
				if (xhr.status === 0 || xhr.readyState === 0 || xhr.status === 12017 || xhr.status === 12029) {
					// Either server wasn't available or client killed it to navigate to a different page
					// 12017 is Windows error meaning closed connection
					// 12029 is Windows error meaning can't connect to server
					return; // FRAGILE: ASSUME: we don't need to log this
				}
				logContent.httpStatus = xhr.status;
				logContent.responseText = xhr.responseText;
				try {
					var err = null, contentType;
					contentType = xhr.getResponseHeader('content-type');
					if (contentType && contentType.indexOf('application/json') > -1) {
						logContent.ex = JSON.parse(xhr.responseText);
						if (logContent.ex && logContent.ex.Message) {
							logContent.xhrMessage = err.Message;
						}
					}
				} catch (innerErr) {
					// FRAGILE: Swallow, avoid blowing up when trying to blow up
				}
			}
			if (filter) {
				proceed = filter({args:arguments, log:logContent, settings:localSettings});
				// like jQuery return falsey to disable, return truthy or undefined to proceed
			}
			if (proceed === undefined || proceed) {
				log(logContent, localSettings);
			}
		});
	};

	$.log = log;
	$.logBrowser = logBrowser;
	$.logAjax = logAjax;
}(window, window.document, jQuery));
