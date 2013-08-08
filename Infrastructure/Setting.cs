namespace BetaSigmaPhi.Infrastructure {
	using System;
	using System.Configuration;
	using BetaSigmaPhi.Library;

	public interface ISetting {
		HashType HashType { get; }
		int MaxLoginFailCount { get; }
		TimeSpan LoginFailWindow { get; }
		int MinSaltLength { get; }
		int MaxSaltLength { get; }
	}

	public class Setting : ISetting {
		public HashType HashType { get { return (HashType)Enum.Parse( typeof(HashType), ConfigurationManager.AppSettings["HashType"] ); } }
		public int MaxLoginFailCount { get { return ConfigurationManager.AppSettings["MaxLoginFailCount"].ToIntOrNull() ?? 5; } }
		public TimeSpan LoginFailWindow { get { return TimeSpan.FromMinutes( ConfigurationManager.AppSettings["LoginFailWindow_Minutes"].ToIntOrNull() ?? 360 ); } }
		public int MinSaltLength { get { return ConfigurationManager.AppSettings["MinSaltLength"].ToIntOrNull() ?? 32; } }
		public int MaxSaltLength { get { return ConfigurationManager.AppSettings["MaxSaltLength"].ToIntOrNull() ?? 64; } }
	}
}
