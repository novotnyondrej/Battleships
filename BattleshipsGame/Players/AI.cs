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
		//Dava hracovi moznost vybrat sadu lodi
		public IReadOnlyDictionary<BattleshipSize, byte> PickBattleshipSet(IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> sets)
		{
			return default;
		}
		//Ziska souradnici, na kterou chce hrac umistit lod
		public (Coordinate position, BattleshipOrientation orientation) PlaceBattleship(Battlefield battlefield, BattleshipSize size)
		{
			return default;
		}
		//Ziska souradnici, na kterou chce hrac zautocit
		public Coordinate Attack(Board board, Battlefield battlefield)
		{
			return null;
		}
	}
}