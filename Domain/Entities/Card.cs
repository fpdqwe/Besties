using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Text;

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
		public DateTime? LastActivationDate { get; set; }
		public User Owner { get; set; }
		public List<Comment> Comments { get; set; }

		public override bool Equals(object? obj)
		{
			var card = obj as Card;
			if (Id != card.Id) return false;
			if (Name != card.Name) return false;
			if (Description != card.Description) return false;
			if (Age != card.Age) return false;
			if (Gender != card.Gender) return false;
			if (TargetGender != card.TargetGender) return false;
			if (Region != card.Region) return false;
			if (IsSmoking != card.IsSmoking) return false;
			if (IsDrinking != card.IsDrinking) return false;
			return true;
		}
		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
