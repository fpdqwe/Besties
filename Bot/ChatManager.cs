using Bot.Exceptions;
using Bot.Types;
using DAL;
using DAL.Repositories;
using Domain.Entities;
using System.Resources;

namespace Bot
{
    public class ChatManager
	{
		private static readonly ContextManager _contextManager = new ContextManager();
		private static readonly UserRepository _userRepository = new UserRepository(_contextManager);
		private static readonly CardRepository _cardRepository = new CardRepository(_contextManager);
		private static readonly OfferRepository _offerRepository = new OfferRepository(_contextManager);
		private readonly List<Chat> _chats;
		public List<Chat> Chats { get { return _chats; } }

        public ChatManager()
        {
			_chats = new List<Chat>();
        }

		/// <summary>
		/// Возвращает чат по его id, по умолчанию создаёт нового пользователя и анкету, 
		/// если пользователь чата не найден
		/// </summary>
		/// <param name="id">id чата</param>
		/// <param name="username">Тэг пользователя</param>
		/// <param name="makeActive">Активировать чат, если он не будет найден</param>
		/// <returns></returns>
		/// <exception cref="Exception">Выбрасывается если makeActive = false, при этом искомый чат не активен</exception>
		public async Task<Chat> Find(long id, string username = null, bool makeActive = true)
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
			var result = await FindRandomCard(sender.Card, sender.SearchScopes);
			return result;
		}
		public async Task<Card> GetOfferSender (long id)
		{
			return await FindCardByUserId(id);
		}
		private async Task<Chat> CreateNewChat(User user)
		{
			user.Card = await FindCardByUserId(user.Id);
			Chat chat = new Chat(user.Id, user);
			chat.SetActive(false);

			var incomingOffers = await GetIncomingOffers(user.Id);
			if (incomingOffers != null && incomingOffers.Count > 0) chat.SearchScopes.SetIncomingOffers(incomingOffers);
			var outgoingOffers = await GetOutgoingOffers(user.Id);
			if (outgoingOffers != null && outgoingOffers.Count > 0) chat.SearchScopes.SetOutgoingOffers(outgoingOffers);
			
			return chat;
		}
		private void RemoveInactiveChat(Chat sender)
		{
			sender.Unsubscribe(RemoveInactiveChat);
			_chats.Remove(sender);
		}

		// DB operations
		private async Task<User> RecognizeUser(long ChatId, string username)
		{
			var user = await _userRepository.Find(ChatId);
			if (user == null && username == null)
			{
				user = await _userRepository.CreateNewUser(
					ChatId,
					ChatId.ToString(),
					Domain.Enums.ChatMode.Guest);
			}
			if (user == null && username != null)
			{
				user = await _userRepository.CreateNewUser(
					ChatId,
					username,
					Domain.Enums.ChatMode.Guest);
			}

			return user;
		}
		private async Task<Card> FindCardByUserId(long id)
		{
			var card = await _cardRepository.Find(id);
			if (card == null) throw new Exception("А какого хуя у тебя карточка не создалась то епт");
			return card;
		}
		private async Task<Card> FindRandomCard(Card card, SearchScopes scopes)
		{
			var offerList = await _cardRepository.GetCardsForSearch(card);
			foreach (var offer in offerList)
			{
				bool notSkipped = true;
				foreach (var scope in scopes.SkippedIds)
				{
					if (offer.Id == scope) notSkipped = false;	
				}
				if (notSkipped) return offer;
			}
			return null;
		}

		// Offers
		private async Task<List<Offer>> GetUncheckedOffers(long recipientId)
		{
			return await _offerRepository.GetOffersByRecepientId(recipientId);
		}
		private async Task<List<Offer>> GetOutgoingOffers(long senderId)
		{
			return await _offerRepository.GetOffersBySenderId(senderId);
		}
		private async Task<List<Offer>> GetIncomingOffers(long recepientId)
		{
			return await _offerRepository.GetOffersByRecepientId(recepientId);
		}
		public async Task<Offer> SaveOrUpdateOffer(Offer offer)
		{
			if (offer.SenderApproval == null) return null;
			return await _offerRepository.SaveOrUdate(offer);
		}

		// Photo
		public async Task<CardMedia> GetCardPhoto(long id)
		{
			try
			{
				return await Utilities.ResourceReader.GetImage(id);
			}
			catch (Exception ex)
			{
				throw new InvalidOfferException("Offers card photo does not exist or ResourceReader unable to find it", ex, id);
			}
		}
		public async Task SaveCardPhoto(CardMedia media)
		{
			Utilities.ResourceReader.SaveImage(media);
		}
		public async Task SetCardStatus(Card card, bool status)
		{
			card.IsActive = status;
			_cardRepository.SaveOrUdate(card);
		}

		// Card
		public static async Task ApplyCardChanges(Chat chat)
		{
			chat.NewCard.IsActive = true;
			chat.NewCard.LastActivationDate = DateTime.UtcNow;
			await _cardRepository.Update(chat.NewCard);
			chat.Card = new Card()
			{
				Id = chat.NewCard.Id,
				Age = chat.NewCard.Age,
				Name = chat.NewCard.Name,
				Gender = chat.NewCard.Gender,
				TargetGender = chat.NewCard.TargetGender,
				Region = chat.NewCard.Region,
				Description = chat.NewCard.Description,
				IsDrinking = chat.NewCard.IsDrinking,
				IsSmoking = chat.NewCard.IsSmoking,
				IsActive = chat.NewCard.IsActive,
				HealthyMode = chat.NewCard.HealthyMode,
				LastActivationDate = chat.NewCard.LastActivationDate,
			};
			chat.NewCard = null;

			if (chat.User.ChatMode == Domain.Enums.ChatMode.Guest)
			{
				chat.User.ChatMode = Domain.Enums.ChatMode.ExistingUser;
				_userRepository.Update(chat.User);
			}
		}
	}
}
