namespace BetaSigmaPhi.Entity.Tests {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using System.Reflection;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Tests.Base;
	using NUnit.Framework;

	[TestFixture]
	public class EntityPropertyTests : TestBase {

		[Test]
		[TestCaseSource( "AllPropertiesWithRequired" )]
		public void RequiredOnlyOnStringTypes( PropertyInfo Property ) {

			// Arrange

			// Act
			bool isStringType = Property.PropertyType == typeof(string);

			// Assert
			Assert.That( isStringType, Is.True, Property.DeclaringType.Name+"."+Property.Name + " is marked as [Required] but it isn't a string" );
		}

		[Ignore("Until we really hammer the schema, there are many things that end in ID")]
		[Test]
		[TestCaseSource( "AllProperties" )]
		public void DoesNotEndInID( PropertyInfo Property ) {

			// Arrange
			string propertyName = Property.Name;
			Assert.That( propertyName, Is.Not.Null );

			// Act
			bool endsInID = propertyName.EndsWith( "ID" );

			// Assert
			Assert.That( endsInID, Is.False, Property.DeclaringType.Name + "." + Property.Name + " ends with \"ID\", change this to be ...Id." );
		}

		[Test]
		[TestCaseSource( "AllPropertiesWithColumn" )]
		public void ColumnAttrDoesntMatchProperty( PropertyInfo Property ) {

			// Arrange
			string propertyName = Property.Name;
			ColumnAttribute colAttr = Property.GetCustomAttributes( typeof( ColumnAttribute ), inherit:true ).FirstOrDefault() as ColumnAttribute;
			if ( colAttr == null ) {
				throw new ArgumentNullException( "ColumnAttribute", Property.Name + " doesn't have a [Column] attribute" );
			}
			string colName = colAttr.Name;

			// Act
			bool nameIsIdentical = string.Equals( propertyName, colName, StringComparison.InvariantCultureIgnoreCase );

			// Assert
			Assert.That( nameIsIdentical, Is.False, Property.DeclaringType.Name + "." + Property.Name + " has a [Column] attribute with an identical name, the Column attribute isn't needed if the column follows convention" );
		}

		[Test]
		[TestCaseSource( "AllEnumProperties" )]
		public void EnumPropertyHasColumnAttribute( PropertyInfo Property ) {

			// Arrange

			// Act
			bool hasColumnAttr = Property.GetCustomAttributes( typeof( ColumnAttribute ), inherit:true ).FirstOrDefault() != null;
			bool propertyEndsInId = Property.Name.EndsWith( "Id" );

			// Assert
			Assert.That( hasColumnAttr || propertyEndsInId, Is.True, Property.DeclaringType.Name + "." + Property.Name + " is an enum without a [Column] attribute, please add [Column(\"" + Property.Name + "Id\")] so this enum can more easily link to the backing table" );
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

		public List<PropertyInfo> AllPropertiesWithColumn {
			get {
				List<PropertyInfo> properties = (
					from p in this.AllProperties
					let c = p.GetCustomAttributes( typeof( ColumnAttribute ), inherit:true ).FirstOrDefault()
					where c != null
					select p
				).ToList();
				// blank is ok
				return properties;
			}
		}

		public List<PropertyInfo> AllPropertiesWithRequired {
			get {
				List<PropertyInfo> properties = (
					from p in this.AllProperties
					let r = p.GetCustomAttributes( typeof( RequiredAttribute ), inherit:true ).FirstOrDefault()
					where r != null
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
