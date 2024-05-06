using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battlefields;
using Battleships.BattleshipsGame.Battleships;

namespace Battleships.BattleshipsGame.Players
{
	//Rozhrani pro hrace
	interface IPlayer
	{
		//Jmeno hrace
		public string Name { get; }

		//Ziska velikost bitevniho pole
		public byte? GetBattlefieldSize(byte minimumSize = 6, byte maximumSize = 16);
		//Dava hracovi moznost vybrat sadu lodi
		public IReadOnlyDictionary<BattleshipSize, byte> PickBattleshipSet(IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> sets);
		//Ziska souradnici, na kterou chce hrac umistit lod
		public bool PlaceBattleship(Battlefield battlefield);
		public bool PlaceAllBattleships(Battlefield battlefield);
		//Ziska souradnici, na kterou chce hrac zautocit
		public Coordinate Attack(EnemyBattlefield enemyBattlefield, Battlefield ownerBattlefield);
	}
}