using System.ComponentModel.DataAnnotations.Schema;
using Domain.Abstractions;

namespace Domain.Entities
{
    public class CardMedia : IDbEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Guid Id { get; set; }
		public byte[] Image { get; set; }

		public Guid GetPrimaryKey()
		{
			return Id;
		}
	}
}
