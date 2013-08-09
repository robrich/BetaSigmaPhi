namespace BetaSigmaPhi.Repository {
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Data.Entity.Validation;
	using System.Linq;
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Library;
	using BetaSigmaPhi.Repository.Models;

	public interface IRepository<TEntity> where TEntity : IEntity {
		int Save( TEntity Entity );
		int Save( List<TEntity> Entities );
		void Delete( int Id );
		void DeleteForever( int Id );
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
		
		// Clear cache, update GUI, etc
		protected virtual void OnSaved() {
		}

		private void SaveEntityGuts(IBetaSigmaPhiContext db, TEntity Entity) {
			Entity.ModifiedDate = DateTime.Now;
			IDbSet<TEntity> table = db.GetTable<TEntity>();
			if ( Entity.IsNew() ) {
				// Add
				Entity.CreatedDate = Entity.ModifiedDate;
				table.Add( Entity );
			} else {
				// Update
				table.Attach( Entity );
				db.DefeatChangeTracking(Entity);
			}
		}

		public virtual int Save(TEntity Entity) {
			if (Entity == null) {
				throw new ArgumentNullException("Entity", "Can't save null");
			}
			int results = -1;
			using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext()) {
				this.SaveEntityGuts( db, Entity );
				try {
					results = db.SaveChanges();
				} catch (DbEntityValidationException dbEx) {
					throw new ValidationFailureException(
						dbEx.Message, (
							from e in dbEx.EntityValidationErrors
							from f in e.ValidationErrors
							select new ValidationFailure {
								PropertyName = f.PropertyName,
								ErrorMessage = f.ErrorMessage,
							}
						).ToList(), dbEx
					);
				}
			}
			this.OnSaved();
			return results;
		}

		public virtual int Save( List<TEntity> Entities ) {
			if ( Entities.IsNullOrEmpty() ) {
				return 0; // Successfully did nothing
			}
			int results = 0;
			using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext()) {
				List<Exception> exs = new List<Exception>();
				foreach ( TEntity entity in Entities ) {
					try {
						this.SaveEntityGuts( db, entity );
					} catch ( Exception ex ) {
						exs.Add( ex );
					}
				}
				if ( exs.IsNullOrEmpty() ) {
					// Save at once so it'll all succeed or fail
					try {
						results = db.SaveChanges();
					} catch (DbEntityValidationException dbEx) {
						exs.Add( new ValidationFailureException(
							dbEx.Message, (
								from e in dbEx.EntityValidationErrors
								from f in e.ValidationErrors
								select new ValidationFailure {
									PropertyName = f.PropertyName,
									ErrorMessage = f.ErrorMessage,
								}
							).ToList(), dbEx
						));
					}
				}
				if ( !exs.IsNullOrEmpty() ) {
					throw new AggregateException( exs );
				}
			}
			this.OnSaved();
			return results;
		}

		public virtual void Delete( int Id ) {
			if ( Id < 1 ) {
				throw new ArgumentOutOfRangeException( "Id", "Can't delete " + Id );
			}
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				TEntity entity = db.GetTable<TEntity>().Find( Id );
				if ( entity != null ) {
					entity.IsActive = false;
					db.SaveChanges();
				} else {
					// You tried to make it gone and it is -- success!
				}
			}
			this.OnSaved();
		}

		public virtual void DeleteForever( int Id ) {
			if ( Id < 1 ) {
				// TODO: throw?
				return; // You tried to make it gone, it never existed -- success!
			}
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				IDbSet<TEntity> table = db.GetTable<TEntity>();
				TEntity entity = table.Find( Id );
				if ( entity == null ) {
					//throw new ArgumentNullException( "Can't delete id " + Id + " because it doesn't exist", (Exception)null );
					return; // You tried to make it gone and it is -- success!
				}
				table.Remove( entity );
				db.SaveChanges();
			}
			this.OnSaved();
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
