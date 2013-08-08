namespace BetaSigmaPhi.Entity {
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	/// <summary>
	/// It is an interface, but to avoid &quot;where T : IEntity, new()&quot; we make it abstract instead
	/// </summary>
	public abstract class IEntity {

		public IEntity() {
			this.IsActive = true;
			this.CreatedDate = DateTime.Now;
			this.ModifiedDate = DateTime.Now;
		}

		[NotMapped]
		public int Id { get; set; }
		[Timestamp]
		public byte[] RowVersion { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }

		public bool IsNew() {
			return this.Id < 1 || this.RowVersion == null || this.RowVersion.Length == 0;
		}

	}
}
