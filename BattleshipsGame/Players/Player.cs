using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battlefields;
using Battleships.BattleshipsGame.Battleships;

namespace Battleships.BattleshipsGame.Players
{
	//Hrac
	class Player : IPlayer
	{
		//Ziska velikost bitevniho pole
		public byte? GetBattlefieldSize(byte minimumSize = 6, byte maximumSize = 16)
		{
			Console.Clear();
			Console.WriteLine("Battlefield size");
			byte size = byte.Parse(Console.ReadLine());

			if (size < minimumSize) size = minimumSize;
			else if (size > maximumSize) size = maximumSize;
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
		public Coordinate Attack(Board board, Battlefield battlefield)
		{
			return null;
		}
	}
}