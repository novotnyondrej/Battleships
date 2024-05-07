using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Players;
using Battleships.Inputs;
using Battleships.Content;

namespace Battleships.BattleshipsGame.Battlefields
{
	//Hraci deska, ma prehled o tom, komu patri, jak vypada bitevni pole, a ma zakladni prehled o protihracove hraci desce
	class Board
	{
		//Hra, pod kterou tato deska spada
		[JsonIgnore]
		public Game Parent { get; private set; }
		[JsonIgnore]
		public bool HasParent { get => !(Parent is null); }

		//Zda je tato hraci deska ve hre povazovana jako vyzyvatelova
		[JsonIgnore]
		public bool IsChallengerBoard { get => HasParent && Parent.ChallengerBoard == this; }
		//Zda je hrac na rade
		[JsonIgnore]
		public bool OnMove { get => HasParent && IsChallengerBoard == Parent.ChallengerOnMove; }
		//Zda lze provest utok na tuto desku
		[JsonIgnore]
		public bool IsVulnerable { get => HasParent && (!OnMove) && Parent.AttackingAllowed;  }

		//Hrac, kteremu tato deska patri
		public IPlayer Owner { get; }
		//Protihracova hraci deska
		private Board OpponentBoard { get => HasParent ? (IsChallengerBoard ? Parent.OpponentBoard : Parent.ChallengerBoard) : null; }
		private bool HasOpponent { get => !(OpponentBoard is null); }

		//Bitevni pole
		//Tohoto hrace
		public Battlefield _OwnerBattlefield { get; }
		[JsonIgnore]
		public EnemyBattlefield OwnerBattlefield { get => _OwnerBattlefield; }
		//Protihrace
		[JsonIgnore]
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
		public (bool success, AttackResult result, bool sunken) GetAttacked(Coordinate coordinate)
		{
			//Kontrola, ze na desku doopravdy jde zautocit
			if (!IsVulnerable) return (false, default, false);
			//Preneseni zpravy k bitevnimu poli
			return _OwnerBattlefield.GetAttacked(coordinate);
		}
		//Vypise bitevni pole obou hracu
		public void PrintBattlefields(bool opponentSecure = true, bool selfSecure = false)
		{
			//Vypsani bitevnich poli
			Console.Clear();
			InputManager.WriteLine(ContentManager.GetTranslation(TranslationKey.EnemyBattlefield));
			InputManager.WriteLine(OpponentBattlefield.ToString(secure: opponentSecure));
			InputManager.WriteLine("");
			InputManager.WriteLine(ContentManager.GetTranslation(TranslationKey.PlayerBattlefield));
			InputManager.WriteLine(_OwnerBattlefield.ToString(secure: selfSecure));
		}
	}
}