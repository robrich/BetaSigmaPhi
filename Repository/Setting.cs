namespace BetaSigmaPhi.Repository {
	using System;
	using System.Configuration;
	using System.Text.RegularExpressions;
	using System.Web;
	using System.Web.Hosting;
	using BetaSigmaPhi.Library;

	public interface ISettingRepository {
		HashType HashType { get; }
		int MaxLoginFailCount { get; }
		TimeSpan LoginFailWindow { get; }
		int MinSaltLength { get; }
		int MaxSaltLength { get; }
		bool SendErrorEmails { get; }
		string ErrorEmailAddress { get; }
		string SiteUrl { get; }
	}

	public class SettingRepository : ISettingRepository {
		public HashType HashType { get { return (HashType)Enum.Parse( typeof(HashType), ConfigurationManager.AppSettings["HashType"] ); } }
		public int MaxLoginFailCount { get { return ConfigurationManager.AppSettings["MaxLoginFailCount"].ToIntOrNull() ?? 5; } }
		public TimeSpan LoginFailWindow { get { return TimeSpan.FromMinutes( ConfigurationManager.AppSettings["LoginFailWindow_Minutes"].ToIntOrNull() ?? 360 ); } }
		public int MinSaltLength { get { return ConfigurationManager.AppSettings["MinSaltLength"].ToIntOrNull() ?? 32; } }
		public int MaxSaltLength { get { return ConfigurationManager.AppSettings["MaxSaltLength"].ToIntOrNull() ?? 64; } }
		public bool SendErrorEmails { get { return ConfigurationManager.AppSettings["SendErrorEmails"].ToBoolOrNull() ?? false; } }
		public string ErrorEmailAddress { get { return ConfigurationManager.AppSettings["ErrorEmailAddress"]; } }
		public string SiteUrl {
			get {
				if (!HostingEnvironment.IsHosted) {
					throw new ArgumentNullException("SiteUrl", "SiteUrl is null because HostingEnvironment.IsHosted is false");
				}
				if (HttpContext.Current == null) {
					throw new ArgumentNullException("HttpContext.Current", "SiteUrl is null because we're not in a web request");
				}
				string authority = HttpContext.Current.Request.Url.Authority;
				if (Regex.IsMatch(authority, @"^([^\.]+)\.com$")) {
					// Needs www on front
					authority = "www." + authority;
				}
				return "http" + "://" + authority;
			}
		}

	}
}
