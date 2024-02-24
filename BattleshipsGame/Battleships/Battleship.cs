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

		//Hlavni souradnice lodi
		public Coordinate Position { get; }
		//Velikost lodi v polickach
		public BattleshipSize Size { get; }
		//Orientace lodi
		public BattleshipOrientation Orientation { get; }
		//Vsechny souradnice, na kterych se lod nachazi
		public IEnumerable<Coordinate> TotalPosition { get; }

		//Vytvori novou lod
		public Battleship(Battlefield parent, Coordinate position, BattleshipSize size, BattleshipOrientation orientation)
		{
			Parent = parent;
			Position = position;
			Size = size;
			Orientation = orientation;
			TotalPosition = Battleship.GetTotalPosition(parent, position, size, orientation);
		}
		//Ziska vsechny souradnice, na kterych se lod nachazi
		public static IEnumerable<Coordinate> GetTotalPosition(Battlefield battlefield, Coordinate position, BattleshipSize size, BattleshipOrientation orientation)
		{
			//Nacteni souradnic
			byte x = position.X;
			byte y = position.Y;
			//Ziskani smeru
			byte xStep = (byte)(orientation == BattleshipOrientation.East ? 1 : (orientation == BattleshipOrientation.West ? -1 : 0));
			byte yStep = (byte)(orientation == BattleshipOrientation.South ? 1 : (orientation == BattleshipOrientation.North ? -1 : 0));

			List<Coordinate> totalPosition = new();
			for (byte cellNum = 0; cellNum < (byte)size; cellNum++)
			{
				//Vypocet novych souradnic
				byte newX = (byte)(x + xStep * cellNum);
				byte newY = (byte)(y + yStep * cellNum);
				//Nacteni souradnice
				Coordinate newPosition = battlefield.GetCoordinate(newX, newY);
				if (newPosition is null) break;

				//Pridani souradnice do seznamu
				totalPosition.Add(newPosition);
			}
			return totalPosition;
		}
	}
}