﻿namespace BetaSigmaPhi.DataAccess {
	using System;
	using System.Data;
	using System.Data.Entity;
	using System.Data.Entity.ModelConfiguration.Conventions;
	using BetaSigmaPhi.Entity;

	public interface IBetaSigmaPhiContext : IDisposable {
		IDbSet<User> Users { get; }
		IDbSet<TEntity> GetTable<TEntity>() where TEntity : IEntity;
		void DefeatChangeTracking<TEntity>(TEntity Entity) where TEntity : IEntity;
		int SaveChanges();
	}

	public class BetaSigmaPhiContext : DbContext, IBetaSigmaPhiContext {

		public IDbSet<User> Users { get; set; }

		public IDbSet<TEntity> GetTable<TEntity>() where TEntity : IEntity {
			return this.Set<TEntity>();
		}

		public void DefeatChangeTracking<TEntity>(TEntity Entity) where TEntity : IEntity {
			this.Entry(Entity).State = EntityState.Modified;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			base.OnModelCreating(modelBuilder);
		}

		public BetaSigmaPhiContext()
			: base("DefaultConnection") { // FRAGILE: The name of the connectionString in web.connection.config
		}

		static BetaSigmaPhiContext() {
			Database.SetInitializer<BetaSigmaPhiContext>(null);
		}

	}
}