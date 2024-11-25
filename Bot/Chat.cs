using Bot.Utilities;
using Domain.Entities;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Domain.Entities.User;
using Timer = System.Timers.Timer;
using Bot.Types;
using Bot.Exceptions;
using Bot.Branches;

namespace Bot
{
    public delegate Task ChatUpdateHandler(Unit unit);
    public delegate void ChatEventArgs(Chat chat);
	public class Chat
	{
        // Fields and properties
        private readonly Timer _timer;
        private readonly int _timerDelay = 600000;
        private readonly List<ChatEventArgs> _subscribers;
        private ChatUpdateHandler _nextReply;
		public long Id { get => User.Id; }
        public User User { get; private set; }
        public Card Card
        {
            get => User.Card;
            set => User.Card = value;
        }
        public Card NewCard { get; set; }
        public bool IsActive { get; private set; }
        public SearchScopes SearchScopes { get; set; }
        // Ctor
        public Chat(long ChatId, User user)
        {
            User = user;
            IsActive = true;
            _subscribers = new List<ChatEventArgs>();
            _timer = new Timer(_timerDelay);
            _timer.Elapsed += OnTimedEvent;
            SearchScopes = new SearchScopes();
            SearchScopes.SkippedIds.Add(Id);
            BotService.LogMessage($"Chat with id = {Id} initialized at {DateTime.Now}");
        }
        // Utils
        public void SetActive(bool active)
        {
            IsActive = active;
        }
        public string GetUserLink()
        {
            return $"tg://user?id={Id}";
        }
        public void CopyCard()
        {
            NewCard = new Card()
            {
                Id = Id,
                Name = Card.Name,
                Description = Card.Description,
                Gender = Card.Gender,
                TargetGender = Card.TargetGender,
                Age = Card.Age,
                Region = Card.Region,
                IsDrinking = Card.IsDrinking,
                IsSmoking = Card.IsSmoking,
                IsActive = Card.IsActive,
            };
        }

		// Chat expiration logic
		public void StartTimer()
        {
            _timer.AutoReset = false;
            _timer.Start();
        }
        public void ResetTimer()
        {
            _timer.Stop();
            StartTimer();
            BotService.LogMessage($"Timer was reset in chat {Id} at {DateTime.Now}");
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            NotifySubscribers();
        }
        public void Subscribe(ChatEventArgs subscriber)
        {
            _subscribers.Add(subscriber);
        }
        public void Unsubscribe(ChatEventArgs subscriber)
        {
            _subscribers.Remove(subscriber);
        }
        private void NotifySubscribers()
        {
            try
            {
                foreach (var subscriber in _subscribers)
                {
                    BotService.LogMessage($"Chat with id = {Id} become inactive! Timestamp - {DateTime.Now}");
                    subscriber.Invoke(this);
                }
            }
            catch (Exception ex) { BotService.LogMessage(ex.ToString()); } 
        }

        // Chatting logic
        public async Task OnMessageReceived(Unit unit)
        {
            BotService.LogMessage($"{Id} received new message: \"{unit.MesText}\" type of {unit.MesType.ToString()}; " +
                $"Reply - {_nextReply.Method.Name}");

            try
            {
				_nextReply?.Invoke(unit);
			}
            catch (EmptyOfferPoolException emptyPoolEx)
            {
                Search.OnEmptyCardPool(unit.Client, unit.Chat);
            }
            
            ResetTimer();
        }
        public async Task OnMessageReceived(ITelegramBotClient client, Update update)
        {
            var unit = new Unit(client, this, update);
			try
			{
				_nextReply?.Invoke(unit);
			}
			catch (EmptyOfferPoolException emptyPoolEx)
			{
				Search.OnEmptyCardPool(unit.Client, unit.Chat);
			}

			ResetTimer();
		}
        public void SetReply(ChatUpdateHandler reply)
        {
            _nextReply = reply;
        }
    }
}
