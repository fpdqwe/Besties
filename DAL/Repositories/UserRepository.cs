using DAL.Interfaces;
using Domain.Entities;

namespace DAL.Repositories
{
	public class UserRepository : BaseRepository<User>
	{
		public UserRepository(IContextManager contextManager) : base(contextManager) { }
	}
}
