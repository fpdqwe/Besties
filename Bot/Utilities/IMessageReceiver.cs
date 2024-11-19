using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Utilities
{
	public delegate Task MessageHandler(ITelegramBotClient client, Chat sender, Message message);
	public interface IMessageReceiver 
	{
		public IReplyHandler ReplyHandler { get; }
		public MessageType AwaitType { get; }
		public Task OnMessageReceived(ITelegramBotClient client, Chat sender, Message message);
		public Task HandleStart();
		public Task HandleDrop();
	}
}
