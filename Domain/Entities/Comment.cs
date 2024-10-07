namespace Domain.Entities
{
	public class Comment : IDbEntity
	{
		public long Id { get; set; }
		public long TargetId { get; set; }
		public string Value { get; set; }
		public double NumericValue { get; set; }
		public DateTime DateCreated { get; set; }
		public bool IsDeleted { get; set; }
		public Card Card { get; set; }
		public long CardId { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
