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
		public Func<(bool available, TranslationKey reasonTranslationKey)> AvailabilityFunction { get; }

		//Rodicovsky element
		public IMenu Parent { get; set; }
		public bool HasParent { get => Parent is not null; }

		//Moznosti kam dale
		public IEnumerable<IMenu> Options { get; }

		public ParentMenu(TranslationKey? nameTranslationKey, IEnumerable<IMenu> options, Func<(bool available, TranslationKey reaonTranslationKey)> availabilityFunction = default)
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
			//Nacteni dostupnosti moznosti
			List<(bool available, TranslationKey reasonTranslationKey)> availability = Options.Select((option) => option.Availability()).ToList();
			//Pridani moznosti pro vraceni se zpet
			if (HasParent)
			{
				options.Insert(0, ContentManager.GetTranslation(TranslationKey.Back));
				availability.Insert(0, Parent.Availability());
			}
			else
			{
				options.Add(ContentManager.GetTranslation(TranslationKey.Exit));
				availability.Add((true, TranslationKey.Unknown));
			}
			int realOptionsCount = Options.Count();
			int index = 0;
			//Dokud neni konec tak zobrazovat menu
			do
			{
				//Ziskani volby
				index = Input.SelectionInput<string>(TranslationKey.Unknown, options, index, availability);
				if ((!HasParent && index < realOptionsCount) || (HasParent && index > 0))
				{
					//Zobrazeni potomka
					Options.ElementAt(!HasParent ? index : index - 1).Show();
				}
			}
			while ((!HasParent && index < realOptionsCount) || (HasParent && index > 0));
		}
	}
}
