using Bot.Resources;
using Bot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.CardReplies
{
	internal class AgeChangeReply : ReplyHandler
	{
		private const string MESSAGE = "Сколько тебе лет?";
		public AgeChangeReply()
		{
			_awaitType = MessageType.Text;
			_messageText = MESSAGE;
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			if (sender.Card.Age != -1) 
				await SendMessage(client, sender, Message, GetMarkup(sender.Card.Age));
			else
			{
				await base.SendMessage(client, sender);
			}
		}
		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			int age = sender.Card.Age;
			if (int.TryParse(message.Text, out age))
			{
				if (age < 18 || age > 140)
				{
					SendMessage(client, sender, strings.InvalidAgeError);
					return;
				}
				else
				{
					client.SendTextMessageAsync(sender.Id, $"Ваш возраст теперь {age} вместо {sender.Card.Age}");
					sender.NewCard.Age = age;
					_isFinished = true;
					return;
				}
			}
			SendMessage(client, sender, strings.CantParseAgeError);
		}
		private static ReplyKeyboardMarkup GetMarkup(int currentAge)
		{
			var btns = new KeyboardButton[]
			{
				currentAge.ToString()
			};

			var result = new ReplyKeyboardMarkup(btns);
			result.ResizeKeyboard = true;
			result.OneTimeKeyboard = true;
			return result;
		}
	}
}
