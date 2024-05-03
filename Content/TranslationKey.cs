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

		Back = 10,
		Exit = 11,
		NextPage = 12,
		PreviousPage = 13,

		MainMenu = 200,
		NewGame = 201,
		LoadGame = 202,
		View = 203,
		Settings = 204,
		ViewPlayers = 205,
		ViewGames = 206,

		NewPlayer = 30,

		EnterPlayerName = 40,

		TextTooShort = 50,
		TextTooLong = 51,

		OutOfRange = 60,
		NaN = 61,
		TooSmallNumber = 62,
		TooBigNumber = 63,
	}
}