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
	class ActionMenu : IMenu
	{
		//Pripadny preklad nazvu menu
		public TranslationKey? NameTranslationKey { get; }
		public string Name { get => ContentManager.GetTranslation(NameTranslationKey); }
		//Co se ma stat po rozkliknuti
		public Action OnSelect { get; }
		//Vyhodnoti, zda je tato sekce dostupna
		public Func<(bool available, TranslationKey reasonTranslationKey)> AvailabilityFunction { get; }

		//Rodicovsky element
		public IMenu Parent { get; set; }
		public bool HasParent { get => Parent is not null; }

		public ActionMenu(TranslationKey nameTranslationKey, Action onSelect)
		{
			NameTranslationKey = nameTranslationKey;
			OnSelect = onSelect;
		}

		//Provede danou akci
		public void Show()
		{
			//Spusteni akce
			if (OnSelect is not null) OnSelect.Invoke();
		}
	}
}