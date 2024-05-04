using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.BattleshipsGame.Battleships
{
	//Velikost lode
	enum BattleshipSize : byte
	{
		PatrolBoat = 2,
		Submarine = 3,
		//Cruiser = 3,
		Battleship = 4,
		Carrier = 5
	}
}