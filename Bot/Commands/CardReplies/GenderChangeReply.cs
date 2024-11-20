using Bot.Resources;
using Bot.Utilities;
using Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.CardReplies
{
	internal class GenderChangeReply : ReplyHandler
	{
		public const string GENDER_MALE = "Я парень";
		public const string GENDER_FEMALE = "Я девушка";
		public const string GENDER_NEUTRAL = "Не указывать";
		private const string MESSAGE = "Теперь определимся с полом";
		public GenderChangeReply()
		{
			_awaitType = MessageType.Text;
			_messageText = MESSAGE;
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			await SendMessage(client, sender, Message, GenerateMarkup());
		}
		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			switch (message.Text)
			{
				case GENDER_MALE:
					sender.NewCard.Gender = Gender.Male;
					_isFinished = true;
					break;
				case GENDER_FEMALE:
					sender.NewCard.Gender = Gender.Female;
					_isFinished = true;
					break;
				default:
					_isFinished = false;
					break;
			}
		}
		private static ReplyKeyboardMarkup GenerateMarkup()
		{
			var btns = new KeyboardButton[]
			{
				GENDER_MALE,
				GENDER_FEMALE,
			};

			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}
	}
}
