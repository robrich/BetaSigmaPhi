namespace BetaSigmaPhi.Tests.Base {
	using System;
	using TestsBase.Integration;

	public abstract partial class TestTheTests : IntegrationTestBase {
		public Type BaseTestType { get { return typeof( IntegrationTestBase ); } }
	}
}
