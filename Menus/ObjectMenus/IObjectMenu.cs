using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.Menus.ObjectMenus
{
	//Menu pro specificky objekt
	interface IObjectMenu<OfType> : IMenu
	{
		public void Show(OfType obj = default);
	}
}
