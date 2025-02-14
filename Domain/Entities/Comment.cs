using Domain.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Comment : IDbEntity
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid UserId { get; set; }
		public Guid TargetId { get; set; }
		public string Text { get; set; }
		public decimal Estimation { get; set; }
		public DateTime DateCreated { get; set; }
		public bool IsDeleted { get; set; }
		[ForeignKey(nameof(UserId))]
		public User Sender { get; set; }
		[ForeignKey(nameof(TargetId))]
		public User Target { get; set; }

		public Guid GetPrimaryKey()
		{
			return Id;
		}
	}
}
