namespace BetaSigmaPhi.Tests.Base {
	using Moq;
	using Ninject;
	using Ninject.MockingKernel.Moq;
	using NUnit.Framework;

	[TestFixture]
	public abstract class TestBase {

		// Don't need a TransactionScope because there is no data access in these tests

		protected IMockServiceLocator MockServiceLocator { get; private set; }

		[SetUp]
		public void SetupMockRepository() {
			// MockBehavior.Strict says "I have to impliment every use"
			// DefaultValue.Mock means "recursive fakes"
			MockBehavior mockBehavior = MockBehavior.Strict;
			DefaultValue defaultValue = DefaultValue.Mock;

			// Ninject, why you gotta make this so hard?
			NinjectSettings settings = new NinjectSettings();
			settings.SetMockBehavior( mockBehavior );
			MoqMockingKernel kernel = new MoqMockingKernel( settings );
			kernel.MockRepository.DefaultValue = defaultValue;

			// Initialize the mock service locator
			this.MockServiceLocator = new MockServiceLocator( kernel, mockBehavior );
		}

		[TearDown]
		public void TearDownMockRepository() {
			try {
				// Verify all calls were made
				this.MockServiceLocator.VerifyAll();
			} finally {
				// Don't leak into the next test
				this.MockServiceLocator = null;
			}
		}

	}
}
