namespace BetaSigmaPhi.Entity {
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class Poll : IEntity {

		public Poll()  {
			this.Frequency = Frequency.Quarterly;
			this.VoteCountPerFrequency = 1;
		}

		[Key]
		public int PollId {
			get { return this.Id; }
			set { this.Id = value; }
		}

		public int CategoryId { get; set; }
		public Category Category { get; set; }

		[Column("FrequencyId")]
		public Frequency Frequency { get; set; }

		public int VoteCountPerFrequency { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		[NotMapped]
		public bool IsOpenForVoting {
			get { return DateTime.Now >= StartDate && DateTime.Now <= EndDate; }
		}

	}
}
