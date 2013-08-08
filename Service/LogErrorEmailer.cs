namespace BetaSigmaPhi.Service {
	using System;
	using System.Diagnostics;
	using System.Net.Mail;
	using System.Text;
	using System.Web;
	using System.Web.Hosting;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Library;
	using BetaSigmaPhi.Repository;

	public interface ILogErrorEmailer {
		void SendErrorEmail(ErrorLog ErrorLog);
	}

	public class LogErrorEmailer : ILogErrorEmailer {
		private readonly ISettingRepository settingRepository;

		public LogErrorEmailer(ISettingRepository SettingRepository) {
			if (SettingRepository == null) {
				throw new ArgumentNullException("SettingRepository");
			}
			this.settingRepository = SettingRepository;
		}

		public void SendErrorEmail(ErrorLog ErrorLog) {
			if (!this.settingRepository.SendErrorEmails) {
				return;
			}
#if DEBUG
			if (HostingEnvironment.IsHosted && HttpContext.Current != null && HttpContext.Current.Request.IsLocal) {
				return; // No need to email locally run requests
			}
			if (Debugger.IsAttached) {
				return; // No need to email when debugging
			}
#endif

			string br = "<br />";

			string subject = ErrorLog.Message;
			if (string.IsNullOrWhiteSpace(subject)) {
				subject = ErrorLog.ExceptionDetails;
			}
			subject = subject.TrimToLength(40);

			MailMessage mail = new MailMessage();
			mail.To.Add(this.settingRepository.ErrorEmailAddress);
			mail.From = new MailAddress(this.settingRepository.ErrorEmailAddress);
			mail.Subject = String.Format("Error {0}: {1}", ErrorLog.ErrorLogId, subject);

			StringBuilder sb = new StringBuilder();
			sb.Append("<b>Date</b>: " + ErrorLog.CreatedDate);
			sb.Append(string.Format("<b>ErrorLogId</b>: <a href=\"{0}/ErrorLog/Detail/{1}\">{1}</a>{2}", this.settingRepository.SiteUrl, ErrorLog.ErrorLogId, br));
			sb.Append("<b>PageUrl</b>: " + HttpUtility.HtmlEncode(ErrorLog.PageUrl ?? "") + br + br);
			sb.Append("<b>User</b>: " + HttpUtility.HtmlEncode(ErrorLog.UserId) + br + br);
			sb.Append("<b>UserAgent</b>: " + HttpUtility.HtmlEncode(ErrorLog.UserAgent ?? "") + br + br);
			sb.Append("<b>Message</b>: " + HttpUtility.HtmlEncode(ErrorLog.Message ?? "") + br + br);
			sb.Append("<b>Exception</b>: " + HttpUtility.HtmlEncode(ErrorLog.ExceptionDetails ?? "").Replace("\n", "<br />"));

			mail.Body = sb.ToString();
			mail.IsBodyHtml = true;

			using (SmtpClient client = new SmtpClient()) {
				try {
					client.Send(mail);
				} catch {
					// Don't error trying to error
				}
			}
		}

	}
}
