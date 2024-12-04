using Bot.Resources;
using Bot.Types;
using Domain.Entities;
using Domain.Enums;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Branches
{
	public static class Search
	{
		public static async Task OfferCandidate(ITelegramBotClient client, Chat sender)
		{
			if (sender.User.ChatMode == ChatMode.Guest)
			{
				await client.SendTextMessageAsync(sender.Id, strings.EmptyCardSearchError);
				await Menu.SendMenu(client, sender);
			}
			if (sender.SearchScopes.CurrentOfferCard == null)
			{
				await OnEmptyCardPool(client, sender);
				return;
			}
			else if (!await TrySendOfferAsync(client, sender.SearchScopes.CurrentOfferCard, sender))
			{
				await sender.SearchScopes.SkipCurrentOffer(sender, false);
				await OfferCandidate(client, sender);
			}

			sender.SetReply(GetOfferSenderReply);
		}
		public static async Task SendIncomingOffer(Unit unit)
		{
			if (unit.IncomingOffersCount <= 0)
			{
				await Menu.SendMenu(unit.Client, unit.Chat);
				unit.NextHandler = Menu.AwaitMenuAction;
				return;
			}

			var offerSender = await BotService.ChatManager.Find(unit.Chat.SearchScopes.CurrentIncomingOffer.SenderId);

			await SendDefferedOfferToRecipientAsync(offerSender, unit);
		}
		public static async Task AwaitSearchConfirm(Unit unit)
		{
			if (unit.Type != UpdateType.Message) throw new ArgumentException(nameof(unit.Type));
			if (unit.MesText == strings.ApproveShort)
			{
				await OfferCandidate(unit.Client, unit.Chat);
			}
			else if (unit.MesText == strings.RejectShort)
			{
				await Menu.SendMenu(unit.Client, unit.Chat);
			}
			else
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.InvalidAnswerError);
			}
		}
		public static async Task GetOfferSenderReply(Unit unit)
		{
			if (unit.Type != UpdateType.CallbackQuery) throw new ArgumentException(nameof(unit.Type));
			
			var queryData = ReadSearchQuery(unit.Callback.Data);

			if (queryData[3] == "1")
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.OfferSenderApproveReact,
					replyMarkup: GetConfirmMarkup());

				// Skip offer должен выполнятся до уведомления получателя, т.к. сохраняет оффер в бд
				// Иначе, чат получателя инициализуется без информации об оффере и не сможет сохранить ответ
				await unit.Chat.SearchScopes.SkipCurrentOffer(unit.Chat, true);
				NotifyOfferRecipient(unit);
				unit.NextHandler = AwaitSearchConfirm;
			}
			if (queryData[3] == "0")
			{
				await unit.Client.AnswerCallbackQueryAsync(unit.Callback.Id,
					strings.OfferSenderRejectReact,
					cacheTime: 3);
				await unit.Chat.SearchScopes.SkipCurrentOffer(unit.Chat, false);
				await OfferCandidate(unit.Client, unit.Chat);
			}
		}
		public static async Task GetOfferRecipientReply(Unit recipient)
		{
			if (recipient.Type != UpdateType.CallbackQuery) throw new ArgumentException(nameof(recipient.Type));

			var queryData = ReadSearchQuery(recipient.Callback.Data);
			if (queryData[0] != strings.QueryCode_OfferRecepient) return;

			if (queryData[3] == "1")
			{
				var offerSender = await BotService.ChatManager.Find(Convert.ToInt64(queryData[1]));
				await recipient.Client.SendTextMessageAsync(recipient.Id, "Приятного общения!",
					replyMarkup: GetOfferSuccessMarkup(offerSender));

				recipient.Chat.SearchScopes.SkipIncomingOffer(offerSender.Id, true);
				NotifyOfferSender(recipient);
				recipient.NextHandler = AwaitSearchConfirm;
			}
			if (queryData[3] == "0")
			{
				recipient.Chat.SearchScopes.SkipIncomingOffer(int.Parse(queryData[1]), false);
				await OfferCandidate(recipient.Client, recipient.Chat);
			}
		}
		public static async Task GetDeferredRecepientReply(Unit recipient)
		{
			if (recipient.Type != UpdateType.CallbackQuery) throw new ArgumentException(nameof(recipient.Type));

			var queryData = ReadSearchQuery(recipient.Callback.Data);
			if (queryData[0] != strings.QueryCode_OfferRecepient) return;

			if (queryData[3] == "1")
			{
				var offerSender = await BotService.ChatManager.Find(Convert.ToInt64(queryData[1]));
				await recipient.Client.SendTextMessageAsync(recipient.Id, "Приятного общения!",
					replyMarkup: GetOfferSuccessMarkup(offerSender));

				recipient.Chat.SearchScopes.SkipIncomingOffer(offerSender.Id, true);
				NotifyOfferSender(recipient);
			}
			if (queryData[3] == "0")
			{
				recipient.Chat.SearchScopes.SkipIncomingOffer(int.Parse(queryData[1]), false);
			}

			await SendIncomingOffer(recipient);
		}
		public static async Task NotifyOfferRecipient(Unit unit)
		{
			var queryData = ReadSearchQuery(unit.Callback.Data);

			var offerRecipient = await BotService.ChatManager.Find(Convert.ToInt64(queryData[2]));
			await BotService.ChatManager.UpdateIncomingOffers(offerRecipient);

			await SendOfferToRecipientAsync(unit, offerRecipient);

			offerRecipient.SetReply(GetOfferRecipientReply);
		}
		public static async Task NotifyOfferSender(Unit unit)
		{
			if (unit.Type != UpdateType.CallbackQuery) throw new ArgumentException(nameof(unit.Type));

			var queryData = ReadSearchQuery(unit.Callback.Data);

			var offerSender = await BotService.ChatManager.Find(Convert.ToInt64(queryData[1]));

			await unit.Client.SendTextMessageAsync(offerSender.Id, 
				strings.SenderNotificationMessage,
				replyMarkup: GetOfferSuccessMarkup(unit.Chat));

			Menu.SendMenu(unit.Client, unit.Chat);
		}
		public static async Task<bool> TrySendOfferAsync(ITelegramBotClient client, Card offer, Chat sender)
		{
			var media = new CardMedia();
			try
			{
				media = await BotService.ChatManager.GetCardPhoto(offer.Id);
			}
			catch (Exception ex)
			{
				BotService.LogMessage(ex.Message);
				if (ex.InnerException != null) { BotService.LogMessage("Inner: " + ex.InnerException.Message); }
				BotService.ChatManager.SetCardStatus(offer, false);
				return false;
			}
			var sb = new StringBuilder();
			sb.AppendLine($"{offer.Name}, {offer.Age}, {Utilities.ResourceReader.GetRegionName(offer.Region)}");
			sb.AppendLine(strings.Separator);
			sb.AppendLine(offer.Description);
			sb.AppendLine(strings.Separator);
			if (offer.IsSmoking) sb.AppendLine(strings.ParamSmoking + strings.ApproveShort);
			else sb.AppendLine(strings.ParamSmoking + strings.RejectShort);
			if (offer.IsDrinking) sb.AppendLine(strings.ParamDrinking + strings.ApproveShort);
			else sb.AppendLine(strings.ParamDrinking + strings.RejectShort);
			sb.AppendLine(strings.Separator);
			using (var memoryStream = new MemoryStream(media.Image))
			{
				var file = new InputFileStream(memoryStream, $"{media.Id}.jpg");
				await client.SendPhotoAsync(sender.Id,
					file,
					caption: sb.ToString(),
					replyMarkup: GetOfferSenderMarkup(sender, offer));
			}
			return true;
		}
		public static async Task SendOfferToRecipientAsync(Unit sender, Chat recipient)
		{
			await SendOfferToRecipientAsync(sender.Client, sender.Chat, recipient);

			recipient.SetReply(GetOfferRecipientReply);
		}
		private static async Task SendDefferedOfferToRecipientAsync(Chat sender, Unit recepient)
		{
			await SendOfferToRecipientAsync(recepient.Client, sender, recepient.Chat);

			recepient.NextHandler = GetDeferredRecepientReply;
		}
		private static async Task SendOfferToRecipientAsync(ITelegramBotClient client, Chat sender, Chat recipient)
		{
			var sb = new StringBuilder();

			sb.AppendLine($"{sender.Card.Name}, {sender.Card.Age}, {Utilities.ResourceReader.GetRegionName(sender.Card.Region)}");
			sb.AppendLine(strings.Separator);
			sb.AppendLine(strings.ParamDescription + sender.Card.Description);
			if (sender.Card.IsSmoking) sb.AppendLine(strings.ParamSmoking + strings.ApproveShort);
			else sb.AppendLine(strings.ParamSmoking + strings.RejectShort);
			if (sender.Card.IsDrinking) sb.AppendLine(strings.ParamDrinking + strings.ApproveShort);
			else sb.AppendLine(strings.ParamDrinking + strings.RejectShort);

			var media = await BotService.ChatManager.GetCardPhoto(sender.Id);
			using (var memoryStream = new MemoryStream(media.Image))
			{
				var file = new InputFileStream(memoryStream, $"{media.Id}.jpg");
				await client.SendPhotoAsync(recipient.Id,
					file,
					caption: sb.ToString(),
					replyMarkup: GetOfferRecipientMarkup(sender, recipient.Card));
			}
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

		// Exceptions
		public static async Task OnEmptyCardPool(ITelegramBotClient client, Chat chat)
		{
			await client.SendTextMessageAsync(chat.Id,
					strings.EmptyCardPoolError);
			await Menu.SendMenu(client, chat);
			return;
		}

		// Markups
		private static InlineKeyboardMarkup GetOfferSenderMarkup(Chat sender, Card offer)
		{
			InlineKeyboardMarkup markup = new(
				new[]
				{
					InlineKeyboardButton.WithCallbackData(strings.OfferApprove,
					$"{strings.QueryCode_OfferSender};{sender.Id};{offer.Id};1"),

					InlineKeyboardButton.WithCallbackData(strings.OfferReject,
					$"{strings.QueryCode_OfferSender};{sender.Id};{offer.Id};0")
				}
			);
			return markup;
		}
		private static InlineKeyboardMarkup GetOfferRecipientMarkup(Chat sender, Card offer)
		{
			InlineKeyboardMarkup markup = new( 
				new[]
				{
					InlineKeyboardButton.WithCallbackData(strings.OfferApprove,
					$"{strings.QueryCode_OfferRecepient};{sender.Id};{offer.Id};1"),

					InlineKeyboardButton.WithCallbackData(strings.OfferReject,
					$"{strings.QueryCode_OfferRecepient};{sender.Id};{offer.Id};0")
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

		// Query
		public static string[] ReadSearchQuery(string data)
		{
			var values = data.Split(";");
			if (values.Length > 4)
			{
				BotService.LogMessage("Incorrect query received: more than 4 arguments in query");
			}
			BotService.LogMessage($"Query type - {values[0]}; Sender - {values[1]}; Recipient - {values[2]}; Value: {values[3]}");
			return values.ToArray();
		}
	}
}
