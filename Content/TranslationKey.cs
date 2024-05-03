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

		TextTooShort = 20,
		TextTooLong = 21,

		OutOfRange = 30,
		NaN = 31,
		TooSmallNumber = 32,
		TooBigNumber = 33,
	}
}