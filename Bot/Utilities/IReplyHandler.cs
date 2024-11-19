using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Utilities
{
	public interface IReplyHandler
	{
		public MessageHandler Handler { get; }
		public string Message { get; }
		public MessageType AwaitType { get; }
		public Task SendMessage(ITelegramBotClient client, Chat sender);
		public Task Handle(ITelegramBotClient client, Chat sender, Message message);
		public bool IsFinished { get; }
	}
}
