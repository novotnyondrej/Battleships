using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.Data;
using Battleships.BattleshipsGame;
using Battleships.BattleshipsGame.Players;

namespace Battleships.Global
{
	//Trida pro globalni pristup k promennym
	static class GlobalVariables
	{
		//Soubory
		public static string PlayersFileName => "Players.json";
		public static string GamesFileName => "Games.json";
		public static string SettingsFileName => "Settings.json";


		//Ulozeni hraci a hry
		private static List<Player> _Players;
		private static List<Game> _Games;
		public static Settings Settings { get; private set; }
		public static IReadOnlyCollection<Player> Players => _Players.AsReadOnly();
		public static IReadOnlyCollection<Game> Games => _Games.AsReadOnly();

		static GlobalVariables()
		{
			_Players = (DataManager.DeserializeJson<IEnumerable<Player>>(FileManager.LoadSaveFile(PlayersFileName)) ?? Enumerable.Empty<Player>()).ToList();
			_Games = (DataManager.DeserializeJson<IEnumerable<Game>>(FileManager.LoadSaveFile(GamesFileName)) ?? Enumerable.Empty<Game>()).ToList();
			Settings = DataManager.DeserializeJson<Settings>(FileManager.LoadSaveFile(SettingsFileName)) ?? new();
		}
		public static void AddPlayer(Player player)
		{
			_Players.Add(player);
			SavePlayers();
		}
		public static void SavePlayers()
		{
			FileManager.SaveFile(PlayersFileName, DataManager.SerializeJon(_Players) ?? "");
		}
		public static void AddGame(Game game)
		{
			_Games.Add(game);
			SaveGames();
		}
		public static void SaveGames()
		{
			FileManager.SaveFile(GamesFileName, DataManager.SerializeJon(_Games) ?? "");
		}
		public static void SaveSettings()
		{
			FileManager.SaveFile(SettingsFileName, DataManager.SerializeJon(Settings) ?? "");
		}
	}
}
