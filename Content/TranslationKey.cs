using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.Content
{
	//Klice pro preklady textu
	enum TranslationKey
	{
		Unknown = 0,
		Undefined = 1,
		AnyKeyToContinue = 2,

		Back = 10,
		Exit = 11,
		NextPage = 12,
		PreviousPage = 13,

		MainMenu = 20,
		NewGame = 21,
		LoadGame = 22,
		View = 23,
		ViewPlayers = 231,
		ViewGames = 232,
		Settings = 24,
		Help = 25,
		Controls = 250,
		MenuNavigation = 2500,
		Inputs = 2501,
		TextInputs = 25010,
		NumberInputs = 25011,
		SelectionInputs = 25012,

		MenuNavigationText = 30,
		TextInputsText = 31,
		NumberInputsText = 32,
		SelectionInputsText = 33,

		NewObject = 40,
		NewPlayer = 41,

		EnterPlayerName = 42,
		NoGames = 43,
		SelectPlayer = 44,
		ChooseOpponentType = 45,
		Player = 46,
		Computer = 47,
		PlayerAlreadyTaken = 48,
		NameAlreadyTaken = 49,
		NoSpacesAllowed = 491,
		EnterBattlefieldSize = 492,
		SelectBattleshipSet = 493,


		TextTooShort = 50,
		TextTooLong = 51,

		OutOfRange = 60,
		NaN = 61,
		TooSmallNumber = 62,
		TooBigNumber = 63,
	}
}