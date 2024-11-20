using Bot.Utilities;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.CardReplies
{
	internal class BadHabbitsReply : ReplyHandler
	{
        private const string MESSAGE = "Теперь пройдёмся по вредным привычкам ";
		private const string SMOKING_TAG = "Употребляете никотин ";
		private const string ALCO_TAG = "Употребляете алкоголь";
		private const string TAG_NEGATIVE = "- нет.";
		private const string TAG_POSITIVE = "- да.";
		private const string SMOKING_TAG_NEGATIVE = "Не курю";
		private const string SMOKING_TAG_POSITIVE = "Курю";
		private const string ALCO_TAG_POSITIVE = "Выпиваю";
		private const string ALCO_TAG_NEGATIVE = "Не пью";
		private const string APPROVE = "Всё верно";
		private const string SMOKING_NEGATIVE_REACT = "Сильно...";
		private const string SMOKING_POSITIVE_REACT = "Привычка конечно плохая, но не худшая";
		private const string ALCO_NEGATIVE_REACT = "Так и запишем";
		private const string ALCO_POSITIVE_REACT = "Осуждать не буду, времена тяжёлые...";
		private Message _question;
		private Message _reply;
        public BadHabbitsReply()
        {
            _awaitType = MessageType.Text;
            _messageText = MESSAGE;
			
        }
		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			if (_question.Text != Message) await client.DeleteMessageAsync(sender.Id, _question.MessageId);
			switch(message.Text)
			{
				case SMOKING_TAG_NEGATIVE:
					await SendMessage(client, sender, GetHabbits(sender, SMOKING_POSITIVE_REACT), GetMarkup(sender));
					break;
				case SMOKING_TAG_POSITIVE:
					sender.NewCard.IsSmoking = true;
					await SendMessage(client, sender, GetHabbits(sender, SMOKING_POSITIVE_REACT), GetMarkup(sender));
					break;
				case ALCO_TAG_NEGATIVE:
					sender.NewCard.IsDrinking = false;
					await SendMessage(client, sender, GetHabbits(sender, ALCO_NEGATIVE_REACT), GetMarkup(sender));
					break;
				case ALCO_TAG_POSITIVE:
					sender.NewCard.IsDrinking = true;
					await SendMessage(client, sender, GetHabbits(sender, ALCO_POSITIVE_REACT), GetMarkup(sender));
					break;
				case APPROVE:
					_isFinished = true;
					break;
			}
			await client.DeleteMessageAsync(sender.Id, message.MessageId);
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			_question = await client.SendTextMessageAsync(sender.Id,
				Message,
				replyMarkup: GetMarkup(sender));
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender, string message, IReplyMarkup markup = null)
		{
			var mes = await client.SendTextMessageAsync(
				sender.Id,
				message,
				replyMarkup: markup);
			BotService.LogMessage($"Handler replied: {mes.Text}. {mes.From.FirstName}");
			_question = mes;
		}
		public static string GetHabbits(Chat sender, string reaction)
		{
			var sb = new StringBuilder();
			sb.AppendLine(reaction + "\n");
			sb.Append(SMOKING_TAG);
			if (sender.NewCard.IsSmoking) sb.Append(TAG_POSITIVE);
			else sb.Append(TAG_NEGATIVE);
			sb.Append(" ");
			sb.Append(ALCO_TAG);
			if (sender.NewCard.IsSmoking) sb.Append(TAG_POSITIVE);
			else sb.Append(TAG_NEGATIVE);

			return sb.ToString();
		}
		private static ReplyKeyboardMarkup GetMarkup(Chat sender)
		{
			var btns = new KeyboardButton[3];
			if (sender.NewCard.IsSmoking) btns[0] = SMOKING_TAG_NEGATIVE;
			else btns[0] = SMOKING_TAG_POSITIVE;
			if (sender.NewCard.IsDrinking) btns[1] = ALCO_TAG_NEGATIVE;
			else btns[1] = ALCO_TAG_POSITIVE;
			btns[2] = APPROVE;
			
			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			return result;
		}
	}
}
