using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.Content;

namespace Battleships.Menus
{
	//Menu
	interface IMenu
	{
		//Pripadny preklad nazvu menu
		public TranslationKey? NameTranslationKey { get; }
		public string Name { get => ContentManager.GetTranslation(NameTranslationKey); }
		//Vyhodnoti, zda je tato sekce dostupna
		public Func<(bool available, TranslationKey reasonTranslationKey)>? AvailabilityFunction { get; }

		//Rodicovsky element
		public IMenu Parent { get; set; }
		//Zobrazi uzivateli menu
		public void Show();
		//Zda je dostupna tato sekce
		public bool Available() => AvailabilityFunction is null || AvailabilityFunction().available;
	}
}