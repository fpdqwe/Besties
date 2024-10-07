namespace DAL.Interfaces
{
	public interface IContextManager
	{
		ApplicationDbContext CreateDatabaseContext();
	}
}
