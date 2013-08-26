namespace BetaSigmaPhi.TestsBase.Integration {
	using System;
	using System.Configuration;
	using System.Transactions;
	using BetaSigmaPhi.Infrastructure;
	using DataAccess;
	using Ninject;
	using Ninject.Extensions.Conventions;
	using NUnit.Framework;
	using System.IO;

	[Category("Integration")]
	[TestFixture]
	public abstract class IntegrationTestBase {

		private TransactionScope transaction;

		protected ISqlHelper SqlHelper { get; private set; }

		// Technically we should probably reset this on each test, but we'll always get the same answer and it takes a while to do so ...

		// [TestCaseSource] runs before [TestFixtureSetUp] and we need this before even that
		// FRAGILE: Duplicate of Web/App_Start/NinjectWebCommon.cs
		public IntegrationTestBase() {

			// Kick off Ninject
			IKernel kernel = new StandardKernel();

			string path = new Uri(Path.GetDirectoryName(typeof(IntegrationTestBase).Assembly.CodeBase) ?? "").LocalPath;
			string thisNamespace = typeof(IntegrationTestBase).FullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]; // FRAGILE: ASSUME: All our code is in this namespace

			kernel.Bind(x => x
				.FromAssembliesInPath(path) // Blows with "not marked as serializable": , a => a.FullName.StartsWith( assemblyPrefix ) )
				.Select(type => type.IsClass && !type.IsAbstract && type.FullName.StartsWith(thisNamespace)) // .SelectAllClasses() wires up everyone else's stuff too
				.BindDefaultInterface()
			);

			// Add other bindings as necessary
			kernel.Rebind<IBetaSigmaPhiContext>().ToMethod(_ => (IBetaSigmaPhiContext)kernel.GetService(typeof(BetaSigmaPhiContext)));
			this.InitializeOtherTypes(kernel);

			// Initialize the service locator
			ServiceLocator.Initialize(kernel.GetService);

			// Use ServiceLocator sparingly to start us off
			this.SqlHelper = ServiceLocator.GetService<ISqlHelper>();

			// Start a transaction so we won't persist data changes made during tests
			this.transaction = new TransactionScope();
		}

		// Override in tests to initialize other things
		[Ignore]
		protected virtual void InitializeOtherTypes( IKernel Kernel ) {
		}

		[TestFixtureTearDown]
		public void ResetDI() {
			// FRAGILE: There's no way to undo the ServiceLocator: ServiceLocator.Initialize( null );
			this.SqlHelper = null;
			this.transaction.Dispose(); // Don't persist data changed during tests
			// TODO: Reseed tables changed during tests
		}

		/// <summary>
		/// FRAGILE: AppHarbor doesn't replace the connectionString in the config files until deployment, so we can't run integration tests as part of AppHarbor builds<br />
		/// *cough* FAIL *cough*
		/// </summary>
		[TestFixtureSetUp]
		public void IfRunningInAppHarbor() {
			string environment = ConfigurationManager.AppSettings["Environment"];
			if (environment == "Test") { 
				Assert.Ignore("FRAGILE: AppHarbor doesn't replace ConnectionString details until deployment, so we can't run integration tests as part of AppHarbor builds.");
			}
		}

	}
}