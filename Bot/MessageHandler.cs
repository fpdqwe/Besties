﻿using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Bot.Commands;
using Bot.Resources;
using Telegram.Bot.Types.ReplyMarkups;

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
			
			if (message.Type == MessageType.Photo)
			{
				foreach(var file in message.Photo)
				{
					var photo = await client.GetFileAsync(file.FileId);
					
					using (var stream = new FileStream(strings.resourcesPath + $"{photo.FileId}.jpg",
						FileMode.CreateNew, FileAccess.Write))
					{
						await client.DownloadFileAsync(photo.FilePath, stream);
						
						// await client.SendPhotoAsync(message.From.Id, photo, caption: "копия");
					}
					
					
				}

			}
			if (message.Type != MessageType.Text) return;

			var chat = await ChatManager.Find(message.Chat.Id, message.Chat.Username);

			if (chat.IsActive == false && chat.Card.IsActive == true)
			{
				SendMenu(client, chat);
				chat.SetReply(AwaitMenuAction);
				return;
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
			if(message == strings.searchParamsCommand)
			{
				// Nado dodelat'
				await client.SendTextMessageAsync(sender.Id, "Функция пока не доступна");
				await SendMenu(client, sender);
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

		private static ReplyKeyboardMarkup GetMenuReplyMarkup()
		{
			var btns = new KeyboardButton[3];

			btns[0] = new KeyboardButton(strings.searchCommand);
			btns[1] = new KeyboardButton(strings.searchParamsCommand);
			btns[2] = new KeyboardButton(strings.cardEditCommand);

			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}
	}
}
