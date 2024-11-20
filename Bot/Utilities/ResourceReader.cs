using System.Diagnostics;
using System.Xml;
using Bot.Resources;
using Domain.Entities;

namespace Bot.Utilities
{
	public class ResourceReader
	{
		private const string EMPTY_REGION = "Сказочный мир";
		private static bool isInitialized = false;
		private static List<Region> _regions = new List<Region>(16);
		public static bool IsInitialized { get { return isInitialized; } }

		public static byte FindRegionCodeByName(string regionName)
		{
			if (regionName == null) throw new ArgumentNullException("Region name");
			if (IsInitialized == false) InitReader();
			var result = _regions.FirstOrDefault(r => r.Name == regionName);
			if (result == null) return 0;
			return result.Index;
		}
		public static string GetRegionName(byte id)
		{
			if (id >= _regions.Count - 1) return EMPTY_REGION;
			return _regions[id].Name;
		}
		public static void InitReader()
		{
			var st = new Stopwatch();
			BotService.LogMessage("Bot.Utilities.ResourceReader initializing.");
			st.Start();
			InitRegions();
			isInitialized = true;
			st.Stop();
			BotService.LogMessage($"Bot.Utilities.ResourceReader initializied in {st.ElapsedMilliseconds} ms.");
		}

		public static async Task SaveImage(CardMedia media)
		{
			File.WriteAllBytesAsync(strings.imagesPath + $"{media.Id}.jpg", media.Image);
		}
		public static async Task<CardMedia> GetImage(long id)
		{
			CardMedia media = new CardMedia();
			media.Id = id;

			media.Image = await File.ReadAllBytesAsync(strings.imagesPath + $"{media.Id}.jpg");
			return media;
		}
		public static bool IsImageExists(long id)
		{
			return File.Exists(strings.imagesPath + $"{id}.jpg");
		}
		/// <summary>
		/// This method reads xml document with information about regions
		/// </summary>
		private static void InitRegions()
		{
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
					_regions.Add(new Region(value, name));
				}
			}
		}

		private class Region
		{
			public byte Index { get; private set; }
			public string Name { get; private set; }
            public Region(byte id, string name)
            {
                Index = id;
				Name = name;
            }
        }
	}
}
