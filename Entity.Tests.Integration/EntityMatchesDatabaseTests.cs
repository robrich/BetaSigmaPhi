namespace BetaSigmaPhi.Entity.Tests.Integration {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Reflection;
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.TestsBase.Integration;
	using Microsoft.SqlServer.Management.Common;
	using Microsoft.SqlServer.Management.Smo;
	using NUnit.Framework;

	[TestFixture]
	public class EntityMatchesDatabaseTests : IntegrationTestBase {

		public List<Type> AllEntities {
			get {
				Type ientity = typeof( IEntity );
				List<Type> allTypes = new List<Type>( ientity.Assembly.GetTypes() );
				List<Type> entities = (
					from t in allTypes
					where ientity.IsAssignableFrom( t )
					&& t.Name != ientity.Name
					&& t.IsClass && !t.IsAbstract
					select t
				).ToList();
				Assert.That( entities.Count, Is.GreaterThan( 0 ) );
				return entities;
			}
		}

		[Test]
		[TestCaseSource( "AllEntities" )]
		public void EntityMatchesDatabase( Type EntityType ) {

			// Arrange
			TableAndSchema tableName = this.GetDatabaseTableName( EntityType );
			List<PropertyInfo> props = this.GetProperties( EntityType );

			List<PropertyDesc> content = this.GetPropertyDesc( props );

			List<Tuple<PropertyDesc, ColumnDesc, string>> errors = new List<Tuple<PropertyDesc, ColumnDesc, string>>();

			// Act

			List<Column> dbinfo = this.GetTableFromDatabase( tableName );
			Assert.That( dbinfo, Is.Not.Null, string.Format( "{0}: db table {1}.{2} doesn't exist", EntityType.Name, tableName.Schema, tableName.Table ) );
			List<ColumnDesc> dbdata = this.GetColumnData( dbinfo );
			foreach ( PropertyDesc codeProp in content ) {
				ColumnDesc dbProp = this.GetColumn( codeProp.ColumnName, dbdata );

				// Assert

				if ( dbProp == null ) {
					errors.Add( new Tuple<PropertyDesc, ColumnDesc, string>( codeProp, dbProp, "missing from db" ) );
				} else {
					if ( !this.PropertyTypesMatch( dbProp.PropertyType, codeProp.PropertyType ) ) {
						errors.Add( new Tuple<PropertyDesc, ColumnDesc, string>( codeProp, dbProp, string.Format( "type mismatch: {0}, {1}", codeProp.PropertyType.Name, dbProp.PropertyType.Name ) ) );
					}
					if ( dbProp.Nullable != codeProp.Nullable ) {
						errors.Add( new Tuple<PropertyDesc, ColumnDesc, string>( codeProp, dbProp, string.Format( "null mismatch: {0}, {1}", codeProp.Nullable, dbProp.Nullable ) ) );
					}
					if ( codeProp.Length != null && codeProp.Length != dbProp.Length ) {
						errors.Add( new Tuple<PropertyDesc, ColumnDesc, string>( codeProp, dbProp, string.Format( "length mismatch: {0}, {1}", codeProp.Length, dbProp.Length ) ) );
					}
					if ( tableName.DbObjectType != DbObjectType.View ) {
						if ( codeProp.IsPrimaryKey != dbProp.IsPrimaryKey ) {
							errors.Add( new Tuple<PropertyDesc, ColumnDesc, string>( codeProp, dbProp, string.Format( "PrimaryKey mismatch: {0}, {1}", codeProp.IsPrimaryKey, dbProp.IsPrimaryKey ) ) );
						}
					} else {
						// FRAGILE: Can't validate the primary key, views need schema binding to have an index (like a primary key), and to have schema binding, all dependent objects must be tables or views with schema binding
					}
				}

			}

			Assert.That( errors.Count, Is.EqualTo( 0 ), string.Format( 
				"{0} doesn't match {1}: {2}", 
				EntityType.Name, tableName, string.Join( ", ", (from p in errors select p.Item1.PropertyName + ": " + p.Item3).ToList() ) 
			) );
		}

		[Ignore]
		private class PropertyDesc {
			public string PropertyName { get; set; }
			public Type PropertyType { get; set; }
			public string ColumnName { get; set; }
			public int? Length { get; set; }
			public bool Nullable { get; set; }
			public bool IsPrimaryKey { get; set; }
		}

		[Ignore]
		private class TableAndSchema {
			public string Table { get; set; }
			public string Schema { get; set; }
			public DbObjectType DbObjectType { get; set; }
			public override string ToString() {
				return this.Schema + "." + this.Table;
			}
			public override int GetHashCode() {
				return this.ToString().GetHashCode();
			}
		}
		private enum DbObjectType {
			Table,
			View
		}

		private TableAndSchema GetDatabaseTableName( Type EntityType ) {

			string table;
			string schema;

			TableAttribute tableAttr = EntityType.GetCustomAttributes( typeof( TableAttribute ), inherit:true ).FirstOrDefault() as TableAttribute;
			if ( tableAttr != null ) {
				schema = tableAttr.Schema;
				if ( string.IsNullOrEmpty( schema ) ) {
					schema = "dbo";
				}
				table = tableAttr.Name;
			} else {
				table = EntityType.Name;
				schema = "dbo";
			}

			return new TableAndSchema {
				Table = table,
				Schema = schema
			};
		}

		private List<PropertyInfo> GetProperties( Type EntityType ) {
			List<PropertyInfo> properties = new List<PropertyInfo>( EntityType.GetProperties( BindingFlags.Instance | BindingFlags.Public ) );
			// Remove [NotMapped]
			properties = (
				from p in properties
				let nm = p.GetCustomAttributes( typeof( NotMappedAttribute ), inherit:true ).FirstOrDefault()
				where nm == null
				select p
			).ToList();
			// Remove navigation properties: IEntity or List<IEntity>
			properties = (
				from p in properties
				where !typeof( IEntity ).IsAssignableFrom( p.PropertyType )
				&& !(
					p.PropertyType.IsGenericType
					&& p.PropertyType.GetGenericTypeDefinition() == typeof( List<> )
					&& p.PropertyType.GetGenericArguments().Length == 1
					&& typeof(IEntity).IsAssignableFrom(p.PropertyType.GetGenericArguments()[0])
				)
				select p
			).ToList();
			Assert.That( properties.Count, Is.GreaterThan( 0 ) );
			return properties;
		}

		private List<PropertyDesc> GetPropertyDesc( List<PropertyInfo> Props ) {
			List<PropertyDesc> results = new List<PropertyDesc>();
			foreach ( PropertyInfo prop in Props ) {
				PropertyDesc desc = new PropertyDesc {
					PropertyName = prop.Name,
					PropertyType = prop.PropertyType,
					ColumnName = this.GetColumnName( prop ),
					Length = this.GetColumnLength( prop )
				};

				desc.IsPrimaryKey = prop.GetCustomAttributes(typeof(KeyAttribute), inherit: true).FirstOrDefault() != null;

				desc.Nullable = !typeof(ValueType).IsAssignableFrom( prop.PropertyType ) || prop.PropertyType == typeof(char);
				if ( prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ) {
					Type[] genericArgs = prop.PropertyType.GetGenericArguments();
					if ( genericArgs != null && genericArgs.Length == 1 ) {
						desc.PropertyType = genericArgs[0];
						desc.Nullable = true;
					}
				}

				bool hasTimestamp = prop.GetCustomAttributes( typeof( TimestampAttribute ), inherit:true ).FirstOrDefault() != null;
				if ( hasTimestamp ) {
					desc.Nullable = false; // The entity is nullable, but the db isn't so "fake" like the entity isn't either
				}

				if ( this.HasRequiredAttribute( prop ) ) {
					desc.Nullable = false;
				}

				results.Add( desc );
			}
			return results;
		}

		private string GetColumnName( PropertyInfo Property ) {

			string columnName = Property.Name;

			ColumnAttribute columnAttr = Property.GetCustomAttributes( typeof( ColumnAttribute ), inherit:true ).FirstOrDefault() as ColumnAttribute;
			if ( columnAttr != null ) {
				columnName = columnAttr.Name;
			}

			return columnName;
		}

		private int? GetColumnLength( PropertyInfo Property ) {

			int? length = null;

			StringLengthAttribute lengthAttr = Property.GetCustomAttributes( typeof( StringLengthAttribute ), inherit:true ).FirstOrDefault() as StringLengthAttribute;
			if ( lengthAttr != null ) {
				length = lengthAttr.MaximumLength;
			}

			return length;
		}

		private bool HasRequiredAttribute( PropertyInfo Property ) {
			RequiredAttribute requiredAttribute = Property.GetCustomAttributes( typeof( RequiredAttribute ), inherit:true ).FirstOrDefault() as RequiredAttribute;
			return requiredAttribute != null;
		}

		private List<Column> GetTableFromDatabase( TableAndSchema TableAndSchema ) {

			IDbConnectionHelper connectionHelper = ServiceLocator.GetService<IDbConnectionHelper>();

			string connstr = connectionHelper.GetConnection().ConnectionString;

			SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder(connstr);
			string databaseName = connBuilder.InitialCatalog;
			if ( string.IsNullOrEmpty( databaseName ) ) {
				throw new ArgumentNullException( "ConnectionString doesn't contain InitialCatalog" );
			}

			using ( SqlConnection conn = new SqlConnection( connstr ) ) {
				ServerConnection serverConnection = new ServerConnection( conn );
				Server server = new Server( serverConnection );

				if ( !server.Databases.Contains( databaseName ) ) {
					throw new ArgumentException( databaseName + " does not exist in " + connBuilder.DataSource );
				}
				Database database = server.Databases[databaseName];

				ColumnCollection table = (
					from Table t in database.Tables
					where string.Equals( t.Name, TableAndSchema.Table, StringComparison.InvariantCultureIgnoreCase )
					&& string.Equals( t.Schema, TableAndSchema.Schema, StringComparison.InvariantCultureIgnoreCase )
					select t.Columns
				).FirstOrDefault();

				// FRAGILE: We're mis-using a view as if it was a table
				ColumnCollection view = (
					from View t in database.Views
					where string.Equals( t.Name, TableAndSchema.Table, StringComparison.InvariantCultureIgnoreCase )
					&& string.Equals( t.Schema, TableAndSchema.Schema, StringComparison.InvariantCultureIgnoreCase )
					select t.Columns
				).FirstOrDefault();

				List<Column> columns = new List<Column>();
				if ( table != null && table.Count > 0 ) {
					TableAndSchema.DbObjectType = DbObjectType.Table;
					columns.AddRange( table.Cast<Column>() );
				} else if ( view != null && view.Count > 0 ) {
					TableAndSchema.DbObjectType = DbObjectType.View;
					columns.AddRange( view.Cast<Column>() );
				} else {
					columns = null; // Table doesn't exist
				}
				return columns;
			}
		}

		private List<ColumnDesc> GetColumnData( List<Column> Columns ) {
			List<ColumnDesc> results = new List<ColumnDesc>();
			foreach ( Column column in Columns ) {
				ColumnDesc result = new ColumnDesc();
				result.Name = column.Name;
				result.Nullable = column.Nullable;
				result.SqlDataType = column.DataType.SqlDataType;
				result.PropertyType = this.GetTypeFromSqlType( column.DataType.SqlDataType );
				result.Length = column.DataType.MaximumLength;
				result.IsPrimaryKey = column.InPrimaryKey;
				results.Add( result );
			}
			return results;
		}

		private ColumnDesc GetColumn( string ColumnName, List<ColumnDesc> Columns ) {
			return (
				from c in Columns
				where string.Equals( c.Name, ColumnName, StringComparison.InvariantCultureIgnoreCase ) // SQL Server is case insensitive
				select c
			).FirstOrDefault();
		}

		[Ignore]
		private class ColumnDesc {
			public string Name { get; set; }
			public Microsoft.SqlServer.Management.Smo.SqlDataType SqlDataType { get; set; }
			public Type PropertyType { get; set; }
			public int? Length { get; set; }
			public bool Nullable { get; set; }
			public bool IsPrimaryKey { get; set; }
		}

		// http://msdn.microsoft.com/en-us/library/bb386947.aspx
		private Type GetTypeFromSqlType( SqlDataType SqlDataType ) {
			Type result;
			switch ( SqlDataType ) {
				case SqlDataType.None:
				case SqlDataType.Image:
				case SqlDataType.Real:
				case SqlDataType.UserDefinedDataType:
				case SqlDataType.UserDefinedType:
				case SqlDataType.Variant:
				case SqlDataType.SysName:
				case SqlDataType.UserDefinedTableType:
				case SqlDataType.HierarchyId:
				case SqlDataType.Geometry:
				case SqlDataType.Geography:
					// Unsupported
					throw new ArgumentOutOfRangeException( SqlDataType + " is unsupported" );
				case SqlDataType.BigInt:
					result = typeof(long);
					break;
				case SqlDataType.Binary:
					result = typeof(byte[]);
					break;
				case SqlDataType.Bit:
					result = typeof(bool);
					break;
				case SqlDataType.Char:
					result = typeof(char); // Though typeof(string) is also acceptable
					break;
				case SqlDataType.DateTime:
				case SqlDataType.Date:
				case SqlDataType.Time:
				case SqlDataType.DateTimeOffset:
				case SqlDataType.DateTime2:
					result = typeof(DateTime);
					break;
				case SqlDataType.Decimal:
					result = typeof(decimal);
					break;
				case SqlDataType.Float:
					result = typeof(double);
					break;
				case SqlDataType.Int:
					result = typeof(int);
					break;
				case SqlDataType.Money:
					result = typeof(decimal);
					break;
				case SqlDataType.NChar:
				case SqlDataType.NText:
				case SqlDataType.NVarChar:
				case SqlDataType.NVarCharMax:
					result = typeof(string);
					break;
				case SqlDataType.Numeric:
					result = typeof(decimal);
					break;
				case SqlDataType.SmallDateTime:
					result = typeof(DateTime);
					break;
				case SqlDataType.SmallInt:
					result = typeof(Int16);
					break;
				case SqlDataType.SmallMoney:
					result = typeof(decimal);
					break;
				case SqlDataType.Text:
					result = typeof(string);
					break;
				case SqlDataType.Timestamp:
					result = typeof(byte[]);
					break;
				case SqlDataType.TinyInt:
					result = typeof(sbyte);
					break;
				case SqlDataType.UniqueIdentifier:
					result = typeof(Guid);
					break;
				case SqlDataType.VarBinary:
				case SqlDataType.VarBinaryMax:
					result = typeof(byte[]);
					break;
				case SqlDataType.VarChar:
				case SqlDataType.VarCharMax:
					result = typeof(string);
					break;
				case SqlDataType.Xml:
					result = typeof(string);
					break;
				default:
					throw new ArgumentOutOfRangeException( "SqlDataType" );
			}

			return result;
		}

		private bool PropertyTypesMatch( Type DbType, Type CodeType ) {
			bool result = false;
			if ( DbType == CodeType ) {
				result = true;
			} else if ( DbType == typeof(char) && CodeType == typeof(string) ) {
				result = true; // This is ok too
			} else if ( DbType == typeof(int) && CodeType.IsEnum ) {
				result = true; // This is ok too
			} else {
				result = false;
			}
			return result;
		}

	}
}
