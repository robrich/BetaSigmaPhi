namespace BetaSigmaPhi.Entity {
	using BetaSigmaPhi.Entity.Helpers;

	[LookupTable("dbo.NodeType","NodeTypeName","NodeTypeId")]
	public enum NodeType {
		Root = 1,
		Branch = 2,
		Leaf = 3
	}
}
