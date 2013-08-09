namespace BetaSigmaPhi.Web.Models {
	using System.ComponentModel.DataAnnotations;

	public class DocumentBranchEditViewModel : DocumentBreadcrumbViewModel {
		public DocumentBranchEditViewModel() {
			this.IsActive = true;
		}

		public int DocumentId { get; set; }
		public int ParentDocumentId { get; set; }

		// Can be calculated: [Required]
		[StringLength( 80 )]
		public string Slug { get; set; }

		[Required]
		[StringLength( 80 )]
		public string Name { get; set; }

		public bool IsActive { get; set; }
	}
}
