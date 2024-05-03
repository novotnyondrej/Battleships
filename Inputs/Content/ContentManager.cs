using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Reflection;

namespace Battleships.Inputs.Content
{
	//Trida pro nacitani prekladu
	static class ContentManager
	{
		//Praujici prostredi
		private static string WorkingDirectory { get => AppDomain.CurrentDomain.BaseDirectory; }
		//Slozka se zdroji
		private static string ResourcesFolder = "Resources";
		private static string ResourcesDirectory { get => WorkingDirectory + ResourcesFolder + "\\";  }
		//Slozka s preklady
		private static string TranslationsFolder = "Translations";
		private static string TranslationsDirectory { get => ResourcesDirectory + TranslationsFolder + "\\"; }


		private static Dictionary<string, string> GetTranslations(string language = "en")
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "Battleships.Resources.Languages." + language + ".json";

			string content = "";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
			{
				content = reader.ReadToEnd();
			}
			Dictionary<string, string> translations = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
			return translations;
		}
		public static string GetTranslation(TranslationKey translationKey)
		{
			return GetTranslations()[translationKey.ToString()];
		}
	}
}