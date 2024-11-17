using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			var options = optionsBuilder
				.UseNpgsql(_connectionString)
				.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors()
				.Options;
			
			return new ApplicationDbContext(options);
		}

		public static DbContextOptions<ApplicationDbContext> GetContextOptions()
		{
			var connstring = Resources.strings.server2;
			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			var options = optionsBuilder
				.UseNpgsql(connstring)
				.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors()
				.Options;

			return options;
		} 
	}
}
