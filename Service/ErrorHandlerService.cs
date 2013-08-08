namespace BetaSigmaPhi.Service {
	using System;
	using System.Configuration;
	using System.Web;
	using System.Web.Configuration;
	using System.Web.Hosting;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Service.Models;

	/// <summary>
	/// The back-end service for both HandleAndLogErrorAttribute and Global.asax<br />
	/// If you're here by accident, you probably want ILogger in the Infrasturcture project
	/// </summary>
	public interface IErrorHandlerService {
		ErrorHandledResult HandleError( Exception ex );
	}

	/// <summary>
	/// The back-end service for both HandleAndLogErrorAttribute and Global.asax<br />
	/// If you're here by accident, you probably want ILogger in the Infrasturcture project
	/// </summary>
	public class ErrorHandlerService : IErrorHandlerService {
		private readonly ILogger logger;

		private CustomErrorsMode redirectMode { get; set; }
		private bool redirectModeSet { get; set; }

		public ErrorHandlerService( ILogger Logger ) {
			if ( Logger == null ) {
				throw new ArgumentNullException( "Logger" );
			}
			this.logger = Logger;
		}

		public bool IsNotFoundException( Exception ex ) {
			bool result = false;
			if ( ex != null && ex.GetType() == typeof(HttpException) ) {
				if ( !string.IsNullOrEmpty( ex.Message )
					&& ( ex.Message.EndsWith( " does not exist." ) || ex.Message.Contains( " was not found" ) ) ) {
					result = true;
				}
			}
			return result;
		}

		// public to be testable, not part of interface
		/// <summary>
		/// Cache the answer of the relatively expensive ErrorIsHandledInternal()
		/// </summary>
		public bool ErrorIsHandled() {
			bool handled = true;

			switch ( this.GetRedirectMode() ) {
				case CustomErrorsMode.Off:
					handled = false;
					break;
				case CustomErrorsMode.On:
					handled = true;
					break;
				case CustomErrorsMode.RemoteOnly:
					if ( HostingEnvironment.IsHosted ) {
						try {
							handled = !HttpContext.Current.Request.IsLocal;
						} catch {
							// Don't error while trying to handle an error
							handled = false;
						}
					}
					break;
			}

			return handled;
		}

		// public to be testable, not part of interface
		public CustomErrorsMode GetRedirectMode() {
			if ( !this.redirectModeSet ) {
				// Yeah, I could lock, but it isn't that expensive to get the answer, so let both threads do it

				this.redirectMode = CustomErrorsMode.RemoteOnly;

				// Read web.config's line like so:
				// <customErrors mode="RemoteOnly" />
				Configuration configuration = WebConfigurationManager.OpenWebConfiguration( "/" );
				SystemWebSectionGroup systemWeb = (SystemWebSectionGroup)configuration.GetSectionGroup( "system.web" );
				if ( systemWeb != null && systemWeb.CustomErrors != null ) {
					this.redirectMode = systemWeb.CustomErrors.Mode;
				}

				this.redirectModeSet = true;
			}
			return this.redirectMode;
		}

		public Exception GetTheRealException( Exception Source ) {
			Exception result = Source;
			while ( result != null && result.InnerException != null ) {
				bool useInner = false;

				if ( result is HttpUnhandledException ) {
					useInner = true;
				} else if ( result is HttpException && result.Message == "The client disconnected." ) {
					useInner = true;
				}

				if ( useInner ) {
					result = result.InnerException;
				} else {
					break; // All done
				}
			}

			return result;
		}

		public ErrorHandledResult HandleError( Exception Exception ) {

			Exception logException = this.GetTheRealException( Exception );

			string destViewName = "Error";
			int? errorId = this.logger.Log( ex: logException );
			if ( this.IsNotFoundException( logException ) ) {
				// No need to log a 404
				destViewName = "NotFound";
			} else {
				// Log 
				destViewName = "Error";
			}
			bool handled = this.ErrorIsHandled();
			return new ErrorHandledResult {
				Handled = handled,
				ViewName = destViewName,
				ErrorId = errorId
			};
		}

	}
}
