
using BLL.Abstractions;
using BLL.Infrastructure;
using BLL.Services;
using DAL;
using DAL.Interfaces;

namespace API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var configuration = builder.Configuration;
			var services = builder.Services;

			configuration.AddJsonFile("/Properties/secrets.json");
			services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
			services.Configure<DbOptions>(configuration.GetSection(nameof(DbOptions)));
			// Add services to the container.
			services.AddSingleton<IContextManager, ContextManager>();
			services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();
			services.AddSingleton<IPasswordHasher, PasswordHasher>();
			services.AddTransient<IIdentityService, IdentityService>();


			services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
