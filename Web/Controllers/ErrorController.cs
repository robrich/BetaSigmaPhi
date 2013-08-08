namespace BetaSigmaPhi.Web.Controllers
{
	using System;
	using System.Web.Mvc;
	using BetaSigmaPhi.Service;
	using BetaSigmaPhi.Web.Models;

	public class ErrorController : Controller {
		private readonly ILoggerService loggerService;

		public ErrorController(ILoggerService LoggerService) {
			if (LoggerService == null) {
				throw new ArgumentNullException("LoggerService");
			}
			this.loggerService = LoggerService;
		}

		public ActionResult NotFound() {
			return this.View();
		}

		public ActionResult Error(int? ErrorId) {
			return this.View(new ErrorModel {ErrorId = ErrorId});
		}

		// Parameters match object passed from jquery.log.js
		// Avoid the "no such url" error [HttpPost]
		public ActionResult Log(string message, string errorUrl, string referrerUrl) {
			string result = "";
			if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(errorUrl)) { // else there's nothing to log
				string mess = "from JS: " + message;
				if (!string.IsNullOrEmpty(errorUrl)) {
					mess += ", URL: " + errorUrl;
				}
				if (!string.IsNullOrEmpty(referrerUrl)) {
					mess += ", Referrer: " + referrerUrl;
				}
				int? errorId = this.loggerService.Log(mess, RequestUrlOverride: errorUrl);
				if (errorId != null) {
					result += "reference problem ticket id " + errorId;
				}
			}
			return this.Json(new {mess = result});
		}

	}
}
