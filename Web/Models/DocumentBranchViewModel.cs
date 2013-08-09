namespace BetaSigmaPhi.Web.Models {
	using System.Collections.Generic;
	using BetaSigmaPhi.Entity;

	public class DocumentBranchViewModel : DocumentLeafViewModel {
		public List<Document> Children { get; set; }
	}
}
