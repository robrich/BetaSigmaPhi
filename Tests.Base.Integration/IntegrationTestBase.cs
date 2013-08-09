namespace BetaSigmaPhi.TestsBase.Integration {
	using System;
	using BetaSigmaPhi.Infrastructure;
	using DataAccess;
	using Entity;
	using Library;
	using Ninject;
	using Ninject.Extensions.Conventions;
	using NUnit.Framework;
	using System.IO;
	using Repository;

	[Category("Integration")]
	[TestFixture]
	public abstract class IntegrationTestBase {

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

			// Initialize the service locator
			ServiceLocator.Initialize(kernel.GetService);

			// Use ServiceLocator sparingly to start us off
			this.SqlHelper = ServiceLocator.GetService<ISqlHelper>();
		}

		// Override in tests to initialize other things
		[Ignore]
		protected virtual void InitializeOtherTypes( IKernel Kernel ) {
		}

		[TestFixtureTearDown]
		public void ResetDI() {
			// FRAGILE: There's no way to undo the ServiceLocator: ServiceLocator.Initialize( null );
			this.SqlHelper = null;
		}

		/*
		We use a test db spun up for this purpose, so we need not:
		- create a TransactionScope that will avoid persisting the data
		- reseed tables whose primary key seeds got advanced
		*/

	}
}