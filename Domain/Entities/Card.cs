using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entities
{
	public class Card : IDbEntity
	{
		[Key]
		public long Id { get; set; }
		[AllowNull]
		public string Name { get; set; }
		[AllowNull]
		public int Age { get; set; }
		[AllowNull]
		public string Description { get; set; }
		[AllowNull]
		public bool IsSmoking { get; set; }
		[AllowNull]
		public bool IsDrinking { get; set; }
		[AllowNull]
		public bool AnimalsLover { get; set; }
		[AllowNull]
		public int Salary { get; set; }
		[AllowNull]
		public int PSELowerBound { get; set; }
		[AllowNull]
		public int PSEUpperBound { get; set; }
		[AllowNull]
		public bool GreedyMode { get; set; }
		[AllowNull]
		public bool HealthyMode { get; set; }
		public User Owner { get; set; }
		public List<Comment> Comments { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
