using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battlefields;

namespace Battleships.BattleshipsGame.Players
{
	interface IPlayer
	{
		public Coordinate GetNextMove(Board board, Battlefield battlefield);
	}
}