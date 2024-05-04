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

		//Ulozeni hraci
		public static IEnumerable<Player> Players;
		public static IEnumerable<Game> Games;

		static GlobalVariables()
		{
			Players = DataManager.DeserializeJson<IEnumerable<Player>>(FileManager.LoadSaveFile(PlayersFileName)) ?? Enumerable.Empty<Player>();
			Games = DataManager.DeserializeJson<IEnumerable<Game>>(FileManager.LoadSaveFile(GamesFileName)) ?? Enumerable.Empty<Game>();
		}
		
	}
}
