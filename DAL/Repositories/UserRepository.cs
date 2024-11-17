using DAL.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace DAL.Repositories
{
	public class UserRepository : BaseRepository<User>
	{
		public UserRepository(IContextManager contextManager) : base(contextManager) { }

		public async Task<User> CreateNewUser(long id, string name, ChatMode chatMode)
		{
			User result = new User
			{
				Id = id,
				Username = name,
				ChatMode = chatMode,
				Rating = 5,
				Card = new Card()
				{
					Id = id,
					Name = name
				}
			};			
			
			await Add(result);
			//using (var context = CreateDatabaseContext())
			//{
			//	var newUser = context.Users.OrderByDescending(x => x.Id).FirstOrDefault();
			//	newUser.Id = id;
			//	await Update(newUser);
			//}
			return result;
        }
	}
}
