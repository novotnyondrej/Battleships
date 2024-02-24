using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Players;

namespace Battleships.BattleshipsGame.Battlefields
{
	//Hraci deska, ma prehled o tom, komu patri, jak vypada bitevni pole, a ma zakladni prehled o protihracove hraci desce
	class Board
	{
		//Hra, pod kterou tato deska spada
		public Game Parent { get; private set; }
		//Zda je tato hraci deska ve hre povazovana jako vyzyvatelova
		public bool IsChallengerBoard { get => Parent.ChallengerBoard == this; }
		//Zda je hrac na rade
		public bool OnMove { get => IsChallengerBoard == Parent.ChallengerOnMove; }

		//Hrac, kteremu tato deska patri
		public IPlayer Owner { get; }
		//Protihracova hraci deska
		private Board OpponentBoard { get; set; }

		//Bitevni pole
		//Tohoto hrace
		private Battlefield _OwnerBattlefield { get; }
		public EnemyBattlefield OwnerBattlefield { get => _OwnerBattlefield; }
		//Protihrace
		public EnemyBattlefield OpponentBattlefield { get => OpponentBoard.OwnerBattlefield; }
		
		//Vytvori novou hraci desku
		public Board(IPlayer owner, Battlefield ownerBattlefield)
		{
			Owner = owner;
			_OwnerBattlefield = ownerBattlefield;
			//Nastaveni vlastnika bitevniho pole
			ownerBattlefield.SetParent(this);
		}
		//Nastavi hru
		public bool SetParent(Game game)
		{
			return false;
		}
		//Odehraje
		public Coordinate Attack()
		{
			return null;
		}
		public bool GetAttacked(Coordinate coordinate)
		{
			return false;
		}
	}
}