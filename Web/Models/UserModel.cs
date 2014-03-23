namespace BetaSigmaPhi.Web.Models {
	using System.ComponentModel.DataAnnotations;

	public class UserModel {

		public int UserId { get; set; }

		[Required]
		[StringLength( 128 )]
		public string Email { get; set; }

		[Required]
		[StringLength( 64 )]
		public string FirstName { get; set; }

		[StringLength( 64 )]
		public string LastName { get; set; }

		[StringLength( 64 )]
		[DataType( DataType.Password )]
		public string Password { get; set; }

		public bool IsAdmin { get; set; }

		public bool IsElectable { get; set; }

		public bool IsActive { get; set; }

	}
}
