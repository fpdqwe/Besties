using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
	public class User : IDbEntity
	{
		[Key]
		public long Id { get; set; }
		[Required]
		public string Username { get; set; }
		[Required]
		public ChatMode ChatMode { get; set; }
		public Gender Gender { get; set; }
		public Card Card { get; set; }
		
		public long CardId { get; set; }
		public double Rating { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
