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

		//Ulozeni hraci a hry
		private static List<Player> _Players;
		private static List<Game> _Games;
		public static IReadOnlyCollection<Player> Players => _Players.AsReadOnly();
		public static IReadOnlyCollection<Game> Games => _Games.AsReadOnly();

		static GlobalVariables()
		{
			_Players = (DataManager.DeserializeJson<IEnumerable<Player>>(FileManager.LoadSaveFile(PlayersFileName)) ?? Enumerable.Empty<Player>()).ToList();
			_Games = (DataManager.DeserializeJson<IEnumerable<Game>>(FileManager.LoadSaveFile(GamesFileName)) ?? Enumerable.Empty<Game>()).ToList();
		}
		public static void AddPlayer(Player player)
		{
			_Players.Add(player);
		}
		public static void AddGame(Game game)
		{
			_Games.Add(game);
		}
	}
}
