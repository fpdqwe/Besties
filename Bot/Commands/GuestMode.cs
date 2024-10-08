using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Client = Domain.Entities.User;
using DAL.Repositories;
using DAL;

namespace Bot.Commands
{
	public class GuestMode
	{
		public static async void OnStart(ITelegramBotClient client, Update update, CancellationToken ct)
		{
			MessageHandler.LogMessageInfo(update);
			RecognizeUser(update);
			
			
			client.SendTextMessageAsync(
				update.Message.Chat.Id,
				text: SelectGreeting(update.Message.Chat.FirstName));
			client.SendTextMessageAsync(
				update.Message.Chat.Id,
				text: Resources.strings.greetingHookRu);
			
		}

		private static async Task<Client> RecognizeUser(Update update)
		{
			
			var repos = new UserRepository(new ContextManager());
			var user = await repos.Find(update.Message.Chat.Id);
			if (user == null)
			{
				user = await repos.CreateNewUser(
					update.Message.Chat.Id,
					update.Message.Chat.Username,
					Domain.Enums.ChatMode.GuestPrivate);
			}
			return user;
		}
		private static string SelectGreeting(string username)
		{
			var rnd = new Random();
			var num = rnd.Next(0, 99);
			if (num >= 0 && num <= 19)
			{
				return $"{Resources.strings.greetingRu1} {username}!";
			}
			if (num >= 20 && num <= 39)
			{
				return $"{Resources.strings.greetingRu2} {username}!";
			}
			if (num >= 40 && num <= 59)
			{
				return $"{Resources.strings.greetingRu2} {username}!";
			}
			if (num >= 60 && num <= 79)
			{
				return $"{Resources.strings.greetingRu2} {username}!";
			}
			if (num >= 80 && num <= 99)
			{
				return $"{Resources.strings.greetingRu2} {username}!";
			}
			return "бля...";
		}
	}
}
