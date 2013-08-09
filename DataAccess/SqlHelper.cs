namespace BetaSigmaPhi.DataAccess {
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;

	public interface ISqlHelper {
		T ToValOrNull<T>( object Value );
		List<T> ExecuteQuery<T>( string Query, Func<IDataReader, T> Map, List<IDataParameter> Parameters = null, CommandType CommandType = CommandType.StoredProcedure );
		T ExecuteScalar<T>( string Query, List<IDataParameter> Parameters = null, CommandType CommandType = CommandType.StoredProcedure );
		int ExecuteNonQuery( string Query, List<IDataParameter> Parameters = null, CommandType CommandType = CommandType.StoredProcedure );
	}

	public class SqlHelper : ISqlHelper {
		private readonly IDbConnectionHelper dbConnectionHelper;

		public SqlHelper( IDbConnectionHelper DbConnectionHelper ) {
			if ( DbConnectionHelper == null ) {
				throw new ArgumentNullException( "DbConnectionHelper" );
			}
			this.dbConnectionHelper = DbConnectionHelper;
		}

		/// <summary>
		/// A cool helper method that makes <code>reader[&quot;column&quot;] != DBNull.Value ? reader[&quot;column&quot;] : (type)null;</code> more DRY: <code>this.sqlHelper.ToValOrNull&lt;type&gt;(reader[&quot;column&quot;])</code>
		/// </summary>
		public T ToValOrNull<T>( object Value ) {
			return Value == DBNull.Value ? default( T ) : (T)Value;
		}

		// Public to be testable, not intended for public consumption (thus not part of the interface)
		public void ExecuteQueryToFunc( string Query, Action<IDbCommand> Map, List<IDataParameter> Parameters = null, CommandType CommandType = CommandType.StoredProcedure ) {

			if ( Map == null ) {
				throw new ArgumentNullException( "Map" );
			}

			int retryCount = 3;

			while ( true ) {
				try {
					using ( IDbConnection conn = this.dbConnectionHelper.GetConnection() ) {
						using ( IDbCommand cmd = conn.CreateCommand() ) {
							cmd.CommandText = Query;
							cmd.CommandType = CommandType;
							if ( Parameters != null ) {
								foreach ( IDataParameter p in Parameters ) {
									if ( p.Value == null ) {
										p.Value = DBNull.Value;
									}
									cmd.Parameters.Add( p );
								}
							}
							bool open = cmd.Connection.State != ConnectionState.Closed;

							try {
								if ( !open ) {
									cmd.Connection.Open();
								}
								Map( cmd );
							} finally {
								cmd.Parameters.Clear(); // Until the GC runs, the old connection owns the passed in parameters and the retry won't work correctly
								if ( !open && cmd.Connection.State != ConnectionState.Closed ) {
									cmd.Connection.Close();
								}
							}
						}
					}

					break; // Completed successfully
				} catch ( DbException ex ) {
					if ( ex.Message.ToLowerInvariant().Contains( "deadlock" ) && retryCount > 0 ) {
						retryCount--;
					} else {
						throw;
					}
				}

				if ( Parameters != null ) {
					foreach ( IDataParameter p in Parameters ) {
						if ( p.Value == DBNull.Value ) {
							p.Value = null;
						}
					}
				}
			}

		}

		public List<T> ExecuteQuery<T>( string Query, Func<IDataReader, T> Map, List<IDataParameter> Parameters = null, CommandType CommandType = CommandType.StoredProcedure ) {
			List<T> results = new List<T>();
			this.ExecuteQueryToFunc( Query, dbCommand => {
				using ( IDataReader reader = dbCommand.ExecuteReader() ) {
					while ( reader.Read() ) {
						results.Add( Map( reader ) );
					}
				}
			}, Parameters, CommandType );
			return results;
		}

		public T ExecuteScalar<T>( string Query, List<IDataParameter> Parameters = null, CommandType CommandType = CommandType.StoredProcedure ) {
			object output = null;
			this.ExecuteQueryToFunc( Query, dbCommand => { output = dbCommand.ExecuteScalar(); }, Parameters, CommandType );

			T results;
			if ( output == null || output == DBNull.Value ) {
				results = default( T );
			} else if ( output is T ) {
				results = (T)output;
			} else {
				throw new ArgumentException( Query + " didn't return a " + typeof(T) + " or DBNull.Value, it returned " + output );
			}

			return results;
		}

		public int ExecuteNonQuery( string Query, List<IDataParameter> Parameters = null, CommandType CommandType = CommandType.StoredProcedure ) {
			int result = 0;
			this.ExecuteQueryToFunc( Query, dbCommand => {
				result = dbCommand.ExecuteNonQuery();
			}, Parameters, CommandType );
			return result;
		}

		/*
		public DataTable ExecuteQueryToDataTable( string Query, List<IDataParameter> Parameters = null, CommandType CommandType = CommandType.StoredProcedure ) {
			DataSet ds = new DataSet();
			ExecuteQueryToFunc( Query, sqlCommand => new GenericDbDataAdapter( (DbCommand)sqlCommand ).Fill( ds ), Parameters, CommandType );
			return ( ds == null || ds.Tables.Count < 1 ) ? null : ds.Tables[0];
		}
		*/

	}
}
