namespace BetaSigmaPhi.Repository {
	using System.Collections.Generic;
	using System.Linq;
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Repository.Models;

	public interface IErrorLogRepository : IRepository<ErrorLog> {
		PartialList<ErrorLog> GetPage(int PageSize, int PageNumber /*base 1*/);
	}

	public class ErrorLogRepository : Repository<ErrorLog>, IErrorLogRepository {

		public ErrorLogRepository(IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory)
			: base(BetaSigmaPhiContextFactory) {
		}

		public PartialList<ErrorLog> GetPage(int PageSize, int PageNumber /*base 1*/) {

			int page = PageNumber - 1; // Base 1 to base 0
			if (page < 0) {
				page = 0;
			}
			int pageSize = PageSize;
			if (pageSize < 1) {
				pageSize = 50;
			}

			using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext()) {
				var query = (
					from a in db.ErrorLogs
					orderby a.ErrorLogId descending
					select a
				);

				List<ErrorLog> data = query.Skip(pageSize * page).Take(pageSize).ToList();

				int totalItems = 0;
				if (data.Count < pageSize && (data.Count > 0 || page == 0)) {
					totalItems = data.Count + (pageSize * page); // We've got less than a full page and can figure out how many there are
				} else {
					totalItems = query.Count(); // Have to ask the db how many there are
				}

				return new PartialList<ErrorLog>(data, totalItems);
			}
		}

	}
}
