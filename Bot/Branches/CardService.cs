using Bot.Resources;
using Bot.Types;
using Bot.Utilities;
using Domain.Entities;
using Domain.Enums;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Branches
{
	public static class CardService
	{
		// Methods
		public static async Task HandleCardMenuReply(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;

			if (unit.MesText == strings.ChangeOnlyName)
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.NameQuestion,
					replyMarkup: GetNameMarkup(unit.Card));
				unit.NextHandler = ChangeOnlyNameAsync;
			}
			else if (unit.MesText == strings.ChangeOnlyDescription)
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.DescriptionQuestion,
					replyMarkup: GetDescriptionMarkup(unit.Card));
				unit.NextHandler = ChangeOnlyDescription;
			}
			else if (unit.MesText == strings.ChangeOnlyPhoto)
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.PhotoQuestion,
					replyMarkup: GetPhotoMarkup(unit.Card));
				unit.NextHandler = ChangeOnlyPhtoAsync;
			}
			else if (unit.MesText == strings.RemakeCardCommand)
			{
				await CardRemakeAsync(unit.Client, unit.Chat);
			}
			else if (unit.MesText == strings.MenuCommand)
			{
				if (!unit.NewCard.Equals(unit.Card))
					await ChatManager.ApplyCardChanges(unit.Chat);
				await Menu.SendMenu(unit.Client, unit.Chat);
			}
		}
		public static async Task CardRemakeAsync(ITelegramBotClient botClient, Chat sender)
		{
			await botClient.SendTextMessageAsync(sender.Id, "Чтож, давай отредактируем твою анкету!\nДля начала введи свой возраст");
			if (sender.NewCard == null) OnEnteringCardEditMode(sender);
			sender.SetReply(AgeChangeAsync);
		}
		private static async Task AgeChangeAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			if (unit.IngoreReply) { unit.IngoreReply = false; return; }
			int age = unit.Card.Age;
			if(int.TryParse(unit.MesText, out age))
			{
				if (age < 14 || age > 100)
				{
					await unit.Client.SendTextMessageAsync(unit.Id, 
						strings.InvalidAgeError,
						replyMarkup: GetAgeMarkup(unit.Card));
					return;
				}
				else
				{
					await unit.Client.SendTextMessageAsync(unit.Id, $"Ваш возраст теперь {age}.");
					unit.NewCard.Age = age;

					// Next question transition
					await unit.Client.SendTextMessageAsync(unit.Id, strings.GenderCommand, replyMarkup: GetGenderMarkup());
					unit.NextHandler = GenderChangeAsync;
					return;
				}
			}
			await unit.Client.SendTextMessageAsync(unit.Id, strings.CantParseAgeError);
		}
		private static async Task GenderChangeAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			if(unit.MesText == strings.GenderMaleConst) unit.NewCard.Gender = Gender.Male;
			else if (unit.MesText == strings.GenderFemaleConst) unit.NewCard.Gender = Gender.Female;
			else
			{
				await unit.Client.SendTextMessageAsync(unit.Id, strings.InvalidGenderError, replyMarkup: GetGenderMarkup());
				return;
			}

			// Next question transition
			await unit.Client.SendTextMessageAsync(unit.Id,
				strings.TargetGenderQuestion,
				replyMarkup: GetTargetGenderMarkup(unit.Card));
			unit.NextHandler = ChangeTargetGenderAsync;
		}
		private static async Task ChangeTargetGenderAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			if (unit.MesText == strings.TargetGenderMale) unit.NewCard.TargetGender = Gender.Male;
			else if (unit.MesText == strings.TargetGenderFemale) unit.NewCard.TargetGender = Gender.Female;
			else if (unit.MesText == strings.TargetGenderNotSpecified) unit.NewCard.TargetGender = Gender.NotSpecified;

			// Next question transition
			await unit.Client.SendTextMessageAsync(unit.Id,
				strings.PhotoQuestion,
				replyMarkup: GetPhotoMarkup(unit.Card));
			unit.NextHandler = PhotoChangeAsync;
		}
		private static async Task PhotoChangeAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Photo && unit.MesType != MessageType.Text) return;
			if (unit.MesType == MessageType.Text)
			{
				if(unit.MesText == strings.SkipChange)
				{
					await unit.Client.SendTextMessageAsync(unit.Id,
						strings.SkipChange,
						replyMarkup: GetNameMarkup(unit.Card));
				}
			}
			else {
				var photo = unit.Message.Photo;
				if (photo == null) { return; }
				var photoPath = await unit.Client.GetFileAsync(photo[photo.Length - 1].FileId);

				CardMedia cardMedia = new CardMedia();
				cardMedia.Id = unit.Id;

				using (var memoryStream = new MemoryStream())
				{
					await unit.Client.DownloadFileAsync(photoPath.FilePath, memoryStream);
					memoryStream.Seek(0, SeekOrigin.Begin);
					cardMedia.Image = memoryStream.ToArray();
				}

				await BotService.ChatManager.SaveCardPhoto(cardMedia);
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.SuccessPhotoChange);
				
			}
			// Next question transition
			await unit.Client.SendTextMessageAsync(unit.Id,
				strings.NameQuestion,
				replyMarkup: GetNameMarkup(unit.Card));
			unit.NextHandler = NameChangeAsync;
		}
		private static async Task ChangeOnlyPhtoAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Photo && unit.MesType != MessageType.Text) return;
			if (unit.MesType == MessageType.Text)
			{
				if (unit.MesText == strings.SkipChange)
				{
					await unit.Client.SendTextMessageAsync(unit.Id,
						strings.SkipChange,
						replyMarkup: GetCardMenuMarkup());
				}
			}
			else
			{
				var photo = unit.Message.Photo;
				if (photo == null) { return; }
				var photoPath = await unit.Client.GetFileAsync(photo[photo.Length - 1].FileId);

				CardMedia cardMedia = new CardMedia();
				cardMedia.Id = unit.Id;

				using (var memoryStream = new MemoryStream())
				{
					await unit.Client.DownloadFileAsync(photoPath.FilePath, memoryStream);
					memoryStream.Seek(0, SeekOrigin.Begin);
					cardMedia.Image = memoryStream.ToArray();
				}

				await BotService.ChatManager.SaveCardPhoto(cardMedia);
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.SuccessPhotoChange,
					replyMarkup: GetCardMenuMarkup());
				unit.NextHandler = HandleCardMenuReply;
			}
		}
		private static async Task NameChangeAsync(Unit unit)
		{
			await unit.Client.SendTextMessageAsync(unit.Id, $"Имя изменено - {unit.MesText}");
			unit.NewCard.Name = unit.MesText;

			// Next question transition
			await unit.Client.SendTextMessageAsync(unit.Id,
				strings.DescriptionQuestion,
				replyMarkup: GetDescriptionMarkup(unit.Card));
			unit.NextHandler = DescriptionChangeAsync;
		}
		private static async Task ChangeOnlyNameAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			await unit.Client.SendTextMessageAsync(unit.Id,
				$"Имя изменено - {unit.MesText}",
				replyMarkup: GetCardMenuMarkup());
			unit.NewCard.Name = unit.MesText;

			unit.NextHandler = HandleCardMenuReply;
		}
		private static async Task DescriptionChangeAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			if (unit.MesText == strings.SkipChange)
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.SkipReply);
			}
			else
			{
				await unit.Client.SendTextMessageAsync(unit.Id, $"Описание установлено - {unit.MesText}");
				unit.NewCard.Description = unit.MesText;
			}

			// Next question transition
			await unit.Client.SendTextMessageAsync(unit.Id,
				strings.SmokingQuestion,
				replyMarkup: GetConfirmMarkup());
			unit.NextHandler = ChangeSmokingAsync;
		}
		private static async Task ChangeOnlyDescription(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			if (unit.MesText == strings.SkipChange)
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.SkipReply,
					replyMarkup: GetCardMenuMarkup());
			}
			else
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					$"Описание установлено - {unit.MesText}",
					replyMarkup: GetCardMenuMarkup());
				unit.NewCard.Description = unit.MesText;
			}

			unit.NextHandler = HandleCardMenuReply;
		}
		private static async Task ChangeSmokingAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			if (unit.MesText == strings.ApproveShort) unit.NewCard.IsSmoking = true;
			else if (unit.MesText == strings.RejectShort) unit.NewCard.IsSmoking = false;
			else
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.InvalidAnswerError);
				return;
			}

			// Next question transition
			await unit.Client.SendTextMessageAsync(unit.Id,
				strings.DrinkQuestion);
			unit.NextHandler = ChangeDrinkingAsync;
		}
		private static async Task ChangeDrinkingAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			if (unit.MesText == strings.ApproveShort) unit.NewCard.IsDrinking = true;
			else if (unit.MesText == strings.RejectShort) unit.NewCard.IsDrinking = false;
			else
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.InvalidAnswerError);
				return;
			}

			// Next question transition
			await unit.Client.SendTextMessageAsync(unit.Id,
				strings.RegionQuestion,
				replyMarkup: GetRegionMarkup(unit.Card));
			unit.NextHandler = RegionChangeAsync;
		}
		private static async Task RegionChangeAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			var region = ResourceReader.FindRegionCodeByName(unit.MesText.ToLower());
			unit.NewCard.Region = region;

			// Next question transition
			await SendCardPreview(unit.Client, unit.Chat);
			unit.NextHandler = ConfirmEditedCardAsync;
		}
		private static async Task ConfirmEditedCardAsync(Unit unit)
		{
			if (unit.MesType != MessageType.Text) return;
			if(unit.MesText == strings.RejectShort)
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.NewCardRejectReact,
					replyMarkup: null);
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.AgeQuestion,
					replyMarkup: GetAgeMarkup(unit.Card));
				unit.NextHandler = AgeChangeAsync;
				return;
			}
			else if (unit.MesText == strings.ApproveShort)
			{
				await unit.Client.SendTextMessageAsync(unit.Id,
					strings.CardEditApproveReact);
				await OnCardEditCompleted(unit.Chat);
				await Menu.SendMenu(unit.Client, unit.Chat);
			}
		}
		private static async Task SendCardPreview(ITelegramBotClient client, Chat sender)
		{
			var media = await BotService.ChatManager.GetCardPhoto(sender.Id);
			var sb = new StringBuilder();
			sb.AppendLine(strings.ParamName + sender.NewCard.Name);
			sb.AppendLine(strings.ParamAge + sender.NewCard.Age);
			sb.AppendLine(strings.ParamGender + "".ToString(sender.NewCard.Gender));
			sb.AppendLine(strings.ParamTargetGender + "".ToString(sender.NewCard.TargetGender));
			sb.AppendLine(strings.ParamRegion + Utilities.ResourceReader.GetRegionName(sender.NewCard.Region));
			sb.AppendLine(strings.Separator);
			sb.AppendLine(strings.ParamDescription + sender.NewCard.Description);
			sb.AppendLine(strings.Separator);
			if (sender.NewCard.IsSmoking) sb.AppendLine(strings.ParamSmoking + strings.ApproveShort);
			else sb.AppendLine(strings.ParamSmoking + strings.RejectShort);
			if (sender.NewCard.IsDrinking) sb.AppendLine(strings.ParamDrinking + strings.ApproveShort);
			else sb.AppendLine(strings.ParamDrinking + strings.RejectShort);
			sb.AppendLine(strings.Separator);
			sb.AppendLine(strings.ConfirmNewCardQuestion);
			using (var memoryStream = new MemoryStream(media.Image)) {
				var file = new InputFileStream(memoryStream, $"{media.Id}.jpg");
				await client.SendPhotoAsync(sender.Id, 
					file,
					caption: sb.ToString(),
					replyMarkup: GetConfirmMarkup());
			}
		}
		public static async Task SendCardPreview(Unit unit)
		{
			unit.Chat.CopyCard();

			var media = await BotService.ChatManager.GetCardPhoto(unit.Id);
			var sb = new StringBuilder();
			sb.AppendLine(strings.ParamName + unit.Card.Name);
			sb.AppendLine(strings.ParamAge + unit.Card.Age);
			sb.AppendLine(strings.ParamGender + "".ToString(unit.Card.Gender));
			sb.AppendLine(strings.ParamTargetGender + "".ToString(unit.Card.TargetGender));
			sb.AppendLine(strings.ParamRegion + Utilities.ResourceReader.GetRegionName(unit.Card.Region));
			sb.AppendLine(strings.Separator);
			sb.AppendLine(strings.ParamDescription + unit.Card.Description);
			sb.AppendLine(strings.Separator);
			if (unit.Card.IsSmoking) sb.AppendLine(strings.ParamSmoking + strings.ApproveShort);
			else sb.AppendLine(strings.ParamSmoking + strings.RejectShort);
			if (unit.Card.IsDrinking) sb.AppendLine(strings.ParamDrinking + strings.ApproveShort);
			else sb.AppendLine(strings.ParamDrinking + strings.RejectShort);
			sb.AppendLine(strings.Separator);
			
			using (var memoryStream = new MemoryStream(media.Image))
			{
				var file = new InputFileStream(memoryStream, $"{media.Id}.jpg");
				await unit.Client.SendPhotoAsync(unit.Id,
					file,
					caption: sb.ToString(),
					replyMarkup: GetCardMenuMarkup());
			}

			unit.Chat.SetReply(HandleCardMenuReply);
		}
		// Keyboard Markups
		private static IReplyMarkup GetCardMenuMarkup()
		{
			return new ReplyKeyboardMarkup(
				new KeyboardButton[][]{
					new KeyboardButton[]
					{
						new KeyboardButton(strings.ChangeOnlyName),
						new KeyboardButton(strings.ChangeOnlyDescription),
						new KeyboardButton(strings.ChangeOnlyPhoto)
					},
					new KeyboardButton[]
					{
						new KeyboardButton(strings.MenuCommand),
						new KeyboardButton(strings.RemakeCardCommand),
					}
				})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = true
			};
		}
		private static IReplyMarkup GetAgeMarkup(Card card)
		{
			if (card.Age != -1)
				return new ReplyKeyboardMarkup(new KeyboardButton(card.Age.ToString()));
			else
				return new ReplyKeyboardRemove();
		}
		private static IReplyMarkup GetRegionMarkup(Card card)
		{
			if (card.Region == 0)
			{
				return new ReplyKeyboardMarkup(new KeyboardButton("Москва"));
			}
			return new ReplyKeyboardMarkup(new KeyboardButton(
				Utilities.ResourceReader.GetRegionName(card.Region)))
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = true
			};
		}
		private static IReplyMarkup GetDescriptionMarkup(Card card)
		{
			if (card.Description == null) return new ReplyKeyboardRemove();
			return new ReplyKeyboardMarkup(new KeyboardButton(strings.SkipChange))
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = true
			};
		}
		private static IReplyMarkup GetConfirmMarkup()
		{
			return new ReplyKeyboardMarkup(new KeyboardButton[]
			{
				new KeyboardButton(strings.ApproveShort),
				new KeyboardButton(strings.RejectShort)
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		private static IReplyMarkup GetGenderMarkup()
		{
			return new ReplyKeyboardMarkup(new KeyboardButton[]
			{
				new KeyboardButton(strings.GenderMaleConst),
				new KeyboardButton(strings.GenderFemaleConst),
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		private static ReplyKeyboardMarkup GetTargetGenderMarkup(Card card)
		{
			if (card.Gender == Gender.Male)
			{
				return new ReplyKeyboardMarkup(new KeyboardButton[]
				{
					new KeyboardButton(strings.TargetGenderFemale),
					new KeyboardButton(strings.TargetGenderMale),
					new KeyboardButton(strings.TargetGenderNotSpecified)
				})
				{ ResizeKeyboard = true, OneTimeKeyboard = true };
			}
			else
			{
				return new ReplyKeyboardMarkup(new KeyboardButton[]
				{
					new KeyboardButton(strings.TargetGenderMale),
					new KeyboardButton(strings.TargetGenderFemale),
					new KeyboardButton(strings.TargetGenderNotSpecified)
				})
				{ ResizeKeyboard = true, OneTimeKeyboard = true };
			}
		}
		private static IReplyMarkup GetPhotoMarkup(Card card)
		{
			if (!Utilities.ResourceReader.IsImageExists(card.Id)) return new ReplyKeyboardRemove();
			return new ReplyKeyboardMarkup(new KeyboardButton(strings.SkipChange))
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = true,
			};
		}
		private static IReplyMarkup GetNameMarkup(Card card)
		{
			if (card.Age == -1) return new ReplyKeyboardRemove();
			return new ReplyKeyboardMarkup(new KeyboardButton(card.Name))
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = true
			};
		}
		// Utils
		public static string GetHabbits(Card card, bool withPrefix = true)
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
			chat.CopyCard();
		}
		private static async Task OnCardEditCompleted(Chat chat)
		{
			chat.NewCard.IsActive = true;
			await ChatManager.ApplyCardChanges(chat);
		}
	}
}
