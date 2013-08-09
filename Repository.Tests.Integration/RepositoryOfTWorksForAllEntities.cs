namespace BetaSigmaPhi.Repository.Tests.Integration {
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.TestsBase.Integration;
	using Ninject;
	using NUnit.Framework;
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Reflection;

	[TestFixture]
	public class RepositoryOfTWorksForAllEntities : IntegrationTestBase {

		private IKernel kernel;

		[Ignore]
		protected override void InitializeOtherTypes(IKernel Kernel) {
			kernel = Kernel;
		}

		[Test]
		[TestCaseSource( "AllEntities" )]
		public void GetByIdWorks( Type TEntity ) {

			// Arrange
			int id = 1; // Something greater than 0 so it'll actually make the db call
			Type repositoryType = typeof( TestRepository<> ).MakeGenericType( TEntity );

			// Have to use IKernel instead of ServiceLocator because ServiceLocator only has GetService<T> not GetService(Type) and we don't have the type at compile time
			var instance = kernel.GetService( repositoryType ); // The type under test
			MethodInfo method = repositoryType.GetMethod( "GetById" );
			Assert.That( instance, Is.Not.Null );
			Assert.That( method, Is.Not.Null );

			// Act
			IEntity entity = method.Invoke( instance, new object[] {id} ) as IEntity;

			// Assert
			// If we're still here, it worked
		}

		public List<Type> AllEntities {
			get {
				Type contextType = typeof(BetaSigmaPhiContext);
				List<PropertyInfo> properties = contextType.GetProperties().ToList();
				List<Type> entities = (
					from p in properties
					let t = p.PropertyType
					where t.IsGenericType
					&& t.GetGenericTypeDefinition() == typeof(IDbSet<>)
					let earr = t.GetGenericArguments()
					where earr != null
					&& earr.Length == 1
					select earr[0]
				).ToList();
				return entities;
			}
		}
		
		/// <summary>
		/// Because Repository is abstract, we'll create a concrete instance of it
		/// </summary>
		[Ignore]
		public class TestRepository<TEntity> : Repository<TEntity> where TEntity : IEntity {
			public TestRepository( IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory )
				: base( BetaSigmaPhiContextFactory ) {
			}
		}

	}
}
