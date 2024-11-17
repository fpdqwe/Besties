using Domain.Enums;
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
		public Gender Gender { get; set; }
		public Gender TargetGender { get; set; }
		public byte Region { get; set; }
		public string? Description { get; set; }
		public bool IsSmoking { get; set; } = false;
		public bool IsDrinking { get; set; } = false;
		public bool HealthyMode { get; set; } = false;
		public bool IsActive { get; set; } = false;
		public User Owner { get; set; }
		public List<Comment> Comments { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}

	public enum Gender : byte
	{
		Male = 0,
		Female = 1,
		NotSpecified = 2
	}
}
