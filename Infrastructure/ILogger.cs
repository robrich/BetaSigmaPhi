namespace BetaSigmaPhi.Infrastructure {
	using System;

	public interface ILogger {
		int? Log( string Message, Exception ex = null );
		int? Log( Exception ex );
	}
}
