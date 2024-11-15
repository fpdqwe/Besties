using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entities
{
	public class Card : IDbEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id { get; set; }
		public string? Name { get; set; }
		public int Age { get; set; } = -1;
		public string? Description { get; set; }
		public bool IsSmoking { get; set; } = false;
		public bool IsDrinking { get; set; } = false;
		public bool AnimalsLover { get; set; } = false;
		public int Salary { get; set; } = -1;
		public int PSELowerBound { get; set; } = -1;
		public int PSEUpperBound { get; set; } = -1;
		public bool GreedyMode { get; set; } = false;
		public bool HealthyMode { get; set; } = false;
		public bool IsActive { get; set; } = false;
		public User Owner { get; set; }
		public List<Comment> Comments { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
