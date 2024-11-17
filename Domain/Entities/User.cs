using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
	public class User : IDbEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id { get; set; }
		[Required]
		public string Username { get; set; }
		[Required]
		public ChatMode ChatMode { get; set; }
		public Card Card { get; set; }
		public double Rating { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
