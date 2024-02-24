using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battleships;

namespace Battleships.BattleshipsGame.Battlefields
{
	//Bitevni pole, ma prehled o tom, kde se jednotlive lode nachazeji a na jake pozice se strilelo
	class Battlefield : EnemyBattlefield
	{
		//Hraci deska, ktere toto bitevni pole patri
		public Board Parent { get; private set; }
		//Pozice vsech lodi
		private List<Battleship> _BattleshipsList { get; }
		public IEnumerable<Battleship> BattleshipsList { get => _BattleshipsList; }
		//Vrati velikost lodi, ktera stale chybi v bitevnim poli
		public BattleshipSize? NextMissingBattleship {
			get
			{
				//Pokus o nalezeni lode, ktera na bitevnim poli stale chybi
				KeyValuePair<BattleshipSize, byte> pair = BattleshipSet.FirstOrDefault(pair => _BattleshipsList.Count(battleship => pair.Key == battleship.Size) < pair.Value);
				//Pokud nenalezeno, vratit null (bitevni pole je kompletni)
				if (pair.Equals(default(KeyValuePair<BattleshipSize, byte>))) return null;
				//Vraceni velikosti chybejici lode
				return pair.Key;
			}
		}
		//Zda je bitevni pole plne nastaveno
		public bool Ready { get => !(NextMissingBattleship is null); }

		public Battlefield(byte width, byte? height = null, IReadOnlyDictionary<BattleshipSize, byte> battleshipSet = null) : base(width, height, battleshipSet)
		{
			_BattleshipsList = new();
		}
		//Pokus o umisteni lode na souradnici
		//Pri uspesnem umisteni bude vytvorena nova lod a bude pridana do seznamu lodi
		public bool PlaceNextBattleship(Coordinate coordinate, BattleshipOrientation orientation)
		{
			//Ziskani velikosti pokladane lode
			BattleshipSize? nullableSize = NextMissingBattleship;
			//Kontrola, ze vubec je co pokladat
			if (nullableSize is null) return false;

			BattleshipSize size = (BattleshipSize)nullableSize;

			//Ziskani vsech souradnic, na kterych by se budouci lod nachazela
			IEnumerable<Coordinate> futureTotalPosition = Battleship.GetTotalPosition(this, coordinate, size, orientation);
			//Nejprve je treba overit, ze lod nepresahuje hranici mapy
			if (futureTotalPosition.Count() < (byte)size) return false;

			//Kontrola dostupnosti jednotlivych poli
			bool occupied = _BattleshipsList.Any(
				ship => ship.TotalPosition.Any(
					occupiedPosition => futureTotalPosition.Any(
						futurePosition => occupiedPosition == futurePosition
					)
				)
			);
			if (occupied) return false;

			//Lod lze umistit do bitevniho pole
			_BattleshipsList.Add(new(this, coordinate, size, orientation));
			return true;
		}
		//Nastavi hraci desku, pod kterou toto bitevni podle spada
		public bool SetParent(Board parent)
		{
			Parent = parent;
			return true;
		}
		//Provede utok na souradnici
		public bool Attack(Coordinate coordinate)
		{
			return false;
		}
	}
}