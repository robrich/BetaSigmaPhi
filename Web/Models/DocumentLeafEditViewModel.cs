namespace BetaSigmaPhi.Web.Models {
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;

	public class DocumentLeafEditViewModel : DocumentBranchEditViewModel {

		// nvarchar(max)
		[AllowHtml]
		[DataType( DataType.MultilineText )]
		public string PageContent { get; set; }

		// nvarchar(max)
		[AllowHtml]
		[DataType( DataType.MultilineText )]
		public string HeadContent { get; set; }

		// nvarchar(max)
		[AllowHtml]
		[DataType( DataType.MultilineText )]
		public string ScriptContent { get; set; }
		
	}
}
