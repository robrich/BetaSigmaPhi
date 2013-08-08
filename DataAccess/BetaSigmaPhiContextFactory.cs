namespace BetaSigmaPhi.DataAccess {
	using BetaSigmaPhi.Infrastructure;

	public interface IBetaSigmaPhiContextFactory {
		IBetaSigmaPhiContext GetContext();
	}

	public class BetaSigmaPhiContextFactory : IBetaSigmaPhiContextFactory {

		public IBetaSigmaPhiContext GetContext() {
			return ServiceLocator.GetService<IBetaSigmaPhiContext>();
		}
	}
}
