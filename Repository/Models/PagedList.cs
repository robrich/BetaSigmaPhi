namespace BetaSigmaPhi.Repository.Models {
	using System.Collections.Generic;

	public class PartialList<T> : List<T> {

		public PartialList() {

		}

		public PartialList(IEnumerable<T> Items, int TotalCount)
			: base(Items ?? new List<T>()) {
			this.TotalCount = TotalCount;
		}

		public int TotalCount { get; set; }
	}
}
