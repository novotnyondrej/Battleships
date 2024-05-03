using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.Inputs.Controls;
using Battleships.Content;

namespace Battleships.Inputs
{
	//Trida pro uzivatelske vstupy
	static class Input
	{
		//Textovy vstup s minimalni a maximalni delkou textu
		public static string? TextInput(TranslationKey questionTranslationKey, uint minLength = 0, uint maxLength = 0)
		{
			bool hasMinLength = minLength > 0;
			bool hasMaxLength = maxLength > 0;
			//Kontrola spravneho poradi minimalni a maximalni hodnoty
			if (hasMinLength && hasMaxLength && minLength > maxLength)
			{
				//Prohozeni hodnot
				minLength += maxLength;
				maxLength = minLength - maxLength;
				minLength -= maxLength;
			}
			//Vypsani otazky
			InputManager.PrintQuestion(ContentManager.GetTranslation(questionTranslationKey));
			//Ziskani odpovedi
			string response = InputManager.ReadLine(delegate(string response)
			{
				//Kontrola delky textu
				if (hasMinLength && response.Length < minLength) return (false, String.Format(ContentManager.GetTranslation(TranslationKey.TextTooShort), minLength));
				if (hasMaxLength && response.Length > maxLength) return (false, String.Format(ContentManager.GetTranslation(TranslationKey.TextTooLong), maxLength));
				//Text je platny
				return (true, default);
			});
			return response;
		}
		//Vstup pro zadani cisla s minimalni a maximalni hodnotou
		public static int? IntInput(TranslationKey questionTranslationKey, int? min = null, int? max = null)
		{
			min ??= int.MinValue;
			max ??= int.MaxValue;
			//Kontrola spravneho poradi minimalni a maximalni hodnoty
			if (min > max)
			{
				//Prohozeni hodnot
				min += max;
				max = min - max;
				min -= max;
			}
			//Vypsani otazky
			InputManager.PrintQuestion(ContentManager.GetTranslation(questionTranslationKey));
			//Ziskani odpovedi
			string response = InputManager.ReadLine(delegate (string response)
			{
				//Pokus o prevod na cislo
				int value;
				try
				{
					value = int.Parse(response);
				}
				catch (OverflowException e)
				{
					return (false, ContentManager.GetTranslation(TranslationKey.OutOfRange));
				}
				catch (Exception e)
				{
					return (false, ContentManager.GetTranslation(TranslationKey.NaN));
				}
				//Kontrola hodnoty
				if (value < min) return (false, String.Format(ContentManager.GetTranslation(TranslationKey.TooSmallNumber), min));
				if (value > max) return (false, String.Format(ContentManager.GetTranslation(TranslationKey.TooBigNumber), max));
				//Hodnota je platna
				return (true, default);
			});
			if (response == default) return null;

			return int.Parse(response);
		}
		//Vstup pro vyber z moznosti
		public static int SelectionInput<T>(TranslationKey questionTranslationKey, IEnumerable<T> options)
		{
			//Celkovy pocet moznosti
			int optionsCount = options.Count();
			if (optionsCount <= 0) return -1;

			bool visible = Console.CursorVisible;
			Console.CursorVisible = false;

			//Vypsani otazky
			InputManager.PrintQuestion(ContentManager.GetTranslation(questionTranslationKey), false);
			//Vypsani vsech moznosti
			foreach (T option in options) InputManager.PrintOption(option.ToString());
			//Aktualne vybrana moznost
			int selectedIndex = 0;
			bool selected = false;
			do
			{
				//Vybrani moznosti
				InputManager.SelectOption(selectedIndex);
				//Ziskani vstupu
				Control control = InputManager.GetControlOfGroup(ControlGroup.SelectionInput);
				//Reakce na vstup
				if (control == Control.Cancel)
				{
					Console.CursorVisible = visible;
					return -1;
				}
				else if (control == Control.Confirm)
				{
					//Ukotveni moznosti
					selected = true;
				}
				else if (control == Control.Up)
				{
					//O moznost nahoru
					selectedIndex -= 1;
					if (selectedIndex < 0) selectedIndex += optionsCount;
				}
				else if (control == Control.Down)
				{
					//O moznost dolu
					selectedIndex += 1;
					if (selectedIndex >= optionsCount) selectedIndex -= optionsCount;
				}
			}
			while (!selected);

			Console.CursorVisible = visible;
			return selectedIndex;
		}
	}
}