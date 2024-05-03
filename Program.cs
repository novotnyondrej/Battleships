using System;
using System.Collections.Generic;
using Battleships.BattleshipsGame;
using Battleships.BattleshipsGame.Players;
using Battleships.Inputs;
using Battleships.Inputs.Controls;
using Battleships.Content;
using Battleships.Data;
using Battleships.Menus;
using Battleships.Menus.ObjectMenus;
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
				TranslationKey.MainMenu,
				new List<IMenu>()
				{
					new ParentMenu(TranslationKey.NewGame, Enumerable.Empty<IMenu>()),
					new ParentMenu(TranslationKey.LoadGame, Enumerable.Empty<IMenu>()),
					new ParentMenu(TranslationKey.View, new List<IMenu>()
					{
						new PaginableMenu<IPlayer>(TranslationKey.ViewPlayers, () => new List<IPlayer>() { new Player() }, new PlayerMenu()),
						new ParentMenu(TranslationKey.ViewGames, Enumerable.Empty<IMenu>())
					}),
					new ParentMenu(TranslationKey.Settings, Enumerable.Empty<IMenu>())
				}
			);
			menu.Show();
			Console.WriteLine(FileManager.SaveLocation);
		}
	}
}