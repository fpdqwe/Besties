using Bot.Resources;
using Bot.Utilities;
using Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.CardReplies
{
	internal class TargetGenderReply : ReplyHandler
	{
		private const string MESSAGE = "Кого ищешь?";
		public const string GENDER_MALE = "Парня";
		public const string GENDER_FEMALE = "Девушку";
		public const string GENDER_NEUTRAL = "Не принципиально";
		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			switch (message.Text)
			{
				case GENDER_MALE:
					sender.NewCard.TargetGender = Gender.Male;
					_isFinished = true;
					break;
				case GENDER_FEMALE:
					sender.NewCard.TargetGender = Gender.Female;
					_isFinished = true;
					break;
				case GENDER_NEUTRAL:
					sender.NewCard.TargetGender = Gender.NotSpecified;
					_isFinished = true;
					break;
				default:
					SendMessage(client, sender, strings.InvalidAnswerError, GenerateMarkup());
					break;
			}
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			await SendMessage(client, sender, MESSAGE, GenerateMarkup());
		}
		private static ReplyKeyboardMarkup GenerateMarkup()
		{
			var btns = new KeyboardButton[]
			{
				GENDER_MALE,
				GENDER_FEMALE,
				GENDER_NEUTRAL
			};

			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}
	}
}
