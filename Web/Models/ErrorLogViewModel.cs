namespace BetaSigmaPhi.Web.Models {
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Entity.Models;

	public class ErrorLogViewModel : ErrorLog {
		public ErrorLogViewModel( ErrorLog ErrorLog ) {
			// FRAGILE: ASSUME: we've copied all the properties
			this.ErrorLogId = ErrorLog.ErrorLogId;
			this.CreatedDate = ErrorLog.CreatedDate;
			this.Message = ErrorLog.Message;
			this.ExceptionDetails = ErrorLog.ExceptionDetails;
			this.UserId = ErrorLog.UserId;
			this.User = ErrorLog.User;
			this.ClientIp = ErrorLog.ClientIp;
			this.HttpMethod = ErrorLog.HttpMethod;
			this.PageUrl = ErrorLog.PageUrl;
			this.ReferrerUrl = ErrorLog.ReferrerUrl;
			this.UserAgent = ErrorLog.UserAgent;
		}
		public string Email { get; set; }
		public ExceptionInfo ExceptionInfo { get; set; }
	}
}
