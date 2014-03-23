namespace BetaSigmaPhi.Web.Models {
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;

	public class LoginModel {

		[Required]
		[StringLength( 64 )]
		public string Email { get; set; }

		[Required]
		[StringLength( 64 )]
		[DataType( DataType.Password )]
		public string Password { get; set; }

		public bool RememberMe { get; set; }

		// Inbound
		public string ReturnUrl { get; set; }

		// Outbound
		public string Message { get; set; }

	}
}
