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
			List<(string option, Func<(bool available, TranslationKey reasonTranslationKey)> availability)> options = Options.Select(
				(option) => (option.Name, option.AvailabilityFunction ?? (() => (true, TranslationKey.Unknown)))
			).ToList();
			//Pocet vsech moznosti
			int realOptionsCount = options.Count;
			//Pridani moznosti pro vraceni se zpet
			options.Add(
				(
					HasParent
						? String.Format(ContentManager.GetTranslation(TranslationKey.Back), Parent.Name)
						: ContentManager.GetTranslation(TranslationKey.Exit),
					() => (true, TranslationKey.Unknown)
				)
			);
			//Index vybrane moznosti
			int index = 0;
			//Dokud neni konec tak zobrazovat menu
			do
			{
				//Ziskani volby
				index = Input.SelectionInput(
					NameTranslationKey ?? TranslationKey.Unknown,
					options.Select(
						(optionInfo) =>
						{
							(bool available, TranslationKey reasonTranslationKey) = optionInfo.availability.Invoke();
							return (optionInfo.option, available, reasonTranslationKey);
						}
					),
					index
				);
				if (index >= 0 && index < realOptionsCount)
				{
					//Zobrazeni potomka
					Options.ElementAt(index).Show();
				}
			}
			while (index >= 0 && index < realOptionsCount);
		}
	}
}
