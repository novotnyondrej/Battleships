using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.Content;
using Battleships.Inputs;

namespace Battleships.Menus
{
	//Menu pro zobrazeni zpravy
	class MessageMenu : IMenu
	{
		//Pripadny preklad nazvu menu
		public TranslationKey? NameTranslationKey { get; }
		public string Name { get => ContentManager.GetTranslation(NameTranslationKey); }
		//Zprava co se ma zobrazit po rozkliknuti
		public TranslationKey MessageTranslationKey { get; }
		public string[] Replacemenets { get; }
		//Vyhodnoti, zda je tato sekce dostupna
		public Func<(bool available, TranslationKey reasonTranslationKey)> AvailabilityFunction { get; }

		//Rodicovsky element
		public IMenu Parent { get; set; }
		public bool HasParent { get => Parent is not null; }

		public MessageMenu(TranslationKey nameTranslationKey, TranslationKey messageTranslationKey, string[] replacements = null)
		{
			NameTranslationKey = nameTranslationKey;
			MessageTranslationKey = messageTranslationKey;
			Replacemenets = replacements ?? Array.Empty<string>();
		}

		//Zobrazi uzivateli menu
		public void Show()
		{
			//Vypsani zpravy
			Console.Clear();
			InputManager.WriteLine(String.Format(ContentManager.GetTranslation(MessageTranslationKey), Replacemenets));
			InputManager.WriteLine("\n" + ContentManager.GetTranslation(TranslationKey.AnyKeyToContinue));
			InputManager.ReadKey(true);
		}
	}
}