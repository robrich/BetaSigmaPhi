namespace BetaSigmaPhi.Tests.Base {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using NUnit.Framework;

	/*
	Validates that:
	- All public classes are unit tests and derive from TestBase (or else they have [Ignore] on them)
	- All public methods in these classes have a valid unit test attribute (test, testcase, setup, etc)
	*/

	[TestFixture]
	public abstract partial class TestTheTests {

		[Test]
		[TestCaseSource( "AllClasses" )]
		public void AllClassesAreTestFixtures( Type type ) {

			string typeName = type.FullName;

			object[] attributes = type.GetCustomAttributes( inherit: true );
			Assert.That( attributes, Is.Not.Null, typeName + " has no attributes, so it isn't a TestFixture" );
			Assert.That( attributes.Length, Is.GreaterThan( 0 ), typeName + " has no attributes, so it isn't a TestFixture" );

			bool hasTestFixtureAttribute = attributes.OfType<TestFixtureAttribute>().Any();
			Assert.That( hasTestFixtureAttribute, Is.True, typeName + " doesn't have a TestFixture attribute" );
		}

		[Test]
		[TestCaseSource( "AllClasses" )]
		public void AllClassesDeriveFromTestBase( Type type ) {

			if ( type.IsNested ) {
				// Make them private instead
				// continue;
			}

			Type baseTestType = this.BaseTestType;
			string typeName = type.FullName;

			Assert.That( baseTestType.IsAssignableFrom( type ), Is.True, typeName + " doesn't derive from " + baseTestType.Name );
		}
		
		[Test]
		[TestCaseSource( "AllClasses" )]
		public void AllClassesHaveMethods( Type type ) {

			List<MethodInfo> methods = (
				from m in type.GetMethods( BindingFlags.Public | BindingFlags.Instance )
				where !this.objectMethods.Contains( m.Name ) // Object methods are exempt (e.g. .ToString())
				&& !m.IsSpecialName // Like properties that have get_* and set_* methods
				select m
			).ToList();

			string typeName = type.FullName;

			Assert.That( methods, Is.Not.Null );
			Assert.That( methods.Count, Is.GreaterThan( 0 ), typeName + " doesn't have any methods" );
		}

		[Test]
		[TestCaseSource( "AllMethodsOnAllClasses" )]
		public void AllMethodsAreTests( MethodInfo method ) {

			string methodName = method.DeclaringType.FullName + "." + method.Name + "()";

			var attributes = method.GetCustomAttributes( true );
			Assert.That( attributes, Is.Not.Null, methodName + " has no attributes, so it isn't a Test" );
			Assert.That( attributes.Length, Is.GreaterThan( 0 ), methodName + " has no attributes, so it isn't a Test" );

			bool hasTestAttribute = attributes.Any( attribute =>
				attribute is TestAttribute || attribute is TestCaseAttribute || attribute is TestCaseSourceAttribute || 
				attribute is SetUpAttribute || attribute is TearDownAttribute || attribute is SetUpFixtureAttribute || attribute is TestFixtureTearDownAttribute ||
				attribute is TestFixtureSetUpAttribute || attribute is IgnoreAttribute
			);
			Assert.That( hasTestAttribute, Is.True, methodName + " doesn't have a Test attribute" );
		}

		[Test]
		public void ThisTypeIsNotTestTheTests() {
			Assert.That( this.ThisType, Is.Not.SameAs( typeof( TestTheTests ) ) );
		}

		// in a derived class, this.GetType() will be the derived type
		// but unfortunately this.GetType().Assembly isn't the derived type's assembly
		// but fortunately Assembly.GetAssembly( this.GetType() ) is the derived type's assembly
		private Type ThisType {
			get { return this.GetType(); }
		}

		private Assembly ThisTypeAssembly {
			get { return Assembly.GetAssembly( this.ThisType ); }
		}

		private List<Type> allClassesInThisTypeAssemblyAndTestsBaseAssembly {
			get {
				List<Type> allTypes = new List<Type>();
				Type objectType = typeof(object);
				Type typeStep = this.ThisType;
				while ( typeStep != null && typeStep != objectType ) {
					List<Type> typesFromAssembly = Assembly.GetAssembly( typeStep ).GetExportedTypes().ToList();
					allTypes.AddRange( typesFromAssembly );
					typeStep = typeStep.BaseType;
				}
				return allTypes.Distinct().ToList();
			}
		}

		public List<Type> AllClasses {
			get {
				List<Type> all = (
					from t in this.allClassesInThisTypeAssemblyAndTestsBaseAssembly
					where t.IsClass
					&& !t.IsNotPublic
					&& !t.IsAbstract
					&& !t.IsInterface
					select t
				).ToList();
				List<Type> notIgnored = (
					from t in all
					where !( t.GetCustomAttributes( inherit: true ).OfType<IgnoreAttribute>() ).Any()
					select t
				).ToList();
				Assert.That( notIgnored.Count, Is.GreaterThan( 0 ) );
				return notIgnored;
			}
		}

		public List<MethodInfo> AllMethodsOnAllClasses {
			get {
				return (
					from t in this.AllClasses
					from m in t.GetMethods( BindingFlags.Public | BindingFlags.Instance )
					where !this.objectMethods.Contains( m.Name ) // Object methods are exempt (e.g. .ToString())
					&& !m.IsSpecialName // Like properties that have get_* and set_* methods
					select m
				).ToList();
			}
		}

		private readonly List<string> objectMethods = typeof(object).GetMethods( BindingFlags.Public | BindingFlags.Instance ).Select( m => m.Name ).ToList();

	}
}
