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

		public EnemyBattlefield(int width, int? height = null, IReadOnlyDictionary<BattleshipSize, byte> battleshipSet = null)
		{
			
		}
		//Na zaklade velikosti mapy vygeneruje ruzne sady lodi
		//Sada je serazena od nejvetsi lodi po tu nejmensi
		public static IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> GetAllBattleshipSets(int width, int? height, bool generateFirstOnly = false)
		{
			return null;
		}
	}
}