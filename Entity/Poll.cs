namespace BetaSigmaPhi.Entity {
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class Poll : IEntity {

		[Key]
		public int PollId {
			get { return this.Id; }
			set { this.Id = value; }
		}

		[Column("FrequencyId")]
		public Frequency Frequency { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

	}
}
