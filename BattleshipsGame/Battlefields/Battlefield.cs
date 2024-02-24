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
			
		}
		//Pokus o umisteni lode na souradnici
		//Pri uspesnem umisteni bude vytvorena nova lod a bude pridana do seznamu lodi
		public bool PlaceNextBattleship(Coordinate coordinate, BattleshipOrientation orientation)
		{
			return false;
		}
		//Nastavi hraci desku, pod kterou toto bitevni podle spada
		public bool SetParent(Board parent)
		{
			return false;
		}
		//Provede utok na souradnici
		public bool Attack(Coordinate coordinate)
		{
			return false;
		}
	}
}