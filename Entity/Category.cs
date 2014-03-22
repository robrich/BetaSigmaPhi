namespace BetaSigmaPhi.Entity {
	using System.ComponentModel.DataAnnotations;

	public class Category : IEntity {

		[Key]
		public int CategoryId {
			get { return this.Id; }
			set { this.Id = value; }
		}

		[StringLength(200)]
		public string Name { get; set; }

	}
}
