using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Abstractions;

namespace Domain.Entities
{
    public class Offer : IDbEntity
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid SenderId { get; set; }
		public Guid RecipientId { get; set; }
		public bool? RecipientApproval { get; set; }
		public bool? SenderApproval { get; set; }
		public DateTime DateCreated { get; set; } = DateTime.UtcNow;
		public DateTime? RecipientApprovalDate { get; set; }

		public Guid GetPrimaryKey()
		{
			return Id;
		}
	}
}
