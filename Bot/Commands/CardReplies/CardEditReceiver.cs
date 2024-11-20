using Bot.Utilities;
using Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands.CardReplies
{
	internal class CardEditReceiver : BaseMessageReceiver
	{
		// PhotoChangeReply class can handle two types of messages (photo, text),
		// so we need to mark this handler index and override OnMessageReceiveMethod
		private readonly byte _exceptionIndex = 3;
		private readonly byte _endIndex = 8;
        public CardEditReceiver(ITelegramBotClient client, Chat sender) : base(client, sender)
		{
			sender.NewCard = new Card()
			{
				Id = sender.Id,
			};
			_handlers = new List<IReplyHandler>
			{
				new AgeChangeReply(),
				new GenderChangeReply(),
				new TargetGenderReply(),
				new PhotoChangeReply(),
				new NameChangeReply(),
				new DescriptionReply(),
				new RegionChangeReply(),
				new BadHabbitsReply(),
				new ChangesApproveReply()
			};
			ReplyHandler = _handlers[0];

			BotService.LogMessage($"Chat {sender.Id} has changed ({GetType()})");
		}
		public override async Task OnMessageReceived(ITelegramBotClient client, Chat sender, Message message)
		{
			if (_currentIndex == _exceptionIndex)
			{
				if (message.Type != MessageType.Text && message.Type != MessageType.Photo) return;
			}
			await ReplyHandler.Handle(client, sender, message);

			// If card not approved
			if ((_currentIndex - 1 == _endIndex) && !_currentHandler.IsFinished)
			{
				BotService.LogMessage($"{this.GetType()}: loop restarting...");
				RestartLoop();
				return;
			}

			if (_currentHandler.IsFinished)
			{
				await HandleDrop();
			}

			else BotService.LogMessage($"{this.GetType()}: The handler ({ReplyHandler.GetType()}) worked, but the message did not pass its conditions");
		}
		public override async Task HandleStart()
		{
			BotService.LogMessage($"{this.GetType()}: handler had start ({ReplyHandler.GetType()})");
			await ReplyHandler.SendMessage(_client, _sender);
		}
		public override async Task HandleDrop()
		{
			BotService.LogMessage($"{this.GetType()}: handler dropped ({ReplyHandler.GetType()})");
			ReplyHandler = _handlers[_currentIndex + 1];
			_currentIndex += 1;
			BotService.LogMessage($"{this.GetType()}: handler set ({ReplyHandler.GetType()})");
		}
		private void RestartLoop()
		{
			_currentIndex = 0;
			ReplyHandler = _handlers[_currentIndex];
		}
	}
}
