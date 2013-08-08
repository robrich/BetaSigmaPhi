namespace BetaSigmaPhi.Repository {
	using System;
	using System.Collections.Generic;
	using System.Data.Entity.Validation;
	using System.Linq;
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Repository.Models;

	public interface IRepository<TEntity> where TEntity : IEntity {
		int Save( TEntity Entity );
		bool Any();
		List<TEntity> GetAll();
		List<TEntity> GetActive();
		TEntity GetById( int Id );
	}

	public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : IEntity {
		protected readonly IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory;

		protected Repository( IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory ) {
			if ( BetaSigmaPhiContextFactory == null ) {
				throw new ArgumentNullException( "BetaSigmaPhiContextFactory" );
			}
			this.BetaSigmaPhiContextFactory = BetaSigmaPhiContextFactory;
		}

		public int Save( TEntity Entity ) {
			if ( Entity == null ) {
				throw new ArgumentNullException( "Entity", "Can't save null" );
			}
			Entity.ModifiedDate = DateTime.Now;
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				if ( Entity.IsNew() ) {
					db.GetTable<TEntity>().Add( Entity );
				} else {
					db.GetTable<TEntity>().Attach( Entity );
					db.DefeatChangeTracking( Entity );
				}
				try {
					return db.SaveChanges();
				} catch ( DbEntityValidationException dbEx ) {
					throw new ValidationFailureException( dbEx.Message, (
						from e in dbEx.EntityValidationErrors
						from f in e.ValidationErrors
						select new ValidationFailure {
							PropertyName = f.PropertyName,
							ErrorMessage = f.ErrorMessage,
						}
					).ToList(), dbEx );
				}
			}
		}

		public bool Any() {
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				return db.GetTable<TEntity>().Any();
			}
		}

		public List<TEntity> GetAll() {
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				return db.GetTable<TEntity>().ToList();
			}
		}

		public List<TEntity> GetActive() {
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				return (
					from e in db.GetTable<TEntity>()
					where e.IsActive
					select e
				).ToList();
			}
		}

		public TEntity GetById( int Id ) {
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				return db.GetTable<TEntity>().Find( Id );
			}
		}

	}
}
