using Bot.Resources;
using Bot.Types;
using Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Branches
{
	public static class Menu
	{
		public static async Task MeetUser(Unit unit)
		{
			await unit.Client.SendTextMessageAsync(unit.Id, $"Привет, {unit.Card.Name}!");
			if (unit.Card.IsActive == false && unit.Chat.User.ChatMode == ChatMode.Guest)
			{
				await unit.Client.SendTextMessageAsync(unit.Id, strings.EmptyCardError);
				await CardService.CardRemakeAsync(unit.Client, unit.Chat);
				unit.Chat.SetActive(true);
				return;
			}
			if (unit.Card.IsActive == false && unit.Chat.User.ChatMode == ChatMode.ExistingUser)
			{
				await unit.Client.SendTextMessageAsync(unit.Id, strings.EmptyCardError);
			}
			if (unit.IncomingOffersCount > 0)
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					$"Вы понравились {unit.IncomingOffersCount} людям");
				await SendMenuWithOffers(unit.Client, unit.Chat);
				return;
			}
			await SendMenu(unit.Client, unit.Chat);
		}
		public static async Task MeetNewUser(Unit unit)
		{
			if (unit.Update.Message.From.Username != null)
			{
				await unit.Client.SendTextMessageAsync(unit.Id, $"Привет, {unit.Chat.User.Username}!");
			}
			else
			{
				await unit.Client.SendTextMessageAsync(unit.Id, $"Привет, я бот для знакомств");
			}
			
		}
		public static async Task AwaitMenuAction(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			if (unit.MesText == strings.LookIncomingOffersCommand && unit.IncomingOffersCount > 0)
			{
				await Search.SendIncomingOffer(unit);
			}
			if (unit.MesText == strings.SearchCommand)
			{
				if (unit.Card.IsActive == false)
				{
					await unit.Client.SendTextMessageAsync(unit.Id, strings.EmptyCardSearchError);
					return;
				}
				if (unit.Chat.SearchScopes.CurrentOfferCard == null)
				{
					await unit.Chat.SearchScopes.SkipCurrentOffer(unit.Chat, false);
				}
				await Search.OfferCandidate(unit.Client, unit.Chat);
			}
			if (unit.MesText == strings.ShowCardCommand)
			{
				await CardService.SendCardPreview(unit);
			}
		}
		public static async Task SendMenu(ITelegramBotClient client, Chat sender)
		{
			if (sender.IsActive == false) { sender.SetActive(true); }
			await client.SendTextMessageAsync(sender.Id,
				strings.MenuMessageText,
				replyMarkup: GetMenuReplyMarkup());

			sender.SetReply(AwaitMenuAction);
		}
		private static async Task SendMenuWithOffers(ITelegramBotClient client, Chat sender)
		{
			if (sender.IsActive == false) { sender.SetActive(true); }
			await client.SendTextMessageAsync(sender.Id,
				strings.MenuMessageText,
				replyMarkup: GetMenuWithOffersMarkup());

			sender.SetReply(AwaitMenuAction);
		}

		// Reply markups
		private static ReplyKeyboardMarkup GetMenuReplyMarkup()
		{
			return new ReplyKeyboardMarkup(new KeyboardButton[]
			{
				new KeyboardButton(strings.SearchCommand),
				new KeyboardButton(strings.ShowCardCommand),
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = true,
			};
		}
		private static ReplyKeyboardMarkup GetMenuWithOffersMarkup()
		{
			return new ReplyKeyboardMarkup(new KeyboardButton[][]
			{
				new KeyboardButton[]
				{
					new KeyboardButton(strings.LookIncomingOffersCommand)
				},
				new KeyboardButton[] {
					new KeyboardButton(strings.SearchCommand),
					new KeyboardButton(strings.ShowCardCommand)
				}
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = true,
			};
		}
	}
}
