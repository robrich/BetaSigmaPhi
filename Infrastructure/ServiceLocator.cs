namespace BetaSigmaPhi.Infrastructure {
	using System;

	/// <summary>
	/// This is a Service Locator pattern gadget.  The preferred mechanism is Dependency Injection.  Sometimes that isn't possible<br />
	/// ASP.NET Web Pages can use parameter injection: aspx, asmx, ascx, etc<br />
	/// Only 3 uses should consume this<br />
	/// ALL OTHERS SHOULD RECEIVE THEIR DEPENDENCIES VIA CONSTRUCTOR INJECTION<br />
	/// Here are the 3 valid uses:<br />
	/// 1. Used sparingly to kick off the process, e.g. static void Main() and Application_Start()<br />
	/// 2. Attributes<br />
	/// 3. Extension method classes<br />
	/// If at all possible, one should create 2 constructors:<br />
	/// - The first constructor takes constructor injection parameters as always<br />
	/// - The second constructor calls the first constructor, e.g. : this( ServiceLocator.Get&lt;T&gt;(), ServiceLocator.Get&lt;T&gt;() )<br />
	/// If a second constructor isn't possible (static classes) one should initialize each dependency in the static constructor, setting private static readonly properties<br />
	/// Any use of ServiceLocator anywhere else is in error and should be removed.
	/// </summary>
	public static class ServiceLocator {
		private static Func<Type, object> getService;

		public static void Initialize( Func<Type, object> GetService ) {
			if ( GetService == null ) {
				throw new ArgumentNullException( "GetService" );
			}
			getService = GetService;
		}

		public static object GetService( Type Type ) {
			return getService( Type );
		}

		public static T GetService<T>() {
			return (T)GetService( typeof(T) );
		}

	}
}
