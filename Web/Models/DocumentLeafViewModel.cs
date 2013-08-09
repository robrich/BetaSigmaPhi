namespace BetaSigmaPhi.Web.Models {
	using BetaSigmaPhi.Entity;

	public class DocumentLeafViewModel : DocumentBreadcrumbViewModel {
		public Document Document { get; set; }

		/// <summary>
		/// The path to this node without the base
		/// </summary>
		public string NodeSlugPath { get; set; }
	}
}
