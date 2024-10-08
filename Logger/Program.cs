using Bot;
using DAL;

namespace Logger
{
	public class Program
	{
		static void Main(string[] args)
		{
			var bot = Bot.Program.Init();
			MessageHandler.Log += Log;
			var m = new ContextManager();
			var c = m.CreateDatabaseContext();
			while (true)
			{
				var command = Console.ReadLine();
				if (command == "/exit") { break; }
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
