using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Reflection;
using Battleships.Data;

namespace Battleships.Content
{
	//Trida pro nacitani prekladu
	static class ContentManager
	{
		//Nactene preklady
		private static Dictionary<string, Dictionary<TranslationKey, string>> Translations;
		//Seznam podporovanych jazyku
		public static IEnumerable<string> Languages { get => Translations.Keys; }
		
		static ContentManager()
		{
			//Nacteni prekladu
			Translations = FileManager.GetResources("Battleships.Resources.Languages").Select(
				(translations) => (
					translations.Key,
					DataManager.DeserializeJson<Dictionary<string, string>>(translations.Value).Where(
						(translation) => Enum.TryParse<TranslationKey>(translation.Key, out _)
					).Select(
						(translation) => (Enum.Parse<TranslationKey>(translation.Key), translation.Value)
					).ToDictionary(
						(translation) => translation.Item1,
						(translation) => translation.Value
					)
				)
			).Where(
				(translations) => translations.Item2 != default
			).ToDictionary(
				(translations) => translations.Key,
				(translations) => translations.Item2
			);
		}
		//Ziska preklady v danem jazyce
		private static Dictionary<TranslationKey, string> GetTranslations(string language = "en")
		{
			//Kontrola existence jazyka
			if (!Translations.ContainsKey(language)) return default;
			return Translations[language];
		}
		private static (bool keyExists, string translation) TryGetTranslation(TranslationKey? translationKey, string targetLanguage = "en")
		{
			//Pokus o ziskani jazyka
			Dictionary<TranslationKey, string> translations = GetTranslations(targetLanguage);
			//Kontrola existence jazyka
			if (translations == default) return (false, default);
			//Kontrola exitence prekladu
			if (translationKey is null || !translations.ContainsKey((TranslationKey)translationKey))
			{
				//Klic neexistuje
				//Pokus o nalezeni klice pro neznamy preklad
				if (!translations.ContainsKey(TranslationKey.Undefined)) return (false, default);
				//Vraceni neznameho prekladu
				return (false, String.Format(translations[TranslationKey.Undefined], (translationKey is null ? "null" : translationKey.ToString())));
			}
			return (true, translations[(TranslationKey)translationKey]);
		}
		//Ziska preklad daneho klice
		public static string GetTranslation(TranslationKey? translationKey, string targetLanguage = "en")
		{
			//Pokus o ziskani prekladu v cilovem jazyce
			(bool keyExists, string translation) = TryGetTranslation(translationKey, targetLanguage);
			//Pokud preklad existuje tak vratit
			if (keyExists) return translation;

			string fallback = translation;
			//Pokus o nalezeni prekladu v anglictine
			if (targetLanguage != "en")
			{
				(keyExists, translation) = TryGetTranslation(translationKey, "en");
				//Pokud preklad existuje tak vratit
				if (keyExists) return translation;
				//Pokud klic neexistuje a zaroven neexistuje preklad pro neznamy klic, vratime anglicky preklad
				if (fallback == default) return translation;
			}
			return fallback;
		}
	}
}