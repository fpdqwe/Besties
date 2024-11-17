using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
	public class CardMedia : IDbEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[ForeignKey(nameof(Card.Id))]
		public long Id { get; set; }
		public byte[] Image { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
