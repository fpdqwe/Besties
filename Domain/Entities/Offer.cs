using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class Offer : IDbEntity
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
		public long SenderId { get; set; }
		public long RecipientId { get; set; }
		public bool? RecipientApproval { get; set; }
		public DateTime DateCreated { get; set; } = DateTime.UtcNow;
		public DateTime? RecipientApprovalDate { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
