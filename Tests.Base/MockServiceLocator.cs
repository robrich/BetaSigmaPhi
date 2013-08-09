namespace BetaSigmaPhi.Tests.Base {
	using System;
	using Moq;
	using Ninject;
	using Ninject.MockingKernel.Moq;
	using NUnit.Framework;

	public interface IMockServiceLocator {
		T Get<T>() where T : class;
		Mock<T> GetMock<T>( MockBehavior MockBehavior = MockBehavior.Strict ) where T : class;
		void VerifyAll();
		void Bind<TInterface, TInstance>( TInstance Instance ) where TInstance : TInterface;
	}

	[Ignore]
	public class MockServiceLocator : IMockServiceLocator {
		private readonly MoqMockingKernel kernel;
		private readonly MockBehavior defaultMockBehavior;

		public MockServiceLocator( MoqMockingKernel Kernel, MockBehavior DefaultMockBehavior ) {
			if ( Kernel == null ) {
				throw new ArgumentNullException( "Kernel" );
			}
			this.kernel = Kernel;
			this.defaultMockBehavior = DefaultMockBehavior;
		}

		public T Get<T>() where T : class {
			Type t = typeof(T);
			// If you're asking for a real class, it better be a real class
			Assert.That( t.IsInterface, Is.False );
			Assert.That( t.IsAbstract, Is.False );
			return this.kernel.Get<T>();
		}

		public Mock<T> GetMock<T>( MockBehavior MockBehavior = MockBehavior.Default ) where T : class {
			Type t = typeof(T);
			// If you're asking for a mock, it better be an interface
			Assert.That( t.IsInterface, Is.True );
			if ( MockBehavior != this.defaultMockBehavior ) {
				// FRAGILE: .VerifyAll() won't verify this mock
				Mock<T> instance = new Mock<T>( MockBehavior );
				this.kernel.Rebind<T>().ToConstant( instance.Object );
				return instance;
			} else {
				return this.kernel.GetMock<T>();
			}
		}

		public void VerifyAll() {
			this.kernel.MockRepository.VerifyAll();
		}

		public void Bind<TInterface, TInstance>( TInstance Instance ) where TInstance : TInterface {
			this.kernel.Bind<TInterface>().ToMethod<TInstance>( _ => Instance );
		}
		
	}
}