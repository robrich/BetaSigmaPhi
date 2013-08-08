namespace BetaSigmaPhi.Web.Controllers {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Entity.Models;
	using BetaSigmaPhi.Repository;
	using BetaSigmaPhi.Repository.Models;
	using BetaSigmaPhi.Web.Filters;
	using BetaSigmaPhi.Web.Models;
	using Newtonsoft.Json;

	[RequireAdmin]
	public class ErrorLogController : Controller {
		private readonly IErrorLogRepository errorLogRepository;
		private readonly IUserRepository userRepository;

		private const int defaultPageSize = 50;

		public ErrorLogController( IErrorLogRepository ErrorLogRepository, IUserRepository UserRepository ) {
			if ( ErrorLogRepository == null ) {
				throw new ArgumentNullException( "ErrorLogRepository" );
			}
			if (UserRepository == null) {
				throw new ArgumentNullException("UserRepository");
			}
			this.errorLogRepository = ErrorLogRepository;
			this.userRepository = UserRepository;
		}

		public ActionResult Index(int? id, int? PageSize) {
			int pageNum = id ?? 1;
			if (pageNum < 1) {
				pageNum = 1;
			}
			int pageSize = PageSize ?? defaultPageSize;
			if (pageSize < 1) {
				pageSize = defaultPageSize;
			}
			PartialList<ErrorLog> results = this.errorLogRepository.GetPage(pageSize, pageNum);
			PartialListPageInfo<ErrorLogViewModel> model = new PartialListPageInfo<ErrorLogViewModel> {
				PageNumber = pageNum,
				PageSize = 50,
				TotalCount = results.TotalCount
			};
			if ( results.Count > 0 ) {
				List<User> userCache = new List<User>();
				foreach (ErrorLog result in results) {
					ErrorLogViewModel m = new ErrorLogViewModel(result);
					if (m.UserId > 0) {
						User user = userCache.FirstOrDefault(c => c.UserId == m.UserId);
						if (user == null) {
							user = this.userRepository.GetById(m.UserId ?? 0);
							if (user != null) {
								userCache.Add(user);
							}
						}
						if (user != null) {
							m.Email = user.Email;
						}
					}
					model.Add(m);
				}
			}
			return this.View(model);
		}

		public ActionResult Detail( int id ) {
			ErrorLog log = this.errorLogRepository.GetById( id );
			if ( log == null ) {
				return this.View( "NotFound" );
			}
			ErrorLogViewModel model = new ErrorLogViewModel( log );
			User user = this.userRepository.GetById( model.UserId ?? 0 );
			if (user != null) {
				model.Email = user.Email;
			}
			if (!string.IsNullOrEmpty(log.ExceptionDetails) && log.ExceptionDetails.IndexOf("{", StringComparison.InvariantCultureIgnoreCase) > -1) {
				try {
					model.ExceptionInfo = JsonConvert.DeserializeObject<ExceptionInfo>(log.ExceptionDetails);
				} catch {
					// FRAGILE: ASSUME: old-format (wasn't JSON-serialized errors)
					// Just show the string
					model.ExceptionInfo = null;
				}
			}
			return this.View( model );
		}

	}
}
