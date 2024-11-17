using DAL.Interfaces;
using Domain.Entities;

namespace DAL.Repositories
{
	public class CardMediaRepository : BaseRepository<CardMedia>
	{
		protected CardMediaRepository(IContextManager contextManager) : base(contextManager)
		{
		}


	}
}
