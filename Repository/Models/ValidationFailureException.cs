namespace BetaSigmaPhi.Repository.Models {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ValidationFailureException : Exception {
		public List<ValidationFailure> ValidationFailures { get; private set; }

		public ValidationFailureException( string Message, List<ValidationFailure> ValidationFailures, Exception ex )
			: base( Message + ":\n" + string.Join( "\n", (
				from f in ValidationFailures ?? new List<ValidationFailure>()
				select f.PropertyName + ": " + f.ErrorMessage
			) ), ex ) {
			this.ValidationFailures = ValidationFailures ?? new List<ValidationFailure>();
		}

	}
}
