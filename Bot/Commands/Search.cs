using Domain.Entities;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
	public static class Search
	{
		public static async Task OfferCandidate(ITelegramBotClient botClient, Chat sender, string message)
		{
			var offer = await MessageHandler.ChatManager.GetOffer(sender);
			sender.SearchScopes.LastOffer = offer;
			await botClient.SendTextMessageAsync(sender.Id, GetOfferCard(offer), replyMarkup:GetOfferMarkup());
		}

		public static async Task GetOfferReply(ITelegramBotClient botClient, Chat sender, string message)
		{
			if (message == "Норм") await NotifyOffer(botClient, sender, sender.SearchScopes.LastOffer);
			if (message == "Не норм") sender.SearchScopes.SkippedIds.Add(sender.SearchScopes.LastOffer.Id);
		}

		public static async Task NotifyOffer(ITelegramBotClient botClient, Chat sender, Card offerCard)
		{
			var offerChat = await MessageHandler.ChatManager.Find(offerCard.Id, offerCard.Name);
			await botClient.SendTextMessageAsync(offerChat.Id, "Вы понравились одному человеку!\n" + GetOfferCard(sender.Card),
				replyMarkup: GetOfferMarkup());
			offerChat.SetReply(GetOfferReply);
		}

		public static async Task CompletePairing(ITelegramBotClient botClient, Chat p1, Chat p2)
		{
			await botClient.SendTextMessageAsync(p1.Id, $"Пэйринг удался! напишите @{p2.User.Username}");
			await botClient.SendTextMessageAsync(p2.Id, $"Пэйринг удался! напишите @{p1.User.Username}");
		}

		public static string GetOfferCard(Card offer)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Имя: {offer.Name}");
			sb.AppendLine($"Возраст: {offer.Age}");
			sb.AppendLine("==========");
			sb.AppendLine($"Описание: {offer.Description}");
			sb.AppendLine("==========");
			sb.AppendLine(GetOffersHabbits(offer));
			sb.AppendLine("==========");
			sb.AppendLine();
			sb.AppendLine("Подтверждаем?");

			return sb.ToString();
		}
		private static string GetOffersHabbits(Card offer)
		{
			var sb = new StringBuilder();

			if (offer.IsDrinking) sb.AppendLine("Употребляет алкоголь");
			else sb.AppendLine("Не употребляет алкоголь");
			if (offer.IsSmoking) sb.AppendLine("Употребляет никотин");
			else sb.AppendLine("Употребляет никотин");
			if (offer.AnimalsLover) sb.AppendLine("Любит животных");
			else sb.AppendLine("Не любит животных");

			return sb.ToString();
		}
		private static InlineKeyboardMarkup GetOfferMarkup()
		{
			var btns = new InlineKeyboardButton[2]
			{
				"Норм",
				"Не норм"
			};

			return new InlineKeyboardMarkup(btns);
		}
	}
}
