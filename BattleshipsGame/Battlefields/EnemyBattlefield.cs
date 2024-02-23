using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.BattleshipsGame.Battlefields
{
	//Bitevni pole nepritele. Oproti klasickemu bitevnimu poli nema tento typ pole prehled o pozici lodi
	abstract class EnemyBattlefield
	{
		//Mapa souradnic
		public IEnumerable<IEnumerable<Coordinate>> CoordinateMap { get; }
		//Souradnice, na ktere se strilelo
		private protected Dictionary<Coordinate, AttackResult> _AttackedCoordinates { get; }
		public IReadOnlyDictionary<Coordinate, AttackResult> AttackedCoordinates { get => _AttackedCoordinates; }

		public EnemyBattlefield(int width, int? height = null)
		{
			
		}
	}
}