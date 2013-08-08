namespace BetaSigmaPhi.Library {
	using System;

	public static class TypeHelpers {

		public static int? ToIntOrNull( this string source ) {
			int results = 0;
			if ( !int.TryParse( source, out results ) ) {
				return null;
			} else {
				return results;
			}
		}

		public static long? ToLongOrNull( this string source ) {
			long results = 0;
			if ( !long.TryParse( source, out results ) ) {
				return null;
			} else {
				return results;
			}
		}

		public static DateTime? ToDateTimeOrNull( this string source ) {
			DateTime results;
			if ( !DateTime.TryParse( source, out results ) ) {
				return null;
			} else {
				return results;
			}
		}

		public static double? ToDoubleOrNull( this string source ) {
			double results;
			if ( !double.TryParse( source, out results ) ) {
				return null;
			} else {
				return results;
			}
		}

		public static bool? ToBoolOrNull( this string source ) {
			bool results = false;
			if ( !bool.TryParse( source, out results ) ) {
				return null;
			} else {
				return results;
			}
		}

		public static string ToStringOrNull( this string source ) {
			if ( !string.IsNullOrEmpty( source ) ) {
				return source;
			} else {
				return null;
			}
		}

		public static Guid? ToGuidOrNull( this string source ) {
			Guid results = Guid.Empty;
			if ( !Guid.TryParse( ( source ?? "" ).Replace( @"""", "" ), out results ) ) {
				return null;
			} else {
				return results;
			}
		}

	}
}
