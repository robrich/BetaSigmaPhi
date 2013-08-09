namespace BetaSigmaPhi.Service {
	using System.Linq;
	using BetaSigmaPhi.Library;

	public interface IUrlService {
		string JoinUrl( params string[] PathSegments );
	}

	public class UrlService : IUrlService {

		public string JoinUrl( params string[] PathSegments ) {
			if ( PathSegments.IsNullOrEmpty() ) {
				return "";
			}
			bool startSlash = !string.IsNullOrEmpty( PathSegments[0] ) && PathSegments[0][0] == '/'; // First item starts with a slash, need to put it back on at the end
			string[] content = (
				from s in PathSegments
				let st = this.StripSlashes( s )
				where !string.IsNullOrEmpty( st )
				select st
			).ToArray();
			return (startSlash ? "/" : "") + string.Join( "/", content ).ToLowerInvariant();
		}

		// public for testing, not exposed on interface
		public string StripSlashes( string Source ) {
			string result = Source;
			while ( true ) {
				if ( string.IsNullOrEmpty( result ) ) {
					break; // You asked for nothing, you got it
				} else if ( result == "/" ) {
					result = null;
					break;
				} else if ( result[0] == '/' ) {
					result = result.Substring( 1 );
				} else if ( result[result.Length - 1] == '/' ) {
					result = result.Substring( 0, result.Length - 1 );
				} else {
					break; // Nothing to do
				}
			}
			return result;
		}

	}
}