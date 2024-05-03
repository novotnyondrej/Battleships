using System;
using System.Collections.Generic;
using Battleships.BattleshipsGame;
using Battleships.BattleshipsGame.Players;
using Battleships.Inputs;
using Battleships.Inputs.Controls;
using Battleships.Content;
using Battleships.Data;
using Battleships.Menus;
using System.Linq;

//Straveny cas: 19h
//https://github.com/novotnyondrej/Battleships
namespace Battleships
{
	class Program
	{
		static void Main()
		{
			/*Game game = Game.Create(new Player());
			while (!game.Ended)
			{
				if (game.Progress() == null) break;
			}*/
			//Input.IntInput(TranslationKey.Test, 2, 222);
			//Console.WriteLine("\n" + Input.SelectionInput<int>(TranslationKey.Undefined, new List<int>() { 4, 5, 6 }));
			//Console.WriteLine(ContentManager.GetTranslation(TranslationKey.NaN));
			//Input.TextInput(TranslationKey.Undefined, 222, 22);
			//Console.WriteLine(DataManager.DeserializeJson<string>("\"test\""));
			ParentMenu menu = new(
				TranslationKey.Unknown,
				new List<IMenu>()
				{
					new ParentMenu(null, Enumerable.Empty<IMenu>()),
					new ParentMenu(TranslationKey.Unknown, Enumerable.Empty<IMenu>(), () => (false, TranslationKey.NaN)),
					new ParentMenu(TranslationKey.Unknown, new List<IMenu>()
					{
						new ParentMenu(TranslationKey.Unknown, Enumerable.Empty<IMenu>())
					})
				}
			);
			menu.Show();
		}
	}
}