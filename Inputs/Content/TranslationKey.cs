using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.Inputs.Content
{
	//Klice pro preklady textu
	enum TranslationKey
	{
		Unknown = 0,
		Undefined = 1,

		TextTooShort = 10,
		TextTooLong = 11,

		OutOfRange = 20,
		NaN = 21,
		TooSmallNumber = 22,
		TooBigNumber = 23

	}
}