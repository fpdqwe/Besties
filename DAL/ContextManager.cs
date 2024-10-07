using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DAL
{
	public class ContextManager : IContextManager
	{
		private readonly string _connectionString;

        public ContextManager()
        {
			_connectionString = Resources.strings.dbConnectionString.Substring(1, Resources.strings.dbConnectionString.Length - 2);
		}
        public ApplicationDbContext CreateDatabaseContext()
		{
			Debug.WriteLine("CONNECTION STRING: " + _connectionString);
			
			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			var options = optionsBuilder
				.UseNpgsql(_connectionString)
				.Options;
			
			return new ApplicationDbContext(options);
			
		}
	}
}
