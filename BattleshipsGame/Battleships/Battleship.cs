using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battlefields;

namespace Battleships.BattleshipsGame.Battleships
{
	//Bitevni lod. Obsahuje informace o velikosti, pozici na mape a orientaci.
	class Battleship
	{
		//Bitevni pole, kteremu lod patri
		public EnemyBattlefield Parent { get; } //Mozna? Battlefield Parent { get; }

		//Velikost lodi v polickach
		public BattleshipSize Size { get; }
		//Orientace lodi
		public BattleshipOrientation Orientation { get; }
		//Hlavni souradnice lodi
		public Coordinate Position { get; }
		//Vsechny souradnice, na kterych se lod nachazi
		public IEnumerable<Coordinate> TotalPosition { get; }

		//Vytvori novou lod
		public Battleship(Battlefield parent, BattleshipSize size, BattleshipOrientation orientation, Coordinate position)
		{

		}
	}
}