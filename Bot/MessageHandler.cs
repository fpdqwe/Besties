using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Bot.Commands;
using Bot.Resources;
using Telegram.Bot.Types.ReplyMarkups;
using Domain.Entities;
using System.Text;

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
					case UpdateType.CallbackQuery:
						BotOnQueryReceived(client, update.CallbackQuery);
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
			
			
			Log?.Invoke($"Receive message type {message.Type} in chat {message.Chat.Id}; {message.From.Username};");
			
			
			if (message.Type != MessageType.Text && message.Type != MessageType.Photo) return;

			var chat = await ChatManager.Find(message.Chat.Id, message.Chat.Username);

			if (chat.IsActive == false && chat.Card.IsActive == true)
			{
				SendMenu(client, chat);
				chat.SetReply(AwaitMenuAction);
				return;
			}
			if (message.Type == MessageType.Photo)
			{
				//foreach(var file in message.Photo)
				//{
				//	var photo = await client.GetFileAsync(file.FileId);

				//	using (var stream = new FileStream(strings.resourcesPath + $"{photo.FileId}.jpg",
				//		FileMode.CreateNew, FileAccess.Write))
				//	{
				//		await client.DownloadFileAsync(photo.FilePath, stream);

				//		await client.SendPhotoAsync(message.From.Id, photo, caption: "копия");
				//	}
				//}
				foreach (var photo in message.Photo)
				{
					Log?.Invoke($"Photo uploaded in chat {chat.Id}! {photo.FileId}; {photo.FileUniqueId}; {photo.FileSize}");
				}
				CardEdit.AwaitPhotoChange(client, chat, message.Photo);
			}
			switch (message.Text)
			{
				case "/start":
					chat.SetReply(MeetClient);
					chat.SetActive(true);
					chat.OnMessageReceived(client, message.Text);
					break;
				case "/editcatd":
					CardEdit.InitCardEditMode(client, chat);
					break;
				case "/search":
					Search.OfferCandidate(client, chat);
					break;
				default:
					chat.OnMessageReceived(client, message.Text);
					break;
			}
		}
		private static async Task BotOnQueryReceived(ITelegramBotClient client, CallbackQuery query)
		{
			Log?.Invoke($"Query {query.Id} from {query.From.Username} ({query.From.Id}: {query.Data})");
			
			if (query.Data[0] == '1')
			{
				var chat = await ChatManager.Find(query.From.Id);
				chat.SetReply(Search.GetOfferSenderReply);
				chat.OnMessageReceived(client, query.Data);
			}
			if (query.Data[0] == '2')
			{
				var recipient = await ChatManager.Find(Convert.ToInt64(Search.ReadSearchQuery(query.Data)[2]));
				recipient.SetReply(Search.GetOfferRecipientReply);
				recipient.OnMessageReceived(client, query.Data);
			}
		}
		public static async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken ct)
		{
			Log?.Invoke(exception.Message);
		}
		private static async Task MeetClient(ITelegramBotClient client, Chat sender, string message)
		{
			await client.SendTextMessageAsync(sender.Id, $"Привет, {sender.User.Username}!");
			if(sender.Card.IsActive == false)
			{
				await client.SendTextMessageAsync(sender.Id, $"Сейчас твоя анкета не участвует в поиске");
			}
			SendMenu(client, sender);
		}
		private static async Task AwaitMenuAction(ITelegramBotClient client, Chat sender, string message)
		{
			if(message == strings.searchCommand)
			{
				if(sender.Card.IsActive == false)
				{
					await client.SendTextMessageAsync(sender.Id, "Невозможно запустить поиск с пустой анкетой, сначала отредактируйте её");
					return;
				}
				Search.OfferCandidate(client, sender);
			}
			if(message == strings.showMyCardCommand)
			{
				await SendCardPreview(client, sender);
			}
			if(message == strings.cardEditCommand)
			{
				CardEdit.InitCardEditMode(client, sender);
			}
		}
		public static async Task SendMenu(ITelegramBotClient client, Chat sender)
		{
			if(sender.IsActive == false) {sender.SetActive(true);}
			await client.SendTextMessageAsync(sender.Id, strings.menuText, replyMarkup:GetMenuReplyMarkup());
			
			sender.SetReply(AwaitMenuAction);
		}
		public static void LogMessage(string message)
		{
			Log?.Invoke($"{message}");
		}
		public static async Task SendCardPreview(ITelegramBotClient botClient, Chat sender)
		{
			var media = await ChatManager.GetCardPhoto(sender.Id);
			var sb = new StringBuilder();
			sb.AppendLine($"Имя: {sender.Card.Name}");
			sb.AppendLine($"Возраст: {sender.Card.Age}");
			switch (sender.Card.Gender)
			{
				case Gender.Male:
					sb.AppendLine("Пол: мужской");
					break;
				case Gender.Female:
					sb.AppendLine("Пол: женский");
					break;
				default:
					sb.AppendLine("Пол: засекречен");
					break;
			}
			switch (sender.Card.TargetGender)
			{
				case Gender.Male:
					sb.AppendLine("Пол партнёра: мужской");
					break;
				case Gender.Female:
					sb.AppendLine("Пол партнёра: женский");
					break;
				case Gender.NotSpecified:
					sb.AppendLine("Пол партнёра: не принципиально");
					break;
			}
			sb.AppendLine($"Регион - {sender.Card.Region}");
			sb.AppendLine("==========");
			sb.AppendLine($"Описание: {sender.Card.Description}");
			sb.AppendLine("==========");
			sb.AppendLine(CardEdit.GetHabbits(sender.Card, false));
			sb.AppendLine("==========");
			sb.AppendLine("Подтверждаем?");
			using (var memoryStream = new MemoryStream(media.Image))
			{
				var file = new InputFileStream(memoryStream, $"{media.Id}.jpg");
				await botClient.SendPhotoAsync(sender.Id, file, caption: sb.ToString());

			}
		}

		private static ReplyKeyboardMarkup GetMenuReplyMarkup()
		{
			var btns = new KeyboardButton[3];

			btns[0] = new KeyboardButton(strings.searchCommand);
			btns[1] = new KeyboardButton(strings.showMyCardCommand);
			btns[2] = new KeyboardButton(strings.cardEditCommand);

			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}
	}
}
