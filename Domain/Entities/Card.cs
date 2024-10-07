namespace Domain.Entities
{
	public class Card : IDbEntity
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public int Age { get; set; }
		public string Description { get; set; }
		public bool IsSmoking { get; set; }
		public bool IsDrinking { get; set; }
		public bool AnimalsLover { get; set; }
		public int Salary { get; set; }
		public int PSELowerBound { get; set; }
		public int PSEUpperBound { get; set; }
		public bool GreedyMode { get; set; }
		public bool HealthyMode { get; set; }
		public User Owner { get; set; }
		public List<Comment> Comments { get; set; }

		public long GetPrimaryKey()
		{
			return Id;
		}
	}
}
