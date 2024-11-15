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
			_connectionString = Resources.strings.server2;
		}
        public ApplicationDbContext CreateDatabaseContext()
		{
			Debug.WriteLine("CONNECTION STRING: " + _connectionString);
			
			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			var options = optionsBuilder
				.UseNpgsql(_connectionString)
				.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
				.Options;
			
			return new ApplicationDbContext(options);
			
		}
	}
}
