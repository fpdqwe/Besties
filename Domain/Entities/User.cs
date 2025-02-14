using Domain.Abstractions;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User : IDbEntity
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		[Required, MaxLength(50)]
		public string Name { get; set; }
		public string Password { get; set; }
		public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
		public long TelegramId { get; set; }
		public string EmailAddress { get; set; }
		public string PhoneNumber { get; set; }
		[ForeignKey(nameof(Id))]
		public Card Card { get; set; }

		public Guid GetPrimaryKey()
		{
			return Id;
		}
	}
}
