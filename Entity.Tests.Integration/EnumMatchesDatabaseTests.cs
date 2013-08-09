namespace BetaSigmaPhi.Entity.Tests.Integration {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data;
	using System.Linq;
	using System.Reflection;
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Entity.Helpers;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.TestsBase.Integration;
	using NUnit.Framework;

	[TestFixture]
	public class EnumMatchesDatabaseTests : IntegrationTestBase {

		/* FRAGILE: This test does too much:
		 * - [LookupTable] attribute exists
		 * - db table exists and matches structure (else sqlHelper errors)
		 * - enum and db names and values match 
		 * - enum and db have identical number of items
		 */
		[TestCaseSource( "AllMappedEnumTypes" )]
		public void TestEnumMatchesDb( Type EnumType ) {

			// Arrange
			LookupTableAttribute lookupTable = EnumType.GetCustomAttributes( typeof(LookupTableAttribute), inherit:true ).FirstOrDefault() as LookupTableAttribute;
			if ( lookupTable == null ) {
				Assert.Fail( EnumType.Name + " doesn't have a [LookupTable(..)] attribute" );
			}
			string query = string.Format(
				"SELECT {0}, {1} FROM {2}",
				lookupTable.ValueColumn, lookupTable.TextColumn, lookupTable.Table
			);
			string defineEnumQuery = string.Format(
				"SELECT {0} + ' = ' + CAST( {1} AS nvarchar(20) ) + ',' FROM {2} ORDER BY {1}",
				lookupTable.TextColumn, lookupTable.ValueColumn, lookupTable.Table
			);

			// Act
			Dictionary<int, string> enumValDict = Enum.GetValues( EnumType ).Cast<int>().ToDictionary( t => t, t => Enum.GetName( EnumType, t ) );

			ISqlHelper sqlHelper = ServiceLocator.GetService<ISqlHelper>();
			Dictionary<int, string> dbValDict = null;
			try {
				dbValDict = sqlHelper.ExecuteQuery<Tuple<int, string>>( query, CommandType: CommandType.Text, Map: r => new Tuple<int, string>( (int)r[0], (string)r[1] ) ).ToDictionary( i => i.Item1, i => i.Item2 );

			// Assert
			} catch ( Exception ex ) {
				Assert.Fail( string.Format(
					"Error getting lookup table details for {0}, Query: {1}, Exception: {2}\n{3}", 
					EnumType.Name, query, ex.Message, ex.StackTrace
				) );
			}

			Assert.That( enumValDict, Is.Not.Null );
			Assert.Greater( enumValDict.Count, 0 );

			Assert.That( dbValDict, Is.Not.Null );
			Assert.Greater( dbValDict.Count, 0 );

			foreach ( var entry in enumValDict ) {
				int enumVal = entry.Key;
				string enumString = entry.Value;
				Assert.That( dbValDict.ContainsKey( enumVal ), Is.True, string.Format( "{0} ({1}) is not found in the db, reset the enum by running {2}", enumVal, enumString, defineEnumQuery ) );
				string dbString = dbValDict[enumVal];
				Assert.That( dbString, Is.EqualTo( enumString ), string.Format( "The enum calls {0} {1}, but the db calls it {2}, reset the enum by running {3}", enumVal, enumString, dbString, defineEnumQuery ) );
			}

			Assert.That( dbValDict.Count, Is.EqualTo( enumValDict.Count ), string.Format( "The number of {0} in the database and in the enumeration don't match, reset the enum by running {1}", EnumType.Name, defineEnumQuery ) );

		}

		// FRAGILE: Copied from EntityPropertyTests

		public List<Type> AllMappedEnumTypes {
			get {
				return (
					from e in this.AllEnumProperties
					select e.PropertyType
				).Distinct().ToList();
			}
		}

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

		public List<PropertyInfo> AllProperties {
			get {
				List<PropertyInfo> properties = (
					from e in this.AllEntities
					from p in e.GetProperties( BindingFlags.Instance | BindingFlags.Public )
					select p
				).ToList();
				Assert.That( properties.Count, Is.GreaterThan( 0 ) );
				return properties;
			}
		}

		public List<PropertyInfo> AllMappedProperties {
			get {
				List<PropertyInfo> properties = (
					from p in this.AllProperties
					let n = p.GetCustomAttributes( typeof( NotMappedAttribute ), inherit:true ).FirstOrDefault()
					where n == null
					select p
				).ToList();
				// blank is ok
				return properties;
			}
		}

		public List<PropertyInfo> AllEnumProperties {
			get {
				List<PropertyInfo> properties = (
					from p in this.AllMappedProperties
					where p.PropertyType.IsEnum
					select p
				).ToList();
				// blank is ok
				return properties;
			}
		}

	}
}
