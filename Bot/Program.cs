using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Bot
{
	public class Program
	{
		public static TelegramBotClient Init()
		{
			using var cts = new CancellationTokenSource();
			var botToken = Resources.strings.botToken;
			var bot = new TelegramBotClient(botToken);

			var ro = new ReceiverOptions
			{
				AllowedUpdates = []
			};

			bot.StartReceiving(
				MessageHandler.DefaultHandler,
				MessageHandler.ErrorHandler,
				ro,
				cts.Token
				);
			return bot;
		}
	}
}
