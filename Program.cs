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
using Battleships.Global;

//Straveny cas: 27h
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
					new ActionMenu(TranslationKey.NewGame, () =>
					{
						//Zalozeni hry
						Game game = Game.Create();
						//Kontrola upesnosti
						if (game == default) return;
						//Spusteni hry
						while (game.Progress() == true) { };
					}),
					new ActionMenu(TranslationKey.LoadGame, () =>
					{
						//Vybrani hry
						int gameIndex = Input.SelectionInput(TranslationKey.LoadGame, GlobalVariables.Games.Select(
							(game) => (game.ToString(), !game.Ended, TranslationKey.GameEndedAlready)
						));
						//Kontrola vyberu
						if (gameIndex < 0) return;
						//Pokracovani ve hre
						while (GlobalVariables.Games.ElementAt(gameIndex).Progress() == true) { };

					}, () => (GlobalVariables.Games.Any((game) => !game.Ended), TranslationKey.NoGames)),
					new ParentMenu(TranslationKey.View, new List<IMenu>()
					{
						new ActionMenu(TranslationKey.ViewPlayers, () =>
						{
							//Vybrani hrace
							int playerIndex = Input.SelectionInput(TranslationKey.ViewPlayers, GlobalVariables.Players.Select(
								(player) => player.ToString()
							));
							//Kontrola vyberu
							if (playerIndex < 0) return;
							//Zobrazeni detailu hrace
							GlobalVariables.Players.ElementAt(playerIndex).ShowDetails();

						}, () => (GlobalVariables.Players.Any(), TranslationKey.NoPlayers)),
						//new PaginableMenu<Game>(TranslationKey.ViewGames, () => GlobalVariables.Games, new GameMenu())
					}),
					new ParentMenu(TranslationKey.Settings, new List<IMenu>()
					{
						new ActionMenu(TranslationKey.Language, () =>
						{
							//Ziskani noveho vyberu jazyka
							int languageIndex = Input.SelectionInput(
								TranslationKey.SelectLanguage,
								ContentManager.Languages.Select((language) => ContentManager.GetTranslation(TranslationKey.self, language) + "(" + language + ")"),
								ContentManager.Languages.ToList().IndexOf(GlobalVariables.Settings.Language)
							);
							//Kontrola vyberu
							if (languageIndex < 0) return;
							//Prenastaveni jazyka
							GlobalVariables.Settings.Language = ContentManager.Languages.ElementAt(languageIndex);
							GlobalVariables.SaveSettings();
						})
					}),
					new ParentMenu(TranslationKey.Help, new List<IMenu>()
					{
						new ParentMenu(TranslationKey.Controls, new List<IMenu>()
						{
							new MessageMenu(TranslationKey.MenuNavigation, TranslationKey.MenuNavigationText, new string[]{
								ControlManager.GetControlAsString(Control.Up),
								ControlManager.GetControlAsString(Control.Down),
								InputManager.SelectionColor.ToString(),
								ControlManager.GetControlAsString(Control.Confirm),
								String.Format(ContentManager.GetTranslation(TranslationKey.Back), "[Menu]"),
								ContentManager.GetTranslation(TranslationKey.Exit),
								ControlManager.GetControlAsString(Control.Cancel)
							}),
							new ParentMenu(TranslationKey.Inputs, new List<IMenu>()
							{
								new MessageMenu(TranslationKey.TextInputs, TranslationKey.TextInputsText, new string[]
								{
									ControlManager.GetControlAsString(Control.TextInputLeft),
									ControlManager.GetControlAsString(Control.TextInputRight),
									ControlManager.GetControlAsString(Control.TextInputUp),
									ControlManager.GetControlAsString(Control.TextInputDown),
									ControlManager.GetControlAsString(Control.Back),
									ControlManager.GetControlAsString(Control.Delete),
									ControlManager.GetControlAsString(Control.Confirm),
									ControlManager.GetControlAsString(Control.TextInputCancel)
								}),
								new MessageMenu(TranslationKey.NumberInputs, TranslationKey.NumberInputsText, new string[]
								{
									ControlManager.GetControlAsString(Control.TextInputLeft),
									ControlManager.GetControlAsString(Control.TextInputRight),
									ControlManager.GetControlAsString(Control.TextInputUp),
									ControlManager.GetControlAsString(Control.TextInputDown),
									ControlManager.GetControlAsString(Control.Back),
									ControlManager.GetControlAsString(Control.Delete),
									ControlManager.GetControlAsString(Control.Confirm),
									ControlManager.GetControlAsString(Control.TextInputCancel)
								}),
								new MessageMenu(TranslationKey.SelectionInputs, TranslationKey.SelectionInputsText, new string[]
								{
									ControlManager.GetControlAsString(Control.Up),
									ControlManager.GetControlAsString(Control.Down),
									ControlManager.GetControlAsString(Control.Right),
									ContentManager.GetTranslation(TranslationKey.NextPage),
									ControlManager.GetControlAsString(Control.Left),
									ContentManager.GetTranslation(TranslationKey.PreviousPage),
									InputManager.SelectionColor.ToString(),
									ControlManager.GetControlAsString(Control.Confirm),
									ControlManager.GetControlAsString(Control.Cancel)
								})
							})
						})
					})
				}
			);
			menu.Show();
			//Console.WriteLine(FileManager.SaveLocation);
		}
	}
}