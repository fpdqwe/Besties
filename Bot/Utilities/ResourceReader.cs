using System.Diagnostics;
using System.Xml;
using Bot.Resources;

namespace Bot.Utilities
{
	public class ResourceReader
	{
		private static bool isInitialized = false;
		private static Dictionary<string, byte> _regions;
		public static bool IsInitialized { get { return isInitialized; } }

		public static byte GetRegion(string region)
		{
			if (IsInitialized == false) InitReader();

			byte result = 0;
			_regions.TryGetValue(region.ToLower(), out result);
			return result;
		}

		//public static string GetRegionName()
		//{
		//	if (IsInitialized == false) InitReader();

			
		//}
		public static void InitReader()
		{
			var st = new Stopwatch();
			MessageHandler.LogMessage("Bot.Utilities.ResourceReader initializing.");
			st.Start();
			_regions = GetRegions();
			isInitialized = true;
			st.Stop();
			MessageHandler.LogMessage($"Bot.Utilities.ResourceReader initializied in {st.ElapsedMilliseconds} ms.");
		}
		private static Dictionary<string, byte> GetRegions()
		{
			var dictionary = new Dictionary<string, byte>();

			var document = new XmlDocument();
			using (var stream = new FileStream(strings.regionsXmlPath, FileMode.Open, FileAccess.Read))
			{
				document.Load(stream);
			}
			

			foreach (XmlNode node in document.DocumentElement.ChildNodes)
			{
				string name = node.Attributes["Name"]?.Value;
				byte.TryParse(node.Attributes["Id"]?.Value, out byte value);

				if (!string.IsNullOrEmpty(name))
				{
					dictionary[name] = value;
				}
			}

			return dictionary;
		}
	}
}
