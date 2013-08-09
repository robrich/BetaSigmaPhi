namespace BetaSigmaPhi.DataAccess {
	using System;
	using System.Configuration;
	using System.Data;
	using System.Data.Common;

	public interface IDbConnectionHelper {
		IDbConnection GetConnection();
	}

	public class DbConnectionHelper : IDbConnectionHelper {

		public IDbConnection GetConnection() {

			ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings["DefaultConnection"]; // FRAGILE: The name of the connectionString in web.connection.config
			if ( connObj == null ) {
				throw new ArgumentNullException( "ConnectionString", "ConnectionString is null" );
			}
			string connstr = connObj.ConnectionString;
			if ( string.IsNullOrEmpty( connstr ) ) {
				throw new ArgumentNullException( "ConnectionString", "ConnectionString is null" );
			}

			DbProviderFactory factory = DbProviderFactories.GetFactory( connObj.ProviderName );
			DbConnection conn = factory.CreateConnection();
			conn.ConnectionString = connstr;

			return conn;
		}

	}
}
