namespace BetaSigmaPhi.Entity {
	using System.ComponentModel.DataAnnotations;

	public class ErrorLog : IEntity {

		[Key]
		public int ErrorLogId {
			get { return this.Id; }
			set { this.Id = value; }
		}

		//nvarchar(MAX)
		public string Message { get; set; }
		//nvarchar(MAX)
		public string ExceptionDetails { get; set; }

		public int? UserId { get; set; }
		public User User { get; set; }
		[StringLength(128)]
		public string ClientIp { get; set; }
		[StringLength(10)]
		public string HttpMethod { get; set; }
		[StringLength(512)]
		public string PageUrl { get; set; }
		[StringLength(512)]
		public string ReferrerUrl { get; set; }
		[StringLength(1024)]
		public string UserAgent { get; set; }

	}
}
