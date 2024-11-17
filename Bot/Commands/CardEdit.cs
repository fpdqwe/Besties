using Bot.Resources;
using Bot.Utilities;
using Domain.Entities;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
	public static class CardEdit
	{
		// Methods
		public static async Task InitCardEditMode(ITelegramBotClient botClient, Chat sender)
		{
			await botClient.SendTextMessageAsync(sender.Id, "Чтож, давай отредактируем твою анкету!\nДля начала введи свой возраст");
			if (sender.NewCard == null) OnEnteringCardEditMode(sender);
			sender.SetReply(OnCardAgeChange);
		}
		public static async Task OnCardAgeChange(ITelegramBotClient botClient, Chat sender, string message)
		{
			int age = sender.Card.Age;
			if(int.TryParse(message, out age))
			{
				if (age < 18)
				{
					await botClient.SendTextMessageAsync(sender.Id, "Недопустимый возраст");
					return;
				}
				else
				{
					await botClient.SendTextMessageAsync(sender.Id, $"Ваш возраст теперь {age} вместо {sender.Card.Age}");
					await botClient.SendTextMessageAsync(sender.Id, "Теперь определимся с полом", replyMarkup: GetGenderMarkup());
					sender.NewCard.Age = age;
					sender.SetReply(OnGenderChange);
					return;
				}
			}
			await botClient.SendTextMessageAsync(sender.Id, "Не удалость перевести возраст в число :(");
		}
		public static async Task OnGenderChange(ITelegramBotClient botClient, Chat sender, string message)
		{
			switch (message)
			{
				case "Я парень":
					sender.NewCard.Gender = Gender.Male;
					break;
				case "Я девушка":
					sender.NewCard.Gender = Gender.Female;
					break;
				case "Не указывать":
					sender.NewCard.Gender = Gender.NotSpecified;
					break;
				default:
					sender.NewCard.Gender = Gender.NotSpecified;
					break;
			}

			await botClient.SendTextMessageAsync(sender.Id, "Кого ищешь?", replyMarkup:GetTargetGenderMarkup());
			sender.SetReply(OnTargetGenderChange);
		}
		public static async Task OnTargetGenderChange(ITelegramBotClient botClient, Chat sender, string message)
		{
			switch (message)
			{
				case "Ищу парня":
					sender.NewCard.TargetGender = Gender.Male;
					break;
				case "Ищу девушку":
					sender.NewCard.TargetGender = Gender.Female;
					break;
				default:
					sender.NewCard.TargetGender = Gender.NotSpecified;
					break;
			}

			await botClient.SendTextMessageAsync(sender.Id, "Из какого ты города?");
			sender.SetReply(OnRegionChange);
		}
		public static async Task OnRegionChange(ITelegramBotClient botClient, Chat sender, string message)
		{
			var region = ResourceReader.GetRegion(message);
			sender.NewCard.Region = region;
			await botClient.SendTextMessageAsync(sender.Id, "Как тебя зовут?", replyMarkup: null);
			sender.SetReply(AwaitNameChange);
		}
		public static async Task AwaitDescriptionChange(ITelegramBotClient botClient, Chat sender, string message)
		{
			await botClient.SendTextMessageAsync(sender.Id, $"Описание установлено - {message}, вместо {sender.Card.Description}");
			sender.NewCard.Description = message;
			await botClient.SendTextMessageAsync(sender.Id, "Теперь пройдёмся по вредным привычкам...",
				replyMarkup:GetHabbitsMarkup(sender.Card));
			sender.SetReply(AwaitHabbitsCommand);
		}
		public static async Task AwaitNameChange(ITelegramBotClient botClient, Chat sender, string message)
		{
			await botClient.SendTextMessageAsync(sender.Id, $"Имя изменено - {message}, вместо {sender.Card.Name}");
			sender.NewCard.Name = message;
			await botClient.SendTextMessageAsync(sender.Id, "Теперь самое главное - описание вашей анкеты. Расскажите немного о себе.");
			sender.SetReply(AwaitDescriptionChange);
		}
		public static async Task AwaitHabbitsCommand(ITelegramBotClient botClient, Chat sender, string message)
		{
			if(message == strings.alcoMarkerNegative)
			{
				sender.NewCard.IsDrinking = false;
				await botClient.SendTextMessageAsync(sender.Id, "Так и запишем, вы не пьёте",
					replyMarkup: GetHabbitsMarkup(sender.NewCard));
				return;
			}
			if(message == strings.alcoMarkerPositive)
			{
				sender.NewCard.IsDrinking = true;
				await botClient.SendTextMessageAsync(sender.Id, "Не осуждаем, времена тяжёлые...",
					replyMarkup: GetHabbitsMarkup(sender.NewCard));
				return;
			}
			if(message == strings.smokingMarkerNegative)
			{
				sender.NewCard.IsSmoking = false;
				await botClient.SendTextMessageAsync(sender.Id, "Сильно...",
					replyMarkup: GetHabbitsMarkup(sender.NewCard));
				return;
			}
			if(message == strings.smokingMarkerPositive)
			{
				sender.NewCard.IsSmoking = true;
				await botClient.SendTextMessageAsync(sender.Id, "Учтено",
					replyMarkup: GetHabbitsMarkup(sender.NewCard));
				return;
			}
			if(message == strings.menuEndCommand)
			{
				await botClient.SendTextMessageAsync(sender.Id, GetHabbits(sender.NewCard),
					replyMarkup: GetConfirmMarkup());
				return;
			}
			if(message == strings.confirmationNegative)
			{
				await botClient.SendTextMessageAsync(sender.Id, "Жду указаний",
					replyMarkup: GetHabbitsMarkup(sender.NewCard));
				return;
			}
			if(message == strings.confirmationPositive)
			{
				await botClient.SendTextMessageAsync(sender.Id, "Хорошо, в итоге ваша анкета выглядит сейчас так",
					replyMarkup:null);
				sender.SetReply(ConfirmEditedCard);
				await SendCardPreview(botClient, sender);
			}
		}
		public static async Task ConfirmEditedCard(ITelegramBotClient botClient, Chat sender, string message)
		{
			if(message == strings.confirmationNegative)
			{
				await botClient.SendTextMessageAsync(sender.Id, "Окей, давай начнём сначала", replyMarkup: null);
				await botClient.SendTextMessageAsync(sender.Id, "Сколько тебе лет?");
				sender.SetReply(OnCardAgeChange);
				return;
			}
			if (message == strings.confirmationPositive)
			{
				await botClient.SendTextMessageAsync(sender.Id, "Изменения подтверждены");
				await OnCardEditCompleted(sender);
				await MessageHandler.SendMenu(botClient, sender);
			}
		}
		public static async Task SendCardPreview(ITelegramBotClient botClient, Chat sender)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Имя: {sender.NewCard.Name}");
			sb.AppendLine($"Возраст: {sender.NewCard.Age}");
			switch (sender.NewCard.Gender)
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
			switch (sender.NewCard.TargetGender)
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
			sb.AppendLine($"Регион - {sender.NewCard.Region}");
			sb.AppendLine("==========");
			sb.AppendLine($"Описание: {sender.NewCard.Description}");
			sb.AppendLine("==========");
			sb.AppendLine(GetHabbits(sender.NewCard, false));
			sb.AppendLine("==========");
			sb.AppendLine();
			sb.AppendLine("Подтверждаем?");

			await botClient.SendTextMessageAsync(sender.Id, sb.ToString(), replyMarkup:GetConfirmMarkup());
		}
		// Keyboard Markups
		private static ReplyKeyboardMarkup GetHabbitsMarkup(Card card)
		{
			var btns = new KeyboardButton[3];

			if (card.IsDrinking) btns[0] = new KeyboardButton(strings.alcoMarkerNegative);
			else btns[0] = new KeyboardButton(strings.alcoMarkerPositive);
			if (card.IsSmoking) btns[1] = new KeyboardButton(strings.smokingMarkerNegative);
			else btns[1] = new KeyboardButton(strings.smokingMarkerPositive);
			btns[2] = new KeyboardButton(strings.menuEndCommand);


			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}
		private static ReplyKeyboardMarkup GetConfirmMarkup()
		{
			var btns = new KeyboardButton[2]
			{
				strings.confirmationPositive,
				strings.confirmationNegative,
			};

			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return new ReplyKeyboardMarkup(btns);
		}
		private static ReplyKeyboardMarkup GetGenderMarkup()
		{
			var btns = new KeyboardButton[]
			{
				"Я парень",
				"Я девушка",
				"Не указывать"
			};

			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}
		private static ReplyKeyboardMarkup GetTargetGenderMarkup()
		{
			var btns = new KeyboardButton[]
			{
				"Ищу парня",
				"Ищу девушку",
				"Не принципиально"
			};

			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return new ReplyKeyboardMarkup(btns);
		}
		// Utils
		private static string GetHabbits(Card card, bool withPrefix = true)
		{
			var sb = new StringBuilder();

			if (withPrefix) sb.AppendLine("И так, что мы в итоге имеем:");


			if (card.IsDrinking) sb.AppendLine("Употребляете алкоголь");
			else sb.AppendLine("Не употребляете алкоголь");
			if (card.IsSmoking) sb.AppendLine("Употребляете никотин");
			else sb.AppendLine("Употребляете никотин");

            if (withPrefix) sb.AppendLine("Подтверждаем?");

			return sb.ToString();
		}
		private static void OnEnteringCardEditMode(Chat chat)
		{
			chat.NewCard = new Card()
			{
				Id = chat.Card.Id,
				Age = chat.Card.Age,
				Description = chat.Card.Description,
				IsDrinking = chat.Card.IsDrinking,
				IsSmoking = chat.Card.IsSmoking,		
				HealthyMode = chat.Card.HealthyMode,
				IsActive = false
			};
		}
		private static async Task OnCardEditCompleted(Chat chat)
		{
			chat.NewCard.IsActive = true;
			await ChatManager.ApplyCardChanges(chat);
		}
	}
}
