using DAL.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
	public class UserRepository : BaseRepository<User>
	{
		public UserRepository(IContextManager contextManager) : base(contextManager) { }

		public async Task<User> CreateNewUser(long id, string name)
		{
			throw new NotImplementedException();
        }

		public async Task<User> GetByEmail(string email)
		{
			using(var context = CreateDatabaseContext())
			{
				return await context.Users.Where(u => u.EmailAddress == email)
					.FirstOrDefaultAsync();
			}
		}

		public async Task<User> GetByPhone(string phone)
		{
			using (var context = CreateDatabaseContext())
			{
				return await context.Users.Where(u => u.PhoneNumber == phone)
					.FirstOrDefaultAsync();
			}
		}
	}
}
