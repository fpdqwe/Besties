﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL
{
	public class ApplicationDbContext : DbContext
	{
		private static DbContextOptions<ApplicationDbContext> _options = ContextManager.GetContextOptions();
		public DbSet<User> Users { get; set; }
		public DbSet<Card> Cards { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Meet> Meets { get; set; }
		public DbSet<Offer> Offers { get; set; }
		public DbSet<CardMedia> CardMedia { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}
		public ApplicationDbContext() : base(_options)
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
		}
	}
}
