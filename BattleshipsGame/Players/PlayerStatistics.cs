using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.BattleshipsGame.Players
{
	class PlayerStatistics
	{
		public int TotalHits { get; set; } = 0;
		public int ShipsSunken { get; set; } = 0;
		public int TotalMisses { get; set; } = 0;
		public int GamesWon { get; set; } = 0;
		public int GamesLost { get; set; } = 0;
	}
}
