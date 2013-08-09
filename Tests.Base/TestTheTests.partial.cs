namespace BetaSigmaPhi.Tests.Base {
	using System;

	public abstract partial class TestTheTests : TestBase {
		public Type BaseTestType { get { return typeof(TestBase); } }	
	}
}
