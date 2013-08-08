namespace BetaSigmaPhi.Web.App_Start {
	using System;
	using System.Net.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Dispatcher;
	using System.Web.Mvc;
	using System.Web.Routing;

	// For MVC
	public class FuncControllerFactory : DefaultControllerFactory {
		private readonly Func<Type, object> resolver;

		public FuncControllerFactory( Func<Type, object> Resolver ) {
			if ( Resolver == null ) {
				throw new ArgumentNullException( "Resolver" );
			}
			this.resolver = Resolver;
		}

		protected override IController GetControllerInstance( RequestContext requestContext, Type controllerType ) {
			if ( controllerType == null ) {
				return base.GetControllerInstance( requestContext, controllerType ); // 404
			}
			return this.resolver( controllerType ) as IController;
		}

	}

	// For Web API
	public class FuncHttpControllerActivatorFactory : IHttpControllerActivator {
		private readonly Func<Type, object> resolver;

		public FuncHttpControllerActivatorFactory( Func<Type, object> Resolver ) {
			if ( Resolver == null ) {
				throw new ArgumentNullException( "Resolver" );
			}
			this.resolver = Resolver;
		}

		public IHttpController Create( HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType ) {
			if ( controllerType == null ) {
				return null;
			}
			return this.resolver( controllerType ) as IHttpController;
		}

	}
}
