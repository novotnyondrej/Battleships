using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.Content;
using Battleships.Inputs;
using Battleships.Menus.ObjectMenus;

namespace Battleships.Menus
{
	class PaginableMenu<OfType> : IMenu
	{
		//Pripadny preklad nazvu menu
		public TranslationKey? NameTranslationKey { get; }
		//Vyhodnoti, zda je tato sekce dostupna
		public Func<(bool available, TranslationKey reasonTranslationKey)> AvailabilityFunction { get; }

		//Rodicovsky element
		public IMenu Parent { get; set; }
		public bool HasParent { get => Parent is not null; }

		//Funkce pro ziskani moznosti
		public Func<IEnumerable<OfType>> OptionsFunction { get; }
		//Velikost stranky
		public int PageSize { get; }
		//Menu pro vybrany objekt
		public IObjectMenu<OfType> ObjectMenu { get; }

		public PaginableMenu(TranslationKey? nameTranslationKey, Func<IEnumerable<OfType>> optionsFunction, IObjectMenu<OfType> objectMenu, Func<(bool available, TranslationKey reaonTranslationKey)> availabilityFunction = default, int pageSize = 5)
		{
			NameTranslationKey = nameTranslationKey;
			AvailabilityFunction = availabilityFunction;
			OptionsFunction = optionsFunction;
			PageSize = pageSize;
			ObjectMenu = objectMenu;
		}
		//Zobrazi uzivateli menu
		public void Show()
		{
			/*Input.ObjectSelectionInput<OfType>(
				NameTranslationKey ?? TranslationKey.Unknown,
				OptionsFunction()
			);
			//Nacteni moznosti jako texty
			IEnumerable<OfType> objects = OptionsFunction();
			List<string> options = objects.Select((option) => option.ToString()).ToList();
			int realOptionsCount = options.Count();
			int pagesCount = (int)Math.Ceiling(realOptionsCount / (float)PageSize);

			int pageIndex = 0;
			int index = 1;
			bool end = false;
			//Dokud neni konec tak zobrazovat menu
			do
			{
				//Ziskani dalsi stranky
				List<string> pageOptions = (!options.Any() ? new() : options.GetRange(pageIndex * PageSize, pageIndex + 1 == pagesCount ? realOptionsCount - pageIndex * PageSize : PageSize));
				int realPageOptionsCount = pageOptions.Count;

				//Pridani moznosti pro novy objekt
				pageOptions.Insert(0, ObjectMenu.Name);
				//Pridani moznosti pro nasledujici a predchozi stranku
				if (pageIndex > 0) pageOptions.Insert(1, ContentManager.GetTranslation(TranslationKey.PreviousPage));
				if (pageIndex + 1 < pagesCount) pageOptions.Add(ContentManager.GetTranslation(TranslationKey.NextPage));
				//Pridani moznosti pro vraceni se zpet
				pageOptions.Add(
					HasParent
					? String.Format(ContentManager.GetTranslation(TranslationKey.Back), Parent.Name)
					: ContentManager.GetTranslation(TranslationKey.Exit)
				);
				int pageOptionsCount = pageOptions.Count;

				//Ziskani volby
				index = Input.SelectionInput(NameTranslationKey ?? TranslationKey.Unknown, pageOptions, index);
				if (index == 0)
				{
					//Vytvoreni noveho objktu
					ObjectMenu.Show();
				}
				else if ((pageIndex > 0 ? index > 1 : index > 0) && index - 1 < realPageOptionsCount)
				{
					//Zobrazeni menu pro objekt
					ObjectMenu.Show(objects.ElementAt(pageIndex * PageSize + index - 1));
				}
				else
				{
					//Akce
					if (pageIndex > 0 && index == 1)
					{
						pageIndex--;
						//Reset indexu
						index = (pageIndex > 0 ? 1 : 0) + PageSize + 1;
					}
					else if (pageIndex + 1 < pagesCount && index + 2 == pageOptionsCount)
					{
						pageIndex++;
						//Reset indexu
						index = 1;
					}
					else if (index < 0 || index + 1 == pageOptionsCount)
					{
						end = true;
					}
				}
			}
			while (!end);*/
		}
	}
}
