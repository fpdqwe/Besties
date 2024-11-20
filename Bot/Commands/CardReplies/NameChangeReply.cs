using Bot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.CardReplies
{
	internal class NameChangeReply : ReplyHandler
	{
		private const string MESSAGE = "Как тебя зовут?";
		private const string REACT = "Имя изменено, теперь - ";
        public NameChangeReply()
        {
			_awaitType = MessageType.Text;
			_messageText = MESSAGE;
        }

		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			sender.NewCard.Name = message.Text;
			_isFinished = true;
			await SendMessage(client, sender, REACT + message.Text);
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			await SendMessage(client, sender, Message, GetMarkup(sender.Card.Name));
		}

		private static ReplyKeyboardMarkup GetMarkup(string currentName)
		{
			if (string.IsNullOrEmpty(currentName)) { return null; }
			var result = new ReplyKeyboardMarkup(new KeyboardButton(currentName));
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}
	}
}
