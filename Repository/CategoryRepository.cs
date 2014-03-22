namespace BetaSigmaPhi.Repository {
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;

	public interface ICategoryRepository : IRepository<Category> {

	}

	public class CategoryRepository : Repository<Category>, ICategoryRepository {

		public CategoryRepository(IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory) : base(BetaSigmaPhiContextFactory) {
		}

	}
}
