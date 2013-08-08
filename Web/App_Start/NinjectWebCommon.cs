using BetaSigmaPhi.Web.App_Start;

[assembly: WebActivator.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace BetaSigmaPhi.Web.App_Start {
	using System;
	using System.IO;
	using System.Web;
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Infrastructure;
	using Microsoft.Web.Infrastructure.DynamicModuleHelper;
	using Ninject;
	using Ninject.Extensions.Conventions;
	using Ninject.Web.Common;

	public static class NinjectWebCommon {
		private static readonly Bootstrapper bootstrapper = new Bootstrapper();

		/// <summary>
		/// Starts the application
		/// </summary>
		public static void Start() {
			DynamicModuleUtility.RegisterModule( typeof(OnePerRequestHttpModule) );
			DynamicModuleUtility.RegisterModule( typeof(NinjectHttpModule) );
			bootstrapper.Initialize( CreateKernel );
		}

		/// <summary>
		/// Stops the application.
		/// </summary>
		public static void Stop() {
			bootstrapper.ShutDown();
		}

		/// <summary>
		/// Creates the kernel that will manage your application.
		/// </summary>
		/// <returns>The created kernel.</returns>
		private static IKernel CreateKernel() {
			var kernel = new StandardKernel();
			kernel.Bind<Func<IKernel>>().ToMethod( ctx => () => new Bootstrapper().Kernel );
			kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

			RegisterServices( kernel );
			return kernel;
		}

		/// <summary>
		/// Load your modules or register your services here!
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		private static void RegisterServices( IKernel kernel ) {

			string path = new Uri( Path.GetDirectoryName( typeof( NinjectWebCommon ).Assembly.CodeBase ) ?? "" ).LocalPath;
			string thisNamespace = typeof( NinjectWebCommon ).FullName.Split( new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries )[0]; // FRAGILE: ASSUME: All our code is in this namespace

			kernel.Bind( x => x
				.FromAssembliesInPath( path ) // Blows with "not marked as serializable": , a => a.FullName.StartsWith( assemblyPrefix ) )
				.Select( type => type.IsClass && !type.IsAbstract && type.FullName.StartsWith( thisNamespace ) ) // .SelectAllClasses() wires up everyone else's stuff too
				.BindDefaultInterface()
				.Configure( b => b.InRequestScope() )
			);

			// Add other bindings as necessary
			kernel.Rebind<IBetaSigmaPhiContext>().ToMethod( _ => (IBetaSigmaPhiContext)kernel.GetService( typeof(BetaSigmaPhiContext) ) );

			// Initialize the service locator
			ServiceLocator.Initialize( kernel.GetService );

		}

	}
}
