using Domain.Entities;

namespace BLL.Abstractions
{
	public interface IIdentityService
	{
		public Task<User> Register(string username, string password, string contact);
		public Task<User> RegisterTelegramClient(long telegramId, string password);
		public Task<string> Login(string username, string password);
		
	}
}
