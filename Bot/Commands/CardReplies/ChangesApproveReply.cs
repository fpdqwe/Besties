using Bot.Utilities;
using Domain.Entities;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.CardReplies
{
	internal class ChangesApproveReply : ReplyHandler
	{
        private const string MESSAGE = "Сохранить изменения?";
        private const string APPROVE = "Да";
        private const string REJECT = "Нет";
		private const string NAME_PARAM = "Имя: ";
		private const string AGE_PARAM = "Возраст: ";
		private const string REGION_PARAM = "Регион -";
		private const string GENDER_PARAM = "Пол: ";
		private const string TARGET_GENDER_PARAM = "Пол партнёра: ";
		private const string HABBITS_PARAM = "Вредные привычки: ";
		private const string DESCRIPTION_PARAM = "Описание: ";
		private const string SEPARATOR = "===*===*===";
        public ChangesApproveReply()
        {
            _awaitType = Telegram.Bot.Types.Enums.MessageType.Text;
            _messageText = MESSAGE;
        }

		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			switch (message.Text)
			{
				case APPROVE:
					_isFinished = true;
					break;
				case REJECT:
					_isFinished = false;
					break;
			}
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			var media = await BotService.ChatManager.GetCardPhoto(sender.Id);
			var sb = new StringBuilder();
			sb.AppendLine(NAME_PARAM + sender.NewCard.Name);
			sb.AppendLine(AGE_PARAM + sender.NewCard.Age);
			switch (sender.NewCard.Gender)
			{
				case Gender.Male:
					sb.AppendLine(GenderChangeReply.GENDER_MALE);
					break;
				case Gender.Female:
					sb.AppendLine(GenderChangeReply.GENDER_FEMALE);
					break;
				default:
					sb.AppendLine(GENDER_PARAM + GenderChangeReply.GENDER_NEUTRAL);
					break;
			}
			switch (sender.NewCard.TargetGender)
			{
				case Gender.Male:
					sb.AppendLine(TARGET_GENDER_PARAM + TargetGenderReply.GENDER_MALE);
					break;
				case Gender.Female:
					sb.AppendLine(TARGET_GENDER_PARAM + TargetGenderReply.GENDER_FEMALE);
					break;
				case Gender.NotSpecified:
					sb.AppendLine(TARGET_GENDER_PARAM + TargetGenderReply.GENDER_NEUTRAL);
					break;
			}
			sb.AppendLine(REGION_PARAM + Utilities.ResourceReader.GetRegionName(sender.NewCard.Region));
			sb.AppendLine(SEPARATOR);
			sb.AppendLine(DESCRIPTION_PARAM + sender.NewCard.Description);
			sb.AppendLine(SEPARATOR);
			sb.AppendLine(BadHabbitsReply.GetHabbits(sender, HABBITS_PARAM));
			sb.AppendLine(SEPARATOR);
			sb.AppendLine(MESSAGE);
			using (var memoryStream = new MemoryStream(media.Image))
			{
				var file = new InputFileStream(memoryStream, $"{media.Id}.jpg");
				await client.SendPhotoAsync(sender.Id, file, caption: sb.ToString(), replyMarkup: GetMarkup());

			}
		}
		
		private static ReplyKeyboardMarkup GetMarkup()
		{
			var result = new ReplyKeyboardMarkup(
				new KeyboardButton[]{
					new KeyboardButton(APPROVE),
					new KeyboardButton(REJECT)
			});
			result.OneTimeKeyboard = true;
			return result;
		}
	}
}
