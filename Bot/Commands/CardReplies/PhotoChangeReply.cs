using Bot.Utilities;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.CardReplies
{
	internal class PhotoChangeReply : ReplyHandler
	{
		private const string MESSAGE = "Теперь отправь своё фото";
		private const string SKIP = "Оставить текущее фото? Если хотите поменять, просто пришлите новое фото";
		private const string REACT = "Фото изменено";
		private const string APPROVE = "Оставить текущее";
		public override async Task Handle(ITelegramBotClient client, Chat sender, Message message)
		{
			switch (message.Type)
			{
				case MessageType.Text:
					if (message.Text == APPROVE)
					{
						_isFinished = true;
						return;
					}
					else return;
				case MessageType.Photo:
					await DownloadImage(client, sender, message.Photo);
					_isFinished = true;
					break;
			}
		}
		public override async Task SendMessage(ITelegramBotClient client, Chat sender)
		{
			if (Utilities.ResourceReader.IsImageExists(sender.Id))
			{
				await SendMessage(client, sender, SKIP, GetMarkup());
			}
			else
			{
				await base.SendMessage(client, sender);
			}
		}
		private static async Task DownloadImage(ITelegramBotClient client, Chat sender, PhotoSize[] photo)
		{
			if (photo == null) { return; }
			var photoPath = await client.GetFileAsync(photo[photo.Length - 1].FileId);

			CardMedia cardMedia = new CardMedia();
			cardMedia.Id = sender.Id;

			using (var memoryStream = new MemoryStream())
			{
				await client.DownloadFileAsync(photoPath.FilePath, memoryStream);
				memoryStream.Seek(0, SeekOrigin.Begin);
				cardMedia.Image = memoryStream.ToArray();
			}

			BotService.ChatManager.SaveCardPhoto(cardMedia);
			await client.SendTextMessageAsync(sender.Id, "Фото успешно изменено");
		}
		private static ReplyKeyboardMarkup GetMarkup()
		{
			return new ReplyKeyboardMarkup(new KeyboardButton(APPROVE))
			{
				ResizeKeyboard = true
			};
		}
	}
}
