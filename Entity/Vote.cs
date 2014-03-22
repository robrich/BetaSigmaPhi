namespace BetaSigmaPhi.Entity {
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class Vote : IEntity {

		[Key]
		public int VoteId {
			get { return this.Id; }
			set { this.Id = value; }
		}

		public int PollId { get; set; }
		public Poll Poll { get; set; }

		public DateTime VoteDate { get; set; }

		public int VoterUserId { get; set; }
		[ForeignKey("VoterUserId")]
		public User VoterUser { get; set; }

		public int ElectedUserId { get; set; }
		[ForeignKey("ElectedUserId")]
		public User ElectedUser { get; set; }

		public int CategoryId { get; set; }
		public Category Category { get; set; }

	}
}
