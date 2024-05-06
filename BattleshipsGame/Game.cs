using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Players;
using Battleships.BattleshipsGame.Battleships;
using Battleships.BattleshipsGame.Battlefields;
using Battleships.Global;
using Battleships.Inputs;
using Battleships.Content;

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
		public bool Ended { get => ChallengerBoard.OwnerBattlefield.AllShipsSunken || OpponentBoard.OwnerBattlefield.AllShipsSunken; }
		//Kdo vyhral
		public bool ChallengerWon { get => OpponentBoard.OwnerBattlefield.AllShipsSunken; }

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
		public static Game Create(IPlayer challenger = default, IPlayer opponent = default)
		{
			//Spusteni vyberu hrace
			if (challenger == default)
			{
				//Vybrani primarniho hrace
				challenger = Input.ObjectSelectionInput<Player>(
					TranslationKey.SelectPlayer,
					() => GlobalVariables.Players,
					0,
					null,
					Player.Create,
					TranslationKey.NewPlayer
				);
				//Pokud hrac neni nastaven, zrusit vytvareni
				if (challenger == default) return default;
			}
			//Vyber oponenta
			if (opponent == default)
			{
				//Zda uzivatel chce hrat proti pocitaci nebo hracovi
				int decision = Input.SelectionInput(
					TranslationKey.ChooseOpponentType,
					new List<string>()
					{
						ContentManager.GetTranslation(TranslationKey.Player),
						ContentManager.GetTranslation(TranslationKey.Computer)
					}
				);
				//Kontrola rozhodnuti
				if (decision == -1) return default;
				else if (decision == 0)
				{
					//Chce hrat proti hracovi
					//Vyber hrace
					opponent = Input.ObjectSelectionInput<Player>(
						TranslationKey.SelectPlayer,
						() => GlobalVariables.Players,
						0,
						(Player selection) => (selection != challenger, TranslationKey.PlayerAlreadyTaken),
						Player.Create,
						TranslationKey.NewPlayer
					);
					//Pokud hrac neni nastaven, zrusit vytvareni
					if (opponent == default) return default;
				}
				else
				{
					//Chce hrat proti pocitaci
					opponent = new AI();
				}
			}

			//Ziskani velikosti mapy
			byte? unverifiedBattlefieldSize = challenger.GetBattlefieldSize(6, 16);
			//Kontrola vysledku
			if (unverifiedBattlefieldSize == null) return default;
			//Prevod na jisty bajt
			byte battlefieldSize = (byte)unverifiedBattlefieldSize;
			
			//Ziskani moznych sad lodi
			IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> battleshipSets = EnemyBattlefield.GetAllBattleshipSets(battlefieldSize);
			//Vybrani sady lodi
			IReadOnlyDictionary<BattleshipSize, byte> battleshipSet = challenger.PickBattleshipSet(battleshipSets);
			//Kontrola vysledku
			if (battleshipSet == null) return default;
			//Vytvoreni bitevnich poli
			Battlefield challengerBattlefield = new(battlefieldSize, null, battleshipSet);
			Battlefield opponentBattlefield = new(battlefieldSize, null, battleshipSet);
			//Umisteni vsech lodi do bitevnich poli
			//if (!challenger.PlaceAllBattleships(challengerBattlefield)) return default;
			//if (!opponent.PlaceAllBattleships(opponentBattlefield)) return default;

			//Vytvoreni hracich desek
			Board challengerBoard = new(challenger, challengerBattlefield);
			Board opponentBoard = new(opponent, opponentBattlefield);

			//Vytvoreni samotne hry
			Game game = new(challengerBoard, opponentBoard);
			//Pridani hry do globalniho seznamu
			GlobalVariables.AddGame(game);
			return game;
		}
		//Pokracuje ve hre
		public bool? Progress()
		{
			//Kontrola stavu hry
			if (Ended) return false;

			//Ziskani utocnika
			Board currentBoard = (ChallengerOnMove ? ChallengerBoard : OpponentBoard);
			Board nextBoard = (ChallengerOnMove ? OpponentBoard : ChallengerBoard);
			//Pocet pokusu
			byte attemptsCount = 0;
			bool success = false;
			//Uzivatel ma 3 pokusy na uspesny utok, pokud bude z nejakeho duvodu zamitnut 3x za sebou, tah se posune na protihrace
			//Pri spravnem kodu nikdy nebude treba vice jak 1 pokus
			while (!success && attemptsCount < 3)
			{
				//Ziskani souradnice utoku
				Coordinate coordinate = currentBoard.Attack();
				//Pokud souradnice nebyla upresnena, uzivatel si preje akci zrusit
				if (coordinate == null) return null;

				//Udeleni grantu k utoku a nasledne provedeni samotneho utoku
				AttackingAllowed = true;
				success = nextBoard.GetAttacked(coordinate);
				AttackingAllowed = false;
				//Zapocitani pokusu
				attemptsCount++;
			}
			//Prohozeni roli
			ChallengerOnMove = (!ChallengerOnMove);
			//Vraceni vysledku
			return success;
		}
	}
}