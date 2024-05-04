using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.Inputs.Controls
{
	static class ControlManager
	{
		private static Dictionary<Control, IEnumerable<ConsoleKey>> DefaultControlTranslationMap = new()
		{
			{ Control.Confirm, new List<ConsoleKey>() { ConsoleKey.Enter } },
			{ Control.Back, new List<ConsoleKey>() { ConsoleKey.Backspace } },
			{ Control.Delete, new List<ConsoleKey>() { ConsoleKey.Delete} },
			{ Control.TextInputCancel, new List<ConsoleKey>() { ConsoleKey.Escape } },
			{ Control.Cancel, new List<ConsoleKey>() { ConsoleKey.Escape, ConsoleKey.Backspace, ConsoleKey.Delete } },

			{ Control.Up, new List<ConsoleKey>() { ConsoleKey.W, ConsoleKey.U, ConsoleKey.UpArrow, ConsoleKey.NumPad8 } },
			{ Control.Right, new List<ConsoleKey>() { ConsoleKey.D, ConsoleKey.K, ConsoleKey.RightArrow, ConsoleKey.NumPad6} },
			{ Control.Down, new List<ConsoleKey>() { ConsoleKey.S, ConsoleKey.J, ConsoleKey.DownArrow, ConsoleKey.NumPad5 } },
			{ Control.Left, new List<ConsoleKey>() { ConsoleKey.A, ConsoleKey.H, ConsoleKey.LeftArrow, ConsoleKey.NumPad4 } },
			{ Control.TextInputUp, new List<ConsoleKey>() { ConsoleKey.UpArrow} },
			{ Control.TextInputRight, new List<ConsoleKey>() { ConsoleKey.RightArrow} },
			{ Control.TextInputDown, new List<ConsoleKey>() { ConsoleKey.DownArrow} },
			{ Control.TextInputLeft, new List<ConsoleKey>() { ConsoleKey.LeftArrow} }
		};
		public static Dictionary<Control, IEnumerable<ConsoleKey>> ControlTranslationMap => DefaultControlTranslationMap;

		private static Dictionary<ControlGroup, IEnumerable<Control>> _ControlGroups = new()
		{
			{ ControlGroup.TextInput, new List<Control>() { Control.Back, Control.Delete, Control.Confirm, Control.TextInputCancel, Control.TextInputUp, Control.TextInputRight, Control.TextInputDown, Control.TextInputLeft } },
			{ ControlGroup.SelectionInput, new List<Control>() { Control.Cancel, Control.Confirm, Control.Up, Control.Down } },
			{ ControlGroup.PaginableSelectionInput, new List<Control>() { Control.Cancel, Control.Confirm, Control.Up, Control.Right, Control.Down, Control.Left } }
		};
		public static Dictionary<ControlGroup, IEnumerable<Control>> ControlGroups => _ControlGroups;

		public static bool HasGroup(ControlGroup group)
		{
			return ControlGroups.ContainsKey(group) && ControlGroups[group].Any();
		}
		public static bool HasControl(Control control)
		{
			return ControlTranslationMap.ContainsKey(control) && ControlTranslationMap[control].Any();
		}
		public static Control KeyToControlInGroup(ConsoleKey key, ControlGroup group)
		{
			if (!HasGroup(group)) return default;

			return ControlGroups[group].FirstOrDefault(
				control => HasControl(control) && ControlTranslationMap[control].Contains(key)
			);
		}
		public static string GetControlAsString(Control control)
		{
			if (!HasControl(control)) return default;

			return String.Join(", ", ControlTranslationMap[control].Select(
				(key) => key.ToString()
			));
		}
	}
}