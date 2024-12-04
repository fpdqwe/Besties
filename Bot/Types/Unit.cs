using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Types
{
	/// <summary>
	/// Класс не несёт никакой функциональности, нужен для того чтобы повысить читаемость кода
	/// Отражает условную единицу чата (1 апдейт и всё что нужно чтобы его обработать)
	/// </summary>
	public class Unit
	{
		public ITelegramBotClient Client { get; private set; }
		public Chat Chat { get; private set; }
		public Card Card { get => Chat.Card; set => Chat.Card = value; }
		public Card NewCard { get => Chat.NewCard; set => Chat.NewCard = value; }
		public long Id { get => Chat.Id; }
		public int IncomingOffersCount { get => Chat.SearchScopes.IncomingOffersCount; }
		public ChatUpdateHandler NextHandler
		{
			set => Chat.SetReply(value);
		}
		public Update Update { get; private set; }
		public UpdateType Type { get => Update.Type; }
		public Message Message { get => Update.Message; }
		public CallbackQuery Callback { get => Update.CallbackQuery; }
		public MessageType MesType
		{
			get
			{
				if (Message == null) throw new NullReferenceException("Message is null");
				else
				{
					return Message.Type;
				}
			}
		}
		public string MesText { get => Message.Text; }
		public bool IngoreReply { get; set; }
        public Unit(ITelegramBotClient client, Chat sender, Update update)
        {
            Client = client;
			Chat = sender;
			Update = update;
        }
    }
}
