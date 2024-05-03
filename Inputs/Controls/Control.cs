using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.Inputs.Controls
{
	//Ovladaci prvky
	enum Control : byte
	{
		Unknown = 0,

		Confirm = 10,
		Back = 11,
		Delete = 12,
		TextInputCancel = 13,
		Cancel = 14,

		Up = 20,
		Right = 21,
		Down = 22,
		Left = 23,
		TextInputUp = 24,
		TextInputRight = 25,
		TextInputDown = 26,
		TextInputLeft = 27,

		RotateLeft = 31,
		RotateRight = 32,
	}
}
