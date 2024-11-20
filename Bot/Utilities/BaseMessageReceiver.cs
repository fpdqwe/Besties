using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Utilities
{
	internal abstract class BaseMessageReceiver : IMessageReceiver
	{
		private protected ITelegramBotClient _client;
		private protected Chat _sender;
		private protected int _currentIndex = 0;
		private protected List<IReplyHandler> _handlers;
		private protected IReplyHandler _currentHandler;
		public IReplyHandler ReplyHandler
		{
			get
			{
				return _currentHandler;
			}
			set
			{
				_currentHandler = value;
				HandleStart();
			}
		}
		public MessageType AwaitType { get => _currentHandler.AwaitType; }
        protected BaseMessageReceiver(ITelegramBotClient client, Chat sender)
        {
            _client = client;
			_sender = sender;
			if (_handlers != null)
			{
				ReplyHandler = _handlers[_currentIndex];
			}
        }
        public virtual async Task OnMessageReceived(ITelegramBotClient client, Chat sender, Message message)
		{
			if (message.Type != AwaitType) return;
			await _currentHandler.Handle(client, sender, message);
			if (_currentHandler.IsFinished)
			{
				await HandleDrop();
			}
			else BotService.LogMessage($"{this.GetType()}: The handler ({ReplyHandler.GetType()}) worked, but the message did not pass its conditions");
		}
		public virtual async Task HandleStart()
		{
			BotService.LogMessage($"{this.GetType()}: handler had start ({ReplyHandler.GetType()})");
			await ReplyHandler.SendMessage(_client, _sender);
		}
		public virtual async Task HandleDrop()
		{
			BotService.LogMessage($"{this.GetType()}: handler dropped ({ReplyHandler.GetType()})");
			ReplyHandler = _handlers[_currentIndex + 1];
			_currentIndex += 1;
			await HandleStart();
		}
		
	}
}
