using Bot.Commands;
using DAL;
using DAL.Repositories;
using Domain.Entities;

namespace Bot
{
	public class ChatManager
	{
		private static readonly ContextManager _contextManager = new ContextManager();
		private static readonly UserRepository _userRepository = new UserRepository(_contextManager);
		private static readonly CardRepository _cardRepository = new CardRepository(_contextManager);
		private readonly List<Chat> _chats;
		public List<Chat> Chats { get { return _chats; } }

        public ChatManager()
        {
			_chats = new List<Chat>();
        }

		/// <summary>
		/// Возвращает чат по его id, по умолчанию активирует его если он не найден.
		/// Так же по умолчанию создаёт нового пользователя и анкету, если пользователь чата не найден
		/// </summary>
		/// <param name="id">id чата</param>
		/// <param name="username">Тэг пользователя</param>
		/// <param name="makeActive">Активировать чат, если он не будет найден</param>
		/// <returns></returns>
		/// <exception cref="Exception">Выбрасывается если makeActive = false, при этом искомый чат не активен</exception>
        public async Task<Chat> Find(long id, string username, bool makeActive = true)
		{
			if (makeActive)
			{
				foreach (Chat chat in _chats)
				{
					if (chat.Id == id) { return chat; }
				}
				var user = await RecognizeUser(id, username);
				var newChat = await CreateNewChat(user);
				_chats.Add(newChat);
				newChat.Subscribe(RemoveInactiveChat);
				return newChat;
			}
			else
			{
				foreach (Chat chat in _chats)
				{
					if (chat.Id == id) { return chat; }
				}
			}
			throw new Exception($"Сhat with id: {id} is not active");
		}
		public async Task<Card> GetOffer(Chat sender)
		{
			try
			{
				return await FindRandomCard(sender.SearchScopes);
			}
			catch (Exception ex) { throw ex; }
		}
		private async Task<Chat> CreateNewChat(User user)
		{
			user.Card = await FindCardByUserId(user.Id);
			Chat chat = new Chat(user.Id, user);
			chat.SetActive(false);
			return chat;
		}
		private void RemoveInactiveChat(Chat sender)
		{
			_chats.Remove(sender);
			sender.Unsubscribe(RemoveInactiveChat);
		}

		// DB operations
		private async Task<User> RecognizeUser(long ChatId, string username)
		{
			var user = await _userRepository.Find(ChatId);
			if (user == null)
			{
				user = await _userRepository.CreateNewUser(
					ChatId,
					username,
					Domain.Enums.ChatMode.GuestPrivate);
			}

			return user;
		}
		private async Task<Card> FindCardByUserId(long id)
		{
			var card = await _cardRepository.Find(id);
			if (card == null) throw new Exception("А какого хуя у тебя карточка не создалась то епт");
			return card;
		}
		private async Task<Card> FindRandomCard(SearchScopes scopes)
		{
			var list = await _cardRepository.GetAll();
			foreach (var card in list)
			{
				foreach (var scope in scopes.SkippedIds)
				{
					if (card.Id != scope) return card;
				}
			}
			throw new Exception();
		}
		public static async Task ApplyCardChanges(Chat chat)
		{
			await _cardRepository.Update(chat.NewCard);
			chat.Card = chat.NewCard;
			chat.NewCard = null;
		}
	}
}
