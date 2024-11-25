using DAL.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
	public class BaseRepository<T> : IRepository<T> where T : class
	{
		private readonly IContextManager _contextManager;
		protected BaseRepository(IContextManager contextManager)
		{
			_contextManager = contextManager;
		}
		public ApplicationDbContext CreateDatabaseContext()
		{
			return _contextManager.CreateDatabaseContext();
		}
		public async Task<T> Add(T entity)
		{
			using (var context = CreateDatabaseContext())
			{
				var iDbEntity = entity as IDbEntity;

				if (iDbEntity == null)
					throw new ArgumentException("entity should be IDbEntity type", "entity");

				await context.Set<T>().AddAsync(entity);
				
				context.SaveChanges();
			}

			return entity;
		}
		public async Task<bool> Delete(T entity)
		{
			using (var context = CreateDatabaseContext())
			{
				try
				{
					var iDbEntity = entity as IDbEntity;

					if (iDbEntity == null)
						throw new ArgumentException("entity should be IDbEntity type", "entity");

					context.Set<T>().Remove(entity);
					context.SaveChanges();
					return true;
				}
				catch (Exception ex) { return false; }
			}
		}
		public async Task<T> Find(long entityId)
		{
			using (var context = CreateDatabaseContext())
			{
				return await context.Set<T>().FindAsync(entityId);
			}
		}
		public async Task<List<T>> GetAll()
		{
			using (var context = CreateDatabaseContext())
			{
				return await context.Set<T>().ToListAsync();
			}
		}
		public async Task<T> SaveOrUdate(T entity)
		{
			var iDbEntity = entity as IDbEntity;

			if (iDbEntity == null)
				throw new ArgumentException("entity should be IDbEntity type", "entity");

			return iDbEntity.GetPrimaryKey() == 0 ? await Add(entity) : await Update(entity);

		}
		public async Task<T> Update(T entity)
		{
			using (var context = CreateDatabaseContext())
			{
				var iDbEntity = entity as IDbEntity;
				if (iDbEntity == null)
					throw new ArgumentException("entity should be IDbEntity type", "entity");

				var attachedEntity = await context.Set<T>().FindAsync(iDbEntity.GetPrimaryKey());
				context.Entry(attachedEntity).CurrentValues.SetValues(entity);
				await context.SaveChangesAsync();
			}
			return entity;
		}
	}
}
