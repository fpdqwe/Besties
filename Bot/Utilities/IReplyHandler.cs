using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Utilities
{
	internal interface IReplyHandler
	{
		public MessageHandler Handler { get; }
		public string Message { get; }
		public Task SendMessage(ITelegramBotClient client, Chat sender);
	}
}
