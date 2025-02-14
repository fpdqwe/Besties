using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Abstractions;

namespace Domain.Entities
{
    public class Meet : IDbEntity
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Guid Id { get; set; } = Guid.NewGuid();
		public bool FirstPersonApproval { get; set; }
		public bool SecondPersonApproval { get; set; }
		public Guid FirstPersonId { get; set; }
		public Guid SecondPersonId { get; set; }
		public Guid FirstPersonCommentId { get; set; }
		public Guid SecondPersonCommentId { get; set; }
		public DateOnly DateMatched { get; set; }
		public DateOnly DateOfMeet { get; set; }
		[ForeignKey(nameof(FirstPersonId))]
		public User FirstPerson { get; set; }
		[ForeignKey(nameof(SecondPersonId))]
		public User SecondPerson { get; set; }

		public Guid GetPrimaryKey()
		{
			return Id;
		}
	}
}
