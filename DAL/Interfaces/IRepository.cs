namespace DAL.Interfaces
{
	public interface IRepository<T> where T : class
	{
		ApplicationDbContext CreateDatabaseContext();
		Task<List<T>> GetAll();
		Task<T> Find(long entityId);
		Task<T> SaveOrUdate(T entity);
		Task<T> Add(T entity);
		Task<T> Update(T entity);
		Task<bool> Delete(T entity);
	}
}
