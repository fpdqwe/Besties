using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Card> Cards { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Meet> Meets { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasOne(x => x.Card)
				.WithOne(x => x.Owner)
				.HasForeignKey<User>(x => x.CardId);
			modelBuilder.Entity<Comment>()
				.HasOne(x => x.Card)
				.WithMany(x => x.Comments)
				.HasForeignKey(x => x.CardId);
		}
	}
}
