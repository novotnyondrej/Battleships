using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Players;
using Battleships.BattleshipsGame.Battlefields;

namespace Battleships.BattleshipsGame
{
	//Hlavni trida, skrze kterou se zaklada a hraje hra lode
	class Game
	{
		//Hraci plocha vyzyvatele a oponenta
		public Board ChallengerBoard { get; }
		public Board OpponentBoard { get; }

		//Pocatecni stavy
		//Kdo zacinal hru
		public bool ChallengerStarted { get; }
		//Stavy
		//Urcuje, kdo je prave na rade
		public bool ChallengerOnMove { get; private set; }
		//Zda hra aktualne povoluje utoky do ciziho pole
		public bool AttackingAllowed { get; private set; }
		//Zda je hra ukoncena
		public bool Ended { get; private set; }

		//Vytvori novou hru
		private Game(Board challengerBoard, Board opponentBoard, bool challengerStarts = true)
		{
			//Nastaveni hracich desek
			ChallengerBoard = challengerBoard;
			OpponentBoard = opponentBoard;
			//Nastaveni stavu
			ChallengerStarted = challengerStarts;
			ChallengerOnMove = challengerStarts;
			//Informovani hracich desek o prirazeni do hry
			//Hra nejdrive musi definovat hraci desky, pote az aktualizuje stav hracich desek
			//Hraci desky si ve vlastnim zajmu overi, ze pod tuto hru opravdu spadaji
			challengerBoard.SetParent(this);
			opponentBoard.SetParent(this);
		}
		//Pokusi se vytvorit novou hru (na prani uzivatele lze tento proces zrusit)
		public static Game Create()
		{
			return null;
		}

		//Pokracuje ve hre
		public bool Progress()
		{
			return false;
		}
	}
}