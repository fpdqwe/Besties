using Bot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands.Card
{
	internal class AgeChangeReply : ReplyHandler
	{
		public AgeChangeReply(string Message) : base(Message)
		{

		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			await client.SendTextMessageAsync(sender.Id, Message);
		}

		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			int age = sender.Card.Age;
			if (int.TryParse(message.Text, out age))
			{
				if (age < 18)
				{
					await client.SendTextMessageAsync(sender.Id, "Недопустимый возраст");
					return;
				}
				else
				{
					await client.SendTextMessageAsync(sender.Id, $"Ваш возраст теперь {age} вместо {sender.Card.Age}");
					await client.SendTextMessageAsync(sender.Id, "Теперь определимся с полом", replyMarkup: GetGenderMarkup());
					sender.NewCard.Age = age;
					sender.SetReply(client);
					return;
				}
			}
			await client.SendTextMessageAsync(sender.Id, "Не удалость перевести возраст в число :(");
		}
	}
}
