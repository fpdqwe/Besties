using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
	public class Meet : IDbEntity
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
		public bool FirstPersonApproval { get; set; }
		public bool SecondPersonApproval { get; set; }
		public long FirstPersonId { get; set; }
		public long SecondPersonId { get; set; }
		public long FirstPersonCommentId { get; set; }
		public long SecondPersonCommentId { get; set; }
		public DateOnly DateMatched { get; set; }
		public DateOnly DateOfMeet { get; set; }
		public User FirstPerson { get; set; }
		public User SecondPerson { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
