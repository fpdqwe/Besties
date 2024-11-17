using Domain.Entities;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
	public static class Search
	{
		public static async Task OfferCandidate(ITelegramBotClient botClient, Chat sender)
		{
			MessageHandler.LogMessage($"Search requested in chat {sender.Id}.");
			MessageHandler.LogMessage($"Skipped users:\n");
			foreach (var id in sender.SearchScopes.SkippedIds)
			{
				MessageHandler.LogMessage($"{id}");
			}
			var offer = await MessageHandler.ChatManager.GetOffer(sender);
			if (offer == null)
			{
				await botClient.SendTextMessageAsync(sender.Id, "На данный момент нет подходящих вам анкет, попробуйте позже");
				MessageHandler.SendMenu(botClient, sender);
				return;
			}
			sender.SearchScopes.LastOffer = offer;
			sender.SearchScopes.SkippedIds.Add(offer.Id);
			await botClient.SendTextMessageAsync(sender.Id, GetOfferCard(offer), 
				replyMarkup:GetOfferSenderMarkup(sender, offer));
			sender.SetReply(GetOfferSenderReply);
		}

		public static async Task AwaitSearchConfirm(ITelegramBotClient botClient, Chat sender, string message)
		{
			switch (message)
			{
				case "Да":
					OfferCandidate(botClient, sender); 
					break;
				case "Нет":
					MessageHandler.SendMenu(botClient, sender); 
					break;
			}
		}
		public static async Task GetOfferSenderReply(ITelegramBotClient botClient, Chat sender, string queryResult)
		{
			var queryData = ReadSearchQuery(queryResult);
			if (queryData[3] == "1")
			{
				await botClient.SendTextMessageAsync(sender.Id, "Лайк отправлен, продолжаем просмотр?",
					replyMarkup: GetConfirmMarkup());
				NotifyOfferRecipient(botClient, queryResult);
				sender.SetReply(AwaitSearchConfirm);
			}
			if (queryData[3] == "0")
			{
				OfferCandidate(botClient, sender);
			}
		}
		public static async Task GetOfferRecipientReply(ITelegramBotClient botClient, Chat recipient, string queryResult)
		{
			var queryData = ReadSearchQuery(queryResult);
			if (queryData[3] == "1")
			{
				var offerSender = await MessageHandler.ChatManager.Find(Convert.ToInt64(queryData[1]));
				await botClient.SendTextMessageAsync(recipient.Id, "Приятного общения!",
					replyMarkup: GetOfferSuccessMarkup(offerSender));
				
				NotifyOfferSender(botClient, queryResult);
				recipient.SetReply(AwaitSearchConfirm);
			}
			if (queryData[3] == "0")
			{
				recipient.SearchScopes.SkippedIds.Add(recipient.SearchScopes.LastOffer.Id);
				OfferCandidate(botClient, recipient);
			}
		}

		public static async Task NotifyOfferRecipient(ITelegramBotClient botClient, string query)
		{
			var queryData = ReadSearchQuery(query);
			var offerSender = await MessageHandler.ChatManager.Find(Convert.ToInt64(queryData[1]));
			var offerRecipient = await MessageHandler.ChatManager.Find(Convert.ToInt64(queryData[2]));
			
			
			
			await botClient.SendTextMessageAsync(offerRecipient.Id, "Вы понравились одному человеку!\n"
				+ GetOfferCard(await MessageHandler.ChatManager.GetOfferSender(Convert.ToInt64(queryData[1]))),
				replyMarkup: GetOfferRecipientMarkup(offerSender, offerRecipient.Card));

			MessageHandler.SendMenu(botClient, offerRecipient);
		}
		public static async Task NotifyOfferSender(ITelegramBotClient botClient, string query)
		{
			var queryData = ReadSearchQuery(query);
			var offerSender = await MessageHandler.ChatManager.Find(Convert.ToInt64(queryData[1]));
			var offerRecipient = await MessageHandler.ChatManager.Find(Convert.ToInt64(queryData[2]));

			await botClient.SendTextMessageAsync(offerSender.Id, "Вам ответили взаимностью!\n"
				+ GetOfferCard(offerRecipient.Card),
				replyMarkup: GetOfferSuccessMarkup(offerRecipient));

			MessageHandler.SendMenu(botClient, offerRecipient);
		}

		
		
		public static string GetOfferCard(Card offer)
		{
			var sb = new StringBuilder();
			sb.Append($"{offer.Name}");
			sb.Append($", {offer.Age}");
			sb.AppendLine($", region - {offer.Region}");
			sb.AppendLine("==========");
			sb.AppendLine($"Описание: {offer.Description}");
			sb.AppendLine("Вредные привычки:");
			sb.AppendLine(GetOffersHabbits(offer));
			sb.AppendLine("==========");

			return sb.ToString();
		}
		private static string GetOffersHabbits(Card offer)
		{
			var sb = new StringBuilder();

			if (offer.IsDrinking) sb.AppendLine("Культурно выпиваю");
			else sb.AppendLine("Не пью");
			if (offer.IsSmoking) sb.AppendLine("Курю");
			else sb.AppendLine("Не курю");

			return sb.ToString();
		}
		private static InlineKeyboardMarkup GetOfferSenderMarkup(Chat sender, Card offer)
		{
			InlineKeyboardMarkup markup = new(
				new[]
				{
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Норм",$"1;{sender.Id};{offer.Id};1")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Не норм",$"1;{sender.Id};{offer.Id};0")
					}
				}
			);
			return markup;
		}
		private static InlineKeyboardMarkup GetOfferRecipientMarkup(Chat sender, Card offer)
		{
			InlineKeyboardMarkup markup = new(
				new[]
				{
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Норм",$"2;{sender.Id};{offer.Id};1")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Не норм",$"2;{sender.Id};{offer.Id};0")
					}
				}
			);
			return markup;
		}
		private static InlineKeyboardMarkup GetOfferSuccessMarkup(Chat recipient)
		{
			InlineKeyboardMarkup markup = new (
				new[]
				{
					new[]
					{
						InlineKeyboardButton.WithUrl($"Написать {recipient.Card.Name}",recipient.GetUserLink())
					},
				}
			);
			return markup;
		}
		private static ReplyKeyboardMarkup GetConfirmMarkup()
		{
			var btns = new KeyboardButton[2]
			{
				"Да",
				"Нет"
			};

			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}

		// Query handlers
		public static string[] ReadSearchQuery(string data)
		{
			var values = data.Split(";");
			if (values.Length > 4)
			{
				MessageHandler.LogMessage("Incorrect query received: more than 4 arguments in query");
			}
			MessageHandler.LogMessage($"Query type - {values[0]}; Sender - {values[1]}; Recipient - {values[2]}; Value: {values[3]}");
			return values.ToArray();
		}
	}
}
