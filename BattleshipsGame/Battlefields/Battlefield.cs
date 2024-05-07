using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battleships;

namespace Battleships.BattleshipsGame.Battlefields
{
	//Bitevni pole, ma prehled o tom, kde se jednotlive lode nachazeji a na jake pozice se strilelo
	class Battlefield : EnemyBattlefield
	{
		//Hraci deska, ktere toto bitevni pole patri
		[JsonIgnore]
		public Board Parent { get; private set; }
		//Pozice vsech lodi
		private List<Battleship> _BattleshipsList { get; }
		public IEnumerable<Battleship> BattleshipsList { get => _BattleshipsList; }
		//Vrati velikost lodi, ktera stale chybi v bitevnim poli
		[JsonIgnore]
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
		[JsonIgnore]
		public bool Ready { get => NextMissingBattleship is null; }

		public Battlefield(byte width, byte? height = null, IReadOnlyDictionary<BattleshipSize, byte> battleshipSet = null) : base(width, height, battleshipSet)
		{
			_BattleshipsList = new();
		}
		//Zda lod muze byt na danou souradnici polozena
		public bool CanPlaceBattleship(Coordinate coordinate, BattleshipSize size, BattleshipOrientation orientation)
		{
			//Ziskani vsech souradnic, na kterych by se budouci lod nachazela
			IEnumerable<Coordinate> futureTotalPosition = Battleship.GetTotalPosition(this, coordinate, size, orientation);
			//Nejprve je treba overit, ze lod nepresahuje hranici mapy
			if (futureTotalPosition.Count() < (byte)size) return false;
			//Kontrola dostupnosti jednotlivych poli
			bool occupied = _BattleshipsList.Any(
				ship => ship.TotalPosition.Any(
					occupiedPosition => futureTotalPosition.Any(
						futurePosition => occupiedPosition == futurePosition || occupiedPosition.IsNeighborOf(futurePosition)
					)
				)
			);
			return !occupied;
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

			if (!CanPlaceBattleship(coordinate, size, orientation)) return false;

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
		//Prijme utok od oponenta
		public (bool success, AttackResult result, bool sunken) GetAttacked(Coordinate coordinate)
		{
			//Kontrola, ze souradnice jiz nebyla uhodnuta
			if (!CanBeAttacked(coordinate)) return (false, default, false);
			//Ziskani lode, ktera se na teto souradnici nachazi
			Battleship battleship = _BattleshipsList.FirstOrDefault(
				battleship => battleship.TotalPosition.Contains(coordinate)
			);
			//Ziskani vysledku utoku
			AttackResult result = (battleship == default ? AttackResult.Miss : AttackResult.Hit);
			//Pridani vysledku do seznamu
			_AttackedCoordinates.Add(coordinate, result);

			bool sunken = false;
			//Pokud byla lod zasahnuta, je treba provest kontrolu, zda byla plne potopena
			if (result == AttackResult.Hit)
			{
				sunken = battleship.TotalPosition.All(
					coordinate => _AttackedCoordinates.ContainsKey(coordinate)
				);
				//Pokud lod byla potopena, je treba ji pridat do seznamu potopenych lodi a ziskat vsechny okolni policka a oznacit je jako trefu vedle
				if (sunken)
				{
					//Pridani lode do seznamu potopenych lodi
					_SunkenBattleships.Add(battleship);
					//Ziskani vsech okolnich policek
					IEnumerable<Coordinate> neighbors = CoordinateMap.Select(
						row => row.Where(
							coordinate => battleship.TotalPosition.Any(
								position => position.IsNeighborOf(coordinate)
							)
						)
					).SelectMany(item => item);
					//Oznaceni okolnich policek jako trefy vedle
					foreach (Coordinate neighbor in neighbors)
					{
						if (_AttackedCoordinates.ContainsKey(neighbor)) continue;
						//Oznaceni souradnice jako ozkousenou
						_AttackedCoordinates.Add(neighbor, AttackResult.Miss);
					}
				}
			}
			return (true, result, sunken);
		}
		//Ziska znak pro dane pole
		public override string GetColorAtCoordinate(Coordinate coordinate, bool secure = true)
		{
			//Ziskani barvy od pole, ktere nevidi nase lode
			string color = base.GetColorAtCoordinate(coordinate);
			//Pokud barva neni nastavena, zkontrolujeme, jestli nepatri jedne z nasich lodi
			if (color.Length <= 0 && !secure)
			{
				if (_BattleshipsList.Any((battleship) => battleship.TotalPosition.Contains(coordinate))) color = "BackgroundGray";
			}
			return color;
		}
	}
}