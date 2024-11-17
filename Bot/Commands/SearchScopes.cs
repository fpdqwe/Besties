using Domain.Entities;

namespace Bot.Commands
{
	public class SearchScopes
	{
		private List<long> _skippedIds = new List<long>(10);
		public List<long> SkippedIds { get { return _skippedIds; } }
		public Card LastOffer;
	}
}
