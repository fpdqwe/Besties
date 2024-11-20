using Bot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.CardReplies
{
	internal class DescriptionReply : ReplyHandler
	{
        private const string MESSAGE = "Теперь самое главное - описание твоей анкеты";
		private const string SKIP = "Оставить текущее описание";
		private const string REACT = "Новое описание установлено успешно";
        public DescriptionReply()
        {
            _awaitType = MessageType.Text;
            _messageText = MESSAGE;
        }
		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			if(message.Text == SKIP)
			{
				sender.NewCard.Description = sender.Card.Description;
				_isFinished = true;
				return;
			}
			sender.NewCard.Description = message.Text;
			_isFinished = true;
			 await SendMessage(client, sender, REACT);
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			await SendMessage(client, sender, Message, GetMarkup(sender));
		}
		private static ReplyKeyboardMarkup GetMarkup(Chat sender)
		{
			if (string.IsNullOrEmpty(sender.Card.Description)) return null;
			var result = new ReplyKeyboardMarkup(new KeyboardButton(SKIP));
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}
	}
}
