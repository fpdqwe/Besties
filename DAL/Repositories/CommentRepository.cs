using DAL.Interfaces;
using Domain.Entities;

namespace DAL.Repositories
{
	public class CommentRepository : BaseRepository<Comment>
	{
		public CommentRepository(IContextManager contextManager) : base(contextManager) { }
	}
}
