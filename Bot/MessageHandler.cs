using Telegram.Bot.Types;
using Telegram.Bot;
using System.Text;
using Telegram.Bot.Types.Enums;
using Bot.Commands;
using DAL.Repositories;
using DAL;
using User = Domain.Entities.User;
using Domain.Enums;
using Domain.Entities;

namespace Bot
{
	public class MessageHandler
	{
		// Fields and props
		public delegate void LogHandler(string message);
		public static event LogHandler Log;
		public static ChatManager ChatManager = new ChatManager();

		// Methods
		public static async Task DefaultHandler(ITelegramBotClient client, Update update, CancellationToken ct)
		{
			try
			{
				switch (update.Type)
				{
					case UpdateType.Message:
						BotOnMessageReceived(client, update.Message);
						break;

					default:
						break;
				}
			}
			catch (Exception ex)
			{
				Log?.Invoke(ex.Message);
			}
		}
		private static async Task BotOnMessageReceived(ITelegramBotClient client, Message message)
		{
			Log?.Invoke($"Receive message type {message.Type} in chat {message.Chat.Id}");

			if (message.Type != MessageType.Text) return;

			var Chat = await ChatManager.Find(message.Chat.Id, message.Chat.Username);
			if (!Chat.IsActive)
			{
				Chat.SetReply(SayHello);
				Chat.SetActive(true);
			}
			if (Chat.IsActive && Chat.Card.IsActive) (Chat.SetReply(Search.OfferCandidate); 
			Chat.OnMessageReceived(client, message.Text);
		}
		public static async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken ct)
		{
			Log?.Invoke(exception.Message);
		}
		private static async Task SayHello(ITelegramBotClient client, Chat sender, string message)
		{
			await client.SendTextMessageAsync(sender.Id, $"Привет, {sender.User.Username}!");
			await client.SendTextMessageAsync(sender.Id, $"Для начала давай создадим анкету, сколько тебе лет?");
			sender.SetReply(CardEdit.OnCardAgeChange);
		}
		public static void LogMessage(string message)
		{
			Log?.Invoke($"{message}");
		}
	}
}
