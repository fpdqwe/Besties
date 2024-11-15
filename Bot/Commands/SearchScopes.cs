using Domain.Entities;

namespace Bot.Commands
{
	public class SearchScopes
	{
		public List<long> SkippedIds = new List<long>();
		public Card LastOffer;
	}
}
