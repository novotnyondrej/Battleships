using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battleships;

namespace Battleships.BattleshipsGame.Battlefields
{
	//Bitevni pole nepritele. Oproti klasickemu bitevnimu poli nema tento typ pole prehled o pozici lodi
	abstract class EnemyBattlefield
	{
		//Velikost bitevniho pole
		public byte Width { get; }
		public byte Height { get; }

		//Mapa souradnic
		public IEnumerable<IEnumerable<Coordinate>> CoordinateMap { get; }
		//Sada lodi pouzita v bitevnim poli <velikost lodi, pocet>
		public IReadOnlyDictionary<BattleshipSize, byte> BattleshipSet { get; }
		//Souradnice, na ktere se strilelo
		private protected Dictionary<Coordinate, AttackResult> _AttackedCoordinates { get; }
		public IReadOnlyDictionary<Coordinate, AttackResult> AttackedCoordinates { get => _AttackedCoordinates; }
		//Seznam plne odhalenych lodi
		private protected List<Battleship> _SunkenBattleships;
		public IEnumerable<Battleship> SunkenBattleships { get => _SunkenBattleships; }

		//Vyhodnoti, zda jsou vsechny lode potopeny
		//Vsechny lode jsou potopeny, pokud se pocet potopenych lodi shoduje s poctem lodi v sade
		public bool AllShipsSunken
		{
			get => SunkenBattleships.GroupBy(
				ship => ship.Size
			).All(
				group => BattleshipSet[group.First().Size] <= group.Count()
			);
		}

		public EnemyBattlefield(byte width, byte? height = null, IReadOnlyDictionary<BattleshipSize, byte> battleshipSet = null)
		{
			byte trueHeight = height ?? width;
			//Nacteni atributu
			Width = width;
			Height = trueHeight;
			BattleshipSet = battleshipSet ?? EnemyBattlefield.GetAllBattleshipSets(width, trueHeight, true).First();

			//Vytvoreni mapy souradnic
			List<List<Coordinate>> coordinateMap = new();
			//Nacteni souradnic
			for (byte x = 0; x < width; x++)
			{
				List<Coordinate> row = new();

				for (byte y = 0; y < trueHeight; y++)
				{
					//Pridani souradnice
					row.Add(new(x, y));	
				}
				coordinateMap.Add(row);
			}
			CoordinateMap = coordinateMap;
			
			_AttackedCoordinates = new();
			_SunkenBattleships = new();
		}
		//Zda souradnice existuje
		public bool CoordinateExists(byte x, byte y)
		{
			return x <= Width && y <= Width;
		}
		public Coordinate GetCoordinate(byte x, byte y)
		{
			//Kontrola existence souradnice
			if (!CoordinateExists(x, y)) return null;
			//Vraceni souradnice
			return CoordinateMap.ElementAt(x).ElementAt(y);
		}
		//Na zaklade velikosti mapy vygeneruje ruzne sady lodi
		//Sada je serazena od nejvetsi lodi po tu nejmensi
		public static IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> GetAllBattleshipSets(byte width, byte? height = null, bool generateFirstOnly = false)
		{
			return new List<Dictionary<BattleshipSize, byte>>()
			{
				new()
				{
					{ BattleshipSize.Carrier, 1 },
					{ BattleshipSize.Battleship, 1 },
					{ BattleshipSize.Submarine, 1 },
					{ BattleshipSize.Cruiser, 1 },
					{ BattleshipSize.PatrolBoat, 1 }
				}
			};
		}
	}
}