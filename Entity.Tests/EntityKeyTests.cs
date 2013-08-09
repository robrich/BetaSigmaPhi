namespace BetaSigmaPhi.Entity.Tests {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Reflection;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Tests.Base;
	using NUnit.Framework;

	[TestFixture]
	public class EntityKeyTests : TestBase {

		[Test]
		[TestCaseSource( "AllEntities" )]
		public void PrimaryKeyDefined( Type EntityType ) {

			// Act
			PropertyInfo primaryKey = this.GetPrimaryKey( EntityType );

			// Assert
			Assert.That( primaryKey, Is.Not.Null, EntityType.Name + " doesn't have a [Key] attribute on any properties" );
		}

		[Test]
		[TestCaseSource( "AllEntities" )]
		public void PrimaryKeyOverloadsIdGet( Type EntityType ) {

			// Arrange
			IEntity instance = Activator.CreateInstance( EntityType ) as IEntity;
			PropertyInfo primaryKey = this.GetPrimaryKey( EntityType );
			Assert.That( primaryKey, Is.Not.Null ); // Technically not part of this test, but the rest of the test blows up without this
			int expectedId = 280; // Value isn't important, non-zero is

			// Act
			instance.Id = 280;
			int? actualId = primaryKey.GetValue( instance, null ) as int?;

			// Assert
			Assert.That( actualId, Is.Not.Null, EntityType.Name + "'s "+primaryKey.Name+" property should be of the form { get { return this.Key; } set { this.Key = value } }" );
			Assert.That( actualId, Is.EqualTo( expectedId ), EntityType.Name + "'s " + primaryKey.Name + " property should be of the form { get { return this.Key; } set { this.Key = value } }" );
		}

		[Test]
		[TestCaseSource( "AllEntities" )]
		public void PrimaryKeyOverloadsIdSet( Type EntityType ) {

			// Arrange
			IEntity instance = Activator.CreateInstance( EntityType ) as IEntity;
			PropertyInfo primaryKey = this.GetPrimaryKey( EntityType );
			Assert.That( primaryKey, Is.Not.Null ); // Technically not part of this test, but the rest of the test blows up without this
			int expectedId = 280; // Value isn't important, non-zero is

			// Act
			primaryKey.SetValue( instance, expectedId, null );
			int? actualId = instance.Id;
				
			// Assert
			Assert.That( actualId, Is.Not.Null, EntityType.Name + "'s " + primaryKey.Name + " property should be of the form { get { return this.Key; } set { this.Key = value } }" );
			Assert.That( actualId, Is.EqualTo( expectedId ), EntityType.Name + "'s " + primaryKey.Name + " property should be of the form { get { return this.Key; } set { this.Key = value } }" );
		}

		private PropertyInfo GetPrimaryKey( Type EntityType ) {
			List<PropertyInfo> properties = new List<PropertyInfo>( EntityType.GetProperties( BindingFlags.Instance | BindingFlags.Public ) );

			List<PropertyInfo> primaryKeys = (
				from p in properties
				let a = p.GetCustomAttributes( typeof( KeyAttribute ), inherit: true ).FirstOrDefault()
				where a != null
				select p
			).ToList();

			// Other portions validate there's only 1, this need only validate there aren't more than one
			Assert.That( primaryKeys.Count, Is.LessThan( 2 ) );

			return primaryKeys.FirstOrDefault();
		}

		public List<Type> AllEntities {
			get {
				Type ientity = typeof(IEntity);
				List<Type> allTypes = new List<Type>( ientity.Assembly.GetTypes() );
				List<Type> entities = (
					from t in allTypes
					where ientity.IsAssignableFrom(t)
					&& t.Name != ientity.Name
					&& t.IsClass && !t.IsAbstract
					select t
				).ToList();
				return entities;
			}
		}

	}
}
