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
				Gender = Gender.Male,
				Rating = 5,
				Card = new Card(),
			};
			//result.Card.Id = id;
			
			
			await Add(result);

			return result;
        }
	}
}
