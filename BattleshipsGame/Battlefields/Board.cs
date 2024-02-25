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
		public bool HasParent { get => !(Parent is null); }

		//Zda je tato hraci deska ve hr!e povazovana jako vyzyvatelova
		public bool IsChallengerBoard { get => HasParent && Parent.ChallengerBoard == this; }
		//Zda je hrac na rade
		public bool OnMove { get => HasParent && IsChallengerBoard == Parent.ChallengerOnMove; }
		//Zda lze provest utok na tuto desku
		public bool IsVulnerable { get => HasParent && (!OnMove) && Parent.AttackingAllowed;  }

		//Hrac, kteremu tato deska patri
		public IPlayer Owner { get; }
		//Protihracova hraci deska
		private Board OpponentBoard { get => HasParent ? (IsChallengerBoard ? Parent.OpponentBoard : Parent.ChallengerBoard) : null; }
		private bool HasOpponent { get => !(OpponentBoard is null); }

		//Bitevni pole
		//Tohoto hrace
		private Battlefield _OwnerBattlefield { get; }
		public EnemyBattlefield OwnerBattlefield { get => _OwnerBattlefield; }
		//Protihrace
		public EnemyBattlefield OpponentBattlefield { get => HasOpponent ? OpponentBoard.OwnerBattlefield : null; }
		
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
			//Kontrola, ze hraci deska opravdu patri do teto hry
			if (game.ChallengerBoard != this && game.OpponentBoard != this) return false;
			
			Parent = game;
			return true;
		}
		//Odehraje
		public Coordinate Attack()
		{
			//Kontrola, ze opravdu jsme na rade
			if (!OnMove) return null;
			//Ziskani souradnice k utoku
			return Owner.Attack(OpponentBattlefield, _OwnerBattlefield);
		}
		//Prijme utok od oponenta
		public bool GetAttacked(Coordinate coordinate)
		{
			//Kontrola, ze na desku doopravdy jde zautocit
			if (!IsVulnerable) return false;
			//Preneseni zpravy k bitevnimu poli
			return _OwnerBattlefield.GetAttacked(coordinate);
		}
	}
}