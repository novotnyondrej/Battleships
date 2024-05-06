using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battlefields;
using Battleships.BattleshipsGame.Battleships;
using Battleships.Global;
using Battleships.Inputs;
using Battleships.Inputs.Controls;
using Battleships.Content;

namespace Battleships.BattleshipsGame.Players
{
	//Hrac
	class Player : IPlayer
	{
		//Jmeno hrace
		public string Name { get; }

		//Konstruktor pro noveho hrace
		private Player(string name)
		{
			Name = name;
		}
		//Vytvori noveho hrace
		public static Player Create()
		{
			//Vstup pro zadani jmena
			string name = Input.TextInput(
				TranslationKey.EnterPlayerName,
				3,
				24,
				(string name) =>
				{
					//Nesmi obsahovat mezery
					if (name.IndexOf(" ") >= 0 || name.IndexOf("\t") >= 0) return (false, ContentManager.GetTranslation(TranslationKey.NoSpacesAllowed));
					//Nesmi se jmenovat stejne
					if (GlobalVariables.Players.Any((player) => player.Name == name)) return (false, ContentManager.GetTranslation(TranslationKey.NameAlreadyTaken));
					return (true, default);
				}
			);
			//Vytvoreni hrace a pridani do globalniho seznamu hracu
			Player player = new(name);
			GlobalVariables.AddPlayer(player);
			//Vraceni slibeneho hrace
			return player;
		}


		//Ziska velikost bitevniho pole
		public byte? GetBattlefieldSize(byte minimumSize = 6, byte maximumSize = 16)
		{
			//Ziskani velikosti bitevniho pole
			int? size = Input.IntInput(TranslationKey.EnterBattlefieldSize, minimumSize, maximumSize);
			//Kontrola existence
			if (size == null) return null;
			return (byte)size;
		}
		//Dava hracovi moznost vybrat sadu lodi
		public IReadOnlyDictionary<BattleshipSize, byte> PickBattleshipSet(IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> sets)
		{
			//Prevod vsech setu na string
			IEnumerable<string> options = sets.Select(
				(set) => String.Join("\n", set.Select(
					(pair) => pair.Value + "x " + ContentManager.GetTranslation((TranslationKey)Enum.Parse(typeof(TranslationKey), pair.Key.ToString())) +  "(" + Battleship.ToString(pair.Key) + ")"
				))
			);
			//Vstup pro vybrani flotily
			int setIndex = Input.SelectionInput(
				TranslationKey.SelectBattleshipSet,
				options
			);
			//Kontrola vyberu
			if (setIndex < 0) return default;
			return sets.ElementAtOrDefault(setIndex);
		}
		//Prinuti uzivatele polozit lod
		public bool PlaceBattleship(Battlefield battlefield)
		{
			//Ziskani pozice dalsi lode
			(Coordinate coordinate, BattleshipOrientation orientation) = Input.PlaceBattleship(battlefield, battlefield.NextMissingBattleship ?? BattleshipSize.Battleship);
			//Polozeni lode
			if (coordinate != default) battlefield.PlaceNextBattleship(coordinate, orientation);
			return coordinate != default;
		}
		//Prinuti uzivatele polozit vsechny lode do bitevniho pole
		public bool PlaceAllBattleships(Battlefield battlefield)
		{
			if (battlefield.Ready) return true;
			//Informujici zprava, ze hrac poklada lode
			Console.Clear();
			InputManager.WriteLine(String.Format(ContentManager.GetTranslation(TranslationKey.StartPlacingBattleships), Name));
			//Potvrzeni
			InputManager.WriteLine(ContentManager.GetTranslation(TranslationKey.AnyKeyToContinue));
			InputManager.ReadKey(true, false);

			while (!battlefield.Ready)
			{
				if (!PlaceBattleship(battlefield)) return false;
			}
			//Zprava informujici o konci procesu
			Console.Clear();
			InputManager.WriteLine(String.Format(ContentManager.GetTranslation(TranslationKey.EndPlacingBattleships), Name));
			//Potvrzeni
			InputManager.WriteLine(ContentManager.GetTranslation(TranslationKey.AnyKeyToContinue));
			InputManager.ReadKey(true, false);

			return true;
		}
		//Ziska souradnici, na kterou chce hrac zautocit
		public Coordinate Attack(EnemyBattlefield enemyBattlefield, Battlefield ownerBattlefield)
		{
			Console.SetCursorPosition(0, 0);
			Coordinate coordinate = Input.GetCoordinate(TranslationKey.EnemyBattlefield, enemyBattlefield, false);
			return coordinate;
		}
		public override string ToString()
		{
			return Name;
		}
	}
}