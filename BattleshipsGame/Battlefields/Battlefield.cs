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
		private List<Battleship> _AllBattleships { get; }
		public IEnumerable<Battleship> AllBattleships { get => _AllBattleships; }


		public Battlefield(int width, int? height = null) : base(width, height)
		{
			
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