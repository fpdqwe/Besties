using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

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
				.HasForeignKey<User>(x => x.Id);
			modelBuilder.Entity<Comment>()
				.HasOne(x => x.Card)
				.WithMany(x => x.Comments)
				.HasForeignKey(x => x.CardId);

			modelBuilder.Entity<User>()
				.Property(x => x.Id)
				.ValueGeneratedNever();

			//modelBuilder.Entity<Card>()
			//	.Property(x => x.Name)
			//	.IsRequired(false);
			//modelBuilder.Entity<Card>()
			//	.Property(x => x.Age)
			//	.HasDefaultValue(0);
			//modelBuilder.Entity<Card>()
			//	.Property(x => x.Description)
			//	.IsRequired(false);
			//modelBuilder.Entity<Card>()
			//	.Property(x => x.IsSmoking)
			//	.HasDefaultValue(false);
			//modelBuilder.Entity<Card>()
			//	.Property(x => x.IsDrinking)
			//	.HasDefaultValue(false);
			//modelBuilder.Entity<Card>()
			//	.Property(x => x.AnimalsLover)
			//	.HasDefaultValue(false);
			//modelBuilder.Entity<Card>()
			//	.Property(x => x.Salary)
			//	.HasDefaultValue(0);
			//modelBuilder.Entity<Card>()
			//	.Property(x => x.PSELowerBound)
			//	.HasDefaultValue(0);
			//modelBuilder.Entity<Card>()
			//	.Property(x => x.PSEUpperBound)
			//	.HasDefaultValue(0);
			//modelBuilder.Entity<Card>()
			//	.Property(x => x.HealthyMode)
			//	.HasDefaultValue(false);
		}
	}
}
