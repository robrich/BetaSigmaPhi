namespace BetaSigmaPhi.Service {
	using System;
	using System.Collections.Generic;
	using System.Data.Entity.Validation;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading;
	using System.Web;
	using System.Web.Hosting;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Entity.Models;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Library;
	using BetaSigmaPhi.Repository;
	using Newtonsoft.Json;

	public class Logger : LoggerService, ILogger {
		public Logger(IErrorLogRepository ErrorLogRepository, ILogErrorEmailer LogErrorEmailer)
			: base(ErrorLogRepository, LogErrorEmailer) {
		}

		public int? Log(string Message, Exception ex = null) {
			return base.Log(Message, ex, RequestUrlOverride: null);
		}

		public int? Log(Exception ex) {
			return this.Log(null, ex);
		}
	}

	public interface ILoggerService {
		int? Log(string Message, Exception ex = null, string RequestUrlOverride = null);
	}

	public class LoggerService : ILoggerService {
		private readonly IErrorLogRepository errorLogRepository;
		private readonly ILogErrorEmailer logErrorEmailer;

		public LoggerService(IErrorLogRepository ErrorLogRepository, ILogErrorEmailer LogErrorEmailer) {
			if (ErrorLogRepository == null) {
				throw new ArgumentNullException("ErrorLogRepository");
			}
			if (LogErrorEmailer == null) {
				throw new ArgumentNullException("LogErrorEmailer");
			}
			this.errorLogRepository = ErrorLogRepository;
			this.logErrorEmailer = LogErrorEmailer;
		}
		
		public int? Log(string Message, Exception ex = null, string RequestUrlOverride = null) {
			try {
				return this.PrivateLog(Message, ex, RequestUrlOverride);
			} catch (Exception ex2) {
				// Don't error trying to error
				if (ex2 is ThreadAbortException) {
					throw;
				}

				// Try again
				try {
					return this.PrivateLog("Error saving to error log", ex2, RequestUrlOverride);
				} catch (Exception ex3) {
					// Don't error trying to error
					if (ex3 is ThreadAbortException) {
						throw;
					}
#if DEBUG
					throw;
#else
					return null;
#endif
				}
			}
		}
		
		private int? PrivateLog( string Message, Exception ex = null, string RequestUrlOverride = null ) {
			
			ErrorLog err = new ErrorLog {
				Message = Message
			};

			// Date, UserId, etc are inherint in IEntity and Repository<T>

			if (HostingEnvironment.IsHosted) {
				HttpContext context = HttpContext.Current;
				if (context != null && context.Request != null) {
					err.HttpMethod = context.Request.HttpMethod;
					err.PageUrl = context.Request.Url.PathAndQuery; // .OriginalString;
					err.UserAgent = context.Request.UserAgent;
					err.ClientIp = context.Request.UserHostAddress;
					if (context.Request.UrlReferrer != null) {
						err.ReferrerUrl = context.Request.UrlReferrer.OriginalString;
					}
				}
				if (context != null && context.User != null) {
					err.UserId = null; // TODO: get the current user id, e.g. this.userCurrentService.CurrentUserId;
				}
			}

			if (!string.IsNullOrEmpty(RequestUrlOverride)) {
				err.PageUrl = RequestUrlOverride;
			}

			if (ex != null) {
				ExceptionInfo exInfo = this.GetExceptionInfo(ex);
				err.ExceptionDetails = JsonConvert.SerializeObject(exInfo);
			}

			// TODO: try/catch around each, perhaps the other will succeed
			this.errorLogRepository.Save(err);
			this.logErrorEmailer.SendErrorEmail(err);

			if (ex is ThreadAbortException) {
				throw ex; // Can't just "throw" because we aren't necessarily inside a catch
				// FRAGILE: you'll lose the original stack trace
			}

			return err.ErrorLogId;
		}

		// public to be testable, not part of interface
		public ExceptionInfo GetExceptionInfo(Exception ex) {
			if (ex == null) {
				return null;
			}
			ExceptionInfo exInfo = new ExceptionInfo {
				Message = ex.Message,
				StackTrace = ex.StackTrace,
				ExceptionType = ex.GetType().ToString()
			};
			if (ex.Data != null && ex.Data.Count > 0) {
				List<string> data = new List<string>();
				foreach (string key in ex.Data.Keys) {
					var value = ex.Data[key];
					if (value != null) {
						data.Add(key + ": " + value);
					}
				}
				exInfo.Data = data;
			}

			AggregateException aEx = ex as AggregateException;
			if (aEx != null) {
				exInfo.InnerExceptions = (
					from i in aEx.InnerExceptions
					select this.GetExceptionInfo(i)
				).ToList();
			}

			DbEntityValidationException dbEx = ex as DbEntityValidationException;
			if (dbEx != null) {
				List<string> validationErrors = (
					from e in dbEx.EntityValidationErrors
					from f in e.ValidationErrors
					select f.PropertyName + ": " + f.ErrorMessage
				).ToList();
				if (!validationErrors.IsNullOrEmpty()) {
					if (exInfo.Data == null) {
						exInfo.Data = new List<string>();
					}
					exInfo.Data.AddRange(validationErrors);
				}
			}

			WebException wEx = ex as WebException;
			if (wEx != null) {
				HttpWebResponse resp = wEx.Response as HttpWebResponse;
				if (resp != null) {

					StringBuilder content = new StringBuilder();
					if (resp.ResponseUri != null) {
						content.AppendLine("URL: " + resp.ResponseUri.OriginalString);
					}
					content.AppendLine("HttpMethod: " + resp.Method);
					content.AppendLine("HttpStatus: " + (int)resp.StatusCode);
					foreach (string header in resp.Headers.AllKeys) {
						content.AppendLine(header + ": " + resp.Headers[header]);
					}
					try {
						string body = null;
						using (Stream respStream = resp.GetResponseStream()) {
							if (respStream != null) {
								using (StreamReader sr = new StreamReader(respStream)) {
									body = sr.ReadToEnd();
								}
							}
						}
						if (!string.IsNullOrEmpty(body)) {
							content.AppendLine();
							content.AppendLine(body);
						}
					} catch {
						// Don't error when trying to error
					}

					if (exInfo.Data == null) {
						exInfo.Data = new List<string>();
					}
					exInfo.Data.Add(content.ToString());
				}
			}

			if (ex.InnerException != null) {
				exInfo.InnerException = this.GetExceptionInfo(ex.InnerException); // recurse
			}
			return exInfo;
		}

	}
}
