using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.Inputs.Controls
{
	enum ControlGroup : byte
	{
		TextInput = 0,
		SelectionInput = 1,
		PaginableSelectionInput = 2,
		ShipPlacement = 3
	}
}
