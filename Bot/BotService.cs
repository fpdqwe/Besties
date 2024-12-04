using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Bot.Branches;
using Bot.Resources;
using System.Text;
using Domain.Enums;
using Bot.Types;

namespace Bot
{
	public class BotService
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
						await BotOnMessageReceived(client, update);
						break;
					case UpdateType.CallbackQuery:
						await BotOnQueryReceived(client, update);
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
		private static async Task BotOnMessageReceived(ITelegramBotClient client, Update update)
		{
			if (update.Message == null) throw new NullReferenceException(nameof(update.Message));

			var chat = await ChatManager.Find(update.Message.From.Id, update.Message.Chat.Username);

			var chatUnit = new Unit(client, chat, update);
			if (chat.IsActive == false && chat.User.ChatMode == ChatMode.ExistingUser)
			{
				await Menu.MeetUser(chatUnit);
				chat.SetReply(Menu.AwaitMenuAction);
				return;
			}
			else if (chat.IsActive == false && chat.User.ChatMode == ChatMode.Guest)
			{
				await Menu.MeetUser(chatUnit);
				return;
			}
			await CheckForCommandsAsync(chatUnit);
		}
		public static async Task CheckForCommandsAsync(Unit unit)
		{
			if (unit.MesText == strings.MenuTelegramCommand && unit.Chat.User.ChatMode == ChatMode.ExistingUser)
			{
				await Menu.MeetUser(unit);
				unit.Chat.SetActive(true);
				await unit.Chat.OnMessageReceived(unit);
			}
			else if (unit.MesText == strings.MenuTelegramCommand && unit.Chat.User.ChatMode == ChatMode.Guest)
			{
				await Menu.MeetNewUser(unit);
				unit.Chat.SetActive(true);
				await unit.Chat.OnMessageReceived(unit);
			}
			else if (unit.MesText == strings.ShowCardTelegramCommand)
			{
				await CardService.CardRemakeAsync(unit.Client, unit.Chat);
			}
			else if (unit.MesText == strings.SearchTelegramCommand)
			{
				await Search.OfferCandidate(unit.Client, unit.Chat);
			}
			else
			{
				await unit.Chat.OnMessageReceived(unit);
			}
		}
		private static async Task BotOnQueryReceived(ITelegramBotClient client, Update update)
		{
			var query = update.CallbackQuery;
			Log?.Invoke($"Query {query.Id} from {query.From.Username} ({query.From.Id}: {query.Data})");

			var data = query.Data.Split(';');

			if (data[0] == strings.QueryCode_OfferSender)
			{
				var chat = await ChatManager.Find(query.From.Id);
				chat.SetReply(Search.GetOfferSenderReply);
				chat.OnMessageReceived(client, update);
			}
			if (data[0] == strings.QueryCode_OfferRecepient)
			{
				var recipient = await ChatManager.Find(Convert.ToInt64(Search.ReadSearchQuery(query.Data)[2]));
				if(!recipient.IsActive) recipient.SetReply(Search.GetOfferRecipientReply);
				recipient.OnMessageReceived(client, update);
			}
		}
		public static async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken ct)
		{
			Log?.Invoke(exception.Message);
		}
		public static void LogMessage(string message)
		{
			Log?.Invoke($"{message}");
		}
	}
}
