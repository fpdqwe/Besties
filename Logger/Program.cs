using Bot;
using DAL;

namespace Logger
{
	public class Program
	{
		static void Main(string[] args)
		{
			MessageHandler.Log += Log;
			var bot = Bot.Program.Init();
			
			var m = new ContextManager();
			var c = m.CreateDatabaseContext();
			while (true)
			{
				var command = Console.ReadLine();
				switch (command) {
					case "/exit":
						Console.WriteLine("Shutdown command confirmed");
						return;
					case "/currentSessions":
						Console.WriteLine(command);
						MessageHandler.ChatManager.Chats
							.ForEach(chat => Console.WriteLine($"Session #{chat.Id}; user - {chat.Id}({chat.Card.Name})"));
						break;
				}
			}

		}

		public static void Log(string message)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"{message}");
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}
}
