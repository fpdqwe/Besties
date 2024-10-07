using Telegram.Bot.Types;
using Telegram.Bot;
using System.Text;
using Telegram.Bot.Types.Enums;
using Bot.Commands;

namespace Bot
{
	public class MessageHandler
	{
		public delegate void LogHandler(string message);
		public static event LogHandler Log;
		public static async void DefaultHandler(ITelegramBotClient client, Update update, CancellationToken ct)
		{
			if (update.Message != null)
			{
				if (update.Message.Text == "/start")
				{
					GuestMode.OnStart(client, update, ct);
				}
				else if (update.Message.Type == MessageType.Text)
				{
					var res = Peredraznit(update.Message.Text!);
					client.SendTextMessageAsync(
					update.Message.Chat.Id,
					text: $"{res}, бебебе");
					LogMessageInfo(update);
				}
			}
		}
		public static async void ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken ct)
		{
			Log?.Invoke(exception.Message);
		}

		private static string Peredraznit(string input)
		{
			var sb = new StringBuilder(input.ToLower());

			for (var i = 0; i < sb.Length; i += 2)
			{
				sb[i] = char.ToUpper(sb[i]);
			}
			return sb.ToString();
		}
		private static async Task StartingRoute(ITelegramBotClient client, Update update, CancellationToken ct)
		{

		}
		public static void LogMessageInfo(Update update)
		{
			Log?.Invoke($"Received message in {update.Message.Chat.Id}, {update.Message.Chat.Username}" +
				$": {update.Message.Text}, {update.Message.Chat.Type}");
		}
	}
}
