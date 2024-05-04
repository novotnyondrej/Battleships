using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame;
using Battleships.Content;
using Battleships.Inputs;

namespace Battleships.Menus.ObjectMenus
{
	class GameMenu : IObjectMenu<Game>
	{
		//Pripadny preklad nazvu menu
		public TranslationKey? NameTranslationKey { get; }
		public string Name { get => ContentManager.GetTranslation(NameTranslationKey); }
		//Vyhodnoti, zda je tato sekce dostupna
		public Func<(bool available, TranslationKey reasonTranslationKey)> AvailabilityFunction { get; }

		public GameMenu()
		{
			NameTranslationKey = TranslationKey.NewGame;
		}
		//Rodicovsky element
		public IMenu Parent { get; set; }
		public bool HasParent { get => Parent is not null; }
		//Vytvoreni noveho uzivatele
		public void Show()
		{
			Input.TextInput(TranslationKey.EnterPlayerName, 5, 20);
		}
		//Zobrazi uzivateli menu pro hrace
		public void Show(Game game)
		{
			if (game == default)
			{
				Show();
				return;
			}
			Console.Clear();
			Console.WriteLine("game");
			Console.ReadKey();
			return;
		}
	}
}
