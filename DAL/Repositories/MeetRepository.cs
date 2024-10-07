using DAL.Interfaces;
using Domain.Entities;

namespace DAL.Repositories
{
	public class MeetRepository : BaseRepository<Meet>
	{
		public MeetRepository(IContextManager contextManager) : base(contextManager) { }
	}
}
