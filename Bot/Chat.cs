using Bot.Commands;
using Bot.Utilities;
using DAL.Repositories;
using Domain.Entities;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Domain.Entities.User;
using Timer = System.Timers.Timer;

namespace Bot
{
    public delegate Task ChatReplyDelegate(ITelegramBotClient client, Chat sender, string currentMessage);
    public delegate void ChatEventArgs(Chat chat);
	public class Chat
	{
        // Fields and properties
        private readonly Timer _timer;
        private readonly int _timerDelay = 600000;
        private readonly List<ChatEventArgs> _subscribers;
        private ChatReplyDelegate _nextReply;
        public IMessageReceiver CurrentReceiver { get; set; }
		public long Id { get; private set; }
        public User User { get; private set; }
        public Card Card
        {
            get => User.Card;
            set => User.Card = value;
        }
        public Card NewCard { get; set; }
        public bool IsActive { get; private set; }
        private bool _awaitCommand { get; set; }
        public SearchScopes SearchScopes { get; set; }
        // Ctor
        public Chat(long ChatId, User user)
        {
            Id = ChatId;
            User = user;
            IsActive = true;
            _subscribers = new List<ChatEventArgs>();
            _timer = new Timer(_timerDelay);
            _timer.Elapsed += OnTimedEvent;
            SearchScopes = new SearchScopes();
            SearchScopes.SkippedIds.Add(Id);
            BotService.LogMessage($"Chat with id = {Id} become active at {DateTime.Now}");
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
        public void OnMessageReceived(ITelegramBotClient client, string message)
        {
            BotService.LogMessage($"Chat {Id} got new message: \"{message}\" type of {message.GetType}; Reply - {_nextReply.Method.Name}");
            if (_awaitCommand) BotService.LogMessage($"Message in chat {Id} was ignored because of: command expected");
            // _nextReply.Invoke(client, this, message);
            
            ResetTimer();
        }
        public void TestReceive(ITelegramBotClient client, Message message)
        {
			BotService.LogMessage($"Chat {Id} got new message: \"{message.Text}\" " +
                $"type of {message.GetType}; Reply - IMessageReceiver");
			if (_awaitCommand) BotService.LogMessage($"Message in chat {Id} was ignored because of: command expected");
			// _nextReply.Invoke(client, this, message);
            CurrentReceiver?.OnMessageReceived(client, this, message);
			ResetTimer();
		}
        public void SetReply(ChatReplyDelegate reply, bool awaitCommand = false)
        {
            _nextReply = reply;
            _awaitCommand = awaitCommand;
        }
    }
}
