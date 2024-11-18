using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Utilities
{
	internal abstract class ReplyHandler : IReplyHandler
	{
		protected MessageType _awaitType;
		private readonly string _messageText;
		public MessageHandler Handler { get; private set; }
		public string Message { get => _messageText; }
		public MessageType AwaitType { get; private set; }
        protected ReplyHandler(string Message)
        {
            _messageText = Message;
        }

        public virtual async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			await client.SendTextMessageAsync(sender.Id, Message);
		}
		public virtual async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			throw new NotImplementedException();
		}
	}
}
