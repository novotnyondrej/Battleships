using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.Content;
using Battleships.Inputs;

namespace Battleships.Menus
{
	//Menu, ktere pod sebou nese dalsi menu sekce
	class ParentMenu : IMenu
	{
		//Pripadny preklad nazvu menu
		public TranslationKey? NameTranslationKey { get; }
		//Vyhodnoti, zda je tato sekce dostupna
		public Func<(bool available, TranslationKey reasonTranslationKey)>? AvailabilityFunction { get; }

		//Rodicovsky element
		public IMenu Parent { get; set; }
		//Moznosti kam dale
		public IEnumerable<IMenu> Options { get; }

		public ParentMenu(TranslationKey? nameTranslationKey, IEnumerable<IMenu> options, Func<(bool available, TranslationKey reaonTranslationKey)> availabilityFunction)
		{
			NameTranslationKey = nameTranslationKey;
			AvailabilityFunction = availabilityFunction;
			Options = options;
			//Nastaveni rodice pro potomky
			foreach (IMenu option in options) option.Parent = this;
		}
		//Zobrazi uzivateli menu
		public void Show()
		{
			//Nacteni moznosti jako texty
			List<string> options = Options.Select((option) => option.Name).ToList();
			//Pridani moznosti pro vraceni se zpet
			options.Insert(0, ContentManager.GetTranslation((Parent is not null) ? TranslationKey.Back : TranslationKey.Exit));
			//Ziskani volby
			int index = Input.SelectionInput<string>(TranslationKey.Unknown, options);
			//Kontrola volby
			if (index <= 0)
			{
				//Uzivatel se chce vratit o uroven vyse
				if (Parent is not null) Parent.Show();
				return;
			}
			//Zobrazeni potomka
			Options.ElementAt(index - 1).Show();
		}
	}
}
