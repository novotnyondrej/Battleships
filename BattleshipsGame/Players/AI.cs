using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battlefields;
using Battleships.BattleshipsGame.Battleships;

namespace Battleships.BattleshipsGame.Players
{
	//Umela inteligence
	class AI : IPlayer
	{
		//Ziska velikost bitevniho pole
		public byte? GetBattlefieldSize(byte minimumSize = 6, byte maximumSize = 16)
		{
			//Generace nahodneho cisla
			Random random = new();
			byte size = (byte)(random.Next(minimumSize, maximumSize));
			//Navraceni velikosti pole
			return size;
		}
		//Dava hracovi moznost vybrat sadu lodi
		public IReadOnlyDictionary<BattleshipSize, byte> PickBattleshipSet(IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> sets)
		{
			return sets.First();
		}
		//Ziska souradnici, na kterou chce hrac umistit lod
		public bool PlaceBattleship(Battlefield battlefield)
		{
			return false;
		}
		public bool PlaceAllBattleships(Battlefield battlefield)
		{
			
			return false;
		}
		//Ziska souradnici, na kterou chce hrac zautocit
		public Coordinate Attack(EnemyBattlefield enemyBattlefield, Battlefield ownerBattlefield)
		{
			return null;
		}
	}
}