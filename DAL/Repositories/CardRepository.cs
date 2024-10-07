using DAL.Interfaces;
using Domain.Entities;

namespace DAL.Repositories
{
	public class CardRepository : BaseRepository<Card>
	{
		public CardRepository(IContextManager contextManager) : base(contextManager) { }
	}
}
