using Bot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.CardReplies
{
	internal class RegionChangeReply : ReplyHandler
	{
        private const string MESSAGE = "Из какого ты города?";
        public RegionChangeReply()
        {
            _awaitType = MessageType.Text;
            _messageText = MESSAGE;
        }

		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			var region = ResourceReader.FindRegionCodeByName(message.Text);
			if(region == 0) _isFinished = true;
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			await SendMessage(client, sender, Message, GetMarkup(sender));
		}

		private static ReplyKeyboardMarkup GetMarkup(Chat sender)
		{
			var result = new ReplyKeyboardMarkup(new KeyboardButton(
				Utilities.ResourceReader.GetRegionName(sender.Card.Region)))
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = true
			};
			return result;
		}
	}
}
