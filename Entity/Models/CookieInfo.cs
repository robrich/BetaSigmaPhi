namespace BetaSigmaPhi.Entity.Models {
	using System.Collections.Generic;

	public class CookieInfo {
		public string Name { get; set; }
		public string Value { get; set; }
		public List<CookieInfo> Values { get; set; }
	}
}
