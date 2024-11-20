using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Utilities
{
	internal abstract class ReplyHandler : IReplyHandler
	{
		private protected MessageType _awaitType;
		private protected string _messageText;
		private protected bool _isFinished = false;
		public MessageHandler Handler { get; private set; }
		public string Message { get => _messageText; }
		public MessageType AwaitType { get; private set; }
		public bool IsFinished { get => _isFinished; }
        public virtual async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			await client.SendTextMessageAsync(sender.Id, Message);
		}
		public virtual async Task SendMessage(ITelegramBotClient client, Chat sender, string message,
			IReplyMarkup markup = null)
		{
			var mes = await client.SendTextMessageAsync(
				sender.Id,
				message,
				replyMarkup: markup);
			BotService.LogMessage($"Handler replied: {mes.Text}. {mes.From.FirstName}");
		}
		public virtual async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			throw new NotImplementedException();
		}
	}
}
