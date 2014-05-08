namespace BetaSigmaPhi.Entity {
	using System;
	using System.ComponentModel.DataAnnotations;

	public class User : IEntity {
		public User() {
			this.AuthenticationToken = Guid.NewGuid().ToString( "N" );
			this.IsElectable = true;
		}

		[Key]
		public int UserId {
			get { return this.Id; }
			set { this.Id = value; }
		}

		// Change this and you lock them out of a remembered cookie
		[Required]
		[StringLength( 32 )]
		public string AuthenticationToken { get; set; }

		[Required]
		[StringLength( 128 )]
		public string Email { get; set; }

		[Required]
		[StringLength( 64 )]
		public string FirstName { get; set; }

		[StringLength( 64 )]
		public string LastName { get; set; }
		
		// TODO: Store HashType that created this password?
		[Required]
		[StringLength( 256 )]
		public string Password { get; set; }

		[Required]
		[StringLength( 64 )]
		public string Salt { get; set; }

		public int LoginFailCount { get; set; }
		public DateTime? LoginFailStartDate { get; set; }

		public bool IsAdmin { get; set; }

		public bool IsElectable { get; set; }

	}
}
