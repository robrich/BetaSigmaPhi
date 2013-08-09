namespace BetaSigmaPhi.Web.Models {
	using System.Collections.Generic;
	using BetaSigmaPhi.Entity;

	public class DocumentBreadcrumbViewModel {
		public List<Document> PathNodes { get; set; }
		/// <summary>
		/// Should the last node be a link?
		/// </summary>
		public bool LastNodeIsLink { get; set; }
	}
}
