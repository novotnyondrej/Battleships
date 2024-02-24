using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.BattleshipsGame.Battlefields
{
	//Vysledek utoku na souradnici
	enum AttackResult : byte
	{
		Miss = 0, //Lod se na souradnici nenachazi
		Hit = 1 //Lod byla zasahnuta
	}
}
