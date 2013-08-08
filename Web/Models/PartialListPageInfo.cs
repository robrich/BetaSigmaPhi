namespace BetaSigmaPhi.Web.Models {
	using System;
	using BetaSigmaPhi.Repository.Models;

	public class PartialListPageInfo<T> : PartialList<T> {
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalPages {
			get {
				if ( this.PageSize == 0 ) {
					return 0;
				}
				return (int)Math.Ceiling( (double)this.TotalCount / (double)this.PageSize );
			}
		}
	}
}
