﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.Inputs.Controls;

namespace Battleships.Inputs
{
	//Trida pro komunikaci s uzivatelem (metody pro zobrazeni otazek/moznosti/duvodu a pro zaznamenani odpovedi)
	static class InputManager
	{
		//Barvicky
		public static ConsoleColor QuestionColor = ConsoleColor.Blue;
		public static ConsoleColor SelectionColor = ConsoleColor.Blue;
		public static ConsoleColor InputColor = ConsoleColor.White;
		public static ConsoleColor ReasonColor = ConsoleColor.DarkRed;
		//Znak oznacujici uzivatelsky vstup
		public static string InputCharacter { get => "  > "; }
		//Znak oznacujici chybne zadany vstup
		public static string ReasonCharacter { get => "  ! "; }


		//Pocatecni pozice kurzoru
		private static (int Left, int Top) InitialCursorPosition;
		//Seznam pozic kurzoru pro danou otazku
		private static List<(int Left, int Top)> InputCursorPositions = new();
		private static List<(string text, bool available, string reason)> Options = new();


		//Pracovani s kurzorem
		//Ziska pocet charakteru mezi jednotlivymi kurzory
		private static int CursorDifference((int Left, int Top) fromCursor, (int Left, int Top) toCursor)
		{
			//Ziskani celkove sirky konzole
			int totalWidth = Console.WindowWidth;
			//Vypocet charakteru mezi kurzory
			return totalWidth * (fromCursor.Top - toCursor.Top) - toCursor.Left + fromCursor.Left;
		}
		//Posune kurzor o dany pocet charakteru
		private static (int Left, int Top) MoveCursor((int Left, int Top) cursor, int amount)
		{
			int consoleWidth = Console.WindowWidth;
			//Vypocet poctu radku
			int top = cursor.Top + (int)Math.Floor((cursor.Left + amount) / (float)consoleWidth);
			int left = (cursor.Left + amount) % consoleWidth;

			if (top < 0) top = 0;
			if (left < 0) left += consoleWidth;
			return (left, top);
		}
		//Smaze vsechno mezi danymi pozicemi
		private static void ClearBetween((int Left, int Top) fromCursor, (int Left, int Top) toCursor)
		{
			//Zda je to pred from
			bool reverse = (fromCursor.Top > toCursor.Top || (fromCursor.Top == toCursor.Top && fromCursor.Left > toCursor.Left));
			//Pravy pocatecni kurzor
			(int Left, int Top) realFromCursor = (reverse ? toCursor : fromCursor);
			//Ziskani aktualni pozice kurzoru
			(int Left, int Top) original = Console.GetCursorPosition();

			//Vypocet charakteru k promazani
			int charactersToClear = CursorDifference(fromCursor, toCursor);

			//Promazani charakteru
			Console.SetCursorPosition(realFromCursor.Left, realFromCursor.Top);
			Console.Write(String.Join("", Enumerable.Repeat(" ", charactersToClear)));
			//Navraceni na originalni pozici
			Console.SetCursorPosition(original.Left, original.Top);
		}
		//Navrati kurzor na puvodni pozici, popr. vymaze vsechny charaktery mezi pocatecni a aktualni pozici
		public static void RevertCursor((int Left, int Top) initial, bool clear = true)
		{
			//Promazani vseho mezi pocatecni a aktualni pozici
			if (clear) ClearBetween(Console.GetCursorPosition(), initial);
			//Nastaveni pozice na puvodni pozici
			Console.SetCursorPosition(initial.Left, initial.Top);
		}
		//Vrati kurzor na puvodni pozici pred vypisem otazky
		public static void RevertCursorToInitial(bool clear = true) => RevertCursor(InitialCursorPosition, clear);
		//Vrati kurzor na puvodni pozici pred vstupem
		private static void RevertCursorToInput(int? inputIndex = null, bool clear = false)
		{
			//Kontrola, ze vstup existuje
			if (InputCursorPositions.Count <= (inputIndex ?? 0)) return;
			//Nacraceni kurzoru
			RevertCursor(InputCursorPositions[inputIndex ?? 0], clear);
		}


		//Vypisovani textu
		//Vypise text
		public static void Write(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
		{
			text = text.Replace("SelectionColor>", SelectionColor.ToString() + ">");
			text = text.Replace("ReasonColor>", ReasonColor.ToString() + ">");

			bool visible = Console.CursorVisible;
			//Pocatecni barvy
			ConsoleColor originalForegroundColor = Console.ForegroundColor;
			ConsoleColor originalBackgroundColor = Console.BackgroundColor;
			//Barvy pouzite pri vypisu
			List<ConsoleColor> foregroundColorHistory = new() { foregroundColor ?? Console.ForegroundColor };
			List<ConsoleColor> backgroundColorHistory = new() { backgroundColor ?? Console.BackgroundColor };
			Console.CursorVisible = false;
			Console.ForegroundColor = foregroundColor ?? Console.ForegroundColor;
			Console.BackgroundColor = foregroundColor ?? Console.BackgroundColor;

			//Psani textu dokud je porad co psat
			while (text.Length > 0)
			{
				//Jak velka cast textu se ma vypsat
				int writeUntil = text.Length;
				//Nalezeni dalsi definice barvy
				int colorStart = text.IndexOf('<');
				int colorEnd = text.IndexOf('>', colorStart + 1);
				///Pokud definice existuje, tak se pokusime ziskat novou barvu
				if (colorStart >= 0 && colorEnd > 0)
				{
					//Ziskani barvy jako string
					string colorText = text[(colorStart + 1)..colorEnd];
					//Zda se v tento moment ma barva ukoncit
					bool endColor = colorText.StartsWith('/');
					if (endColor) colorText = colorText[1..];
					//Zda se jedna o pozadi
					bool isBackground = colorText.StartsWith("Background");
					if (isBackground) colorText = colorText["Background".Length..];
					//Pokus o rozlusteni barvy
					bool isColor = Enum.TryParse(colorText, out ConsoleColor newColor);
					//Pokud barva existuje, tak zmenit
					if (isColor)
					{
						//Vypsani textu az po definici
						Console.Write(text[..colorStart]);
						//Pristi vypis probehne az od konce definice
						text = text[(colorEnd + 1)..];

						if (isBackground)
						{
							//Pokud je konec barvy, pak nastavime barvu zpet na original pred aktualni barvou
							if (endColor)
							{
								//Kontrola existence barvy (zaroven index nemuze byt 0 protoze prvni barva je vzdy uplne puvodni barva pred vypisem)
								if (backgroundColorHistory.LastIndexOf(newColor) < 1) continue;
								//Odebrani barvy z historie
								backgroundColorHistory = backgroundColorHistory.GetRange(0, backgroundColorHistory.LastIndexOf(newColor));
								//Nastaveni barvy na posledni barvu pred aktualni barvou
								Console.BackgroundColor = backgroundColorHistory.Last();
							}
							else
							{
								//Zmena barvy na pzoadovanou barvu
								Console.BackgroundColor = newColor;
								//Pridani barvy do historie
								backgroundColorHistory.Add(newColor);
							}
						}
						else
						{
							//Pokud je konec barvy, pak nastavime barvu zpet na original pred aktualni barvou
							if (endColor)
							{
								//Kontrola existence barvy (zaroven index nemuze byt 0 protoze prvni barva je vzdy uplne puvodni barva pred vypisem)
								if (foregroundColorHistory.LastIndexOf(newColor) < 1) continue;
								//Odebrani barvy z historie
								foregroundColorHistory = foregroundColorHistory.GetRange(0, foregroundColorHistory.LastIndexOf(newColor));
								//Nastaveni barvy na posledni barvu pred aktualni barvou
								Console.ForegroundColor = foregroundColorHistory.Last();
							}
							else
							{
								//Zmena barvy na pzoadovanou barvu
								Console.ForegroundColor = newColor;
								//Pridani barvy do historie
								foregroundColorHistory.Add(newColor);
							}
						}
						continue;
					}
					else writeUntil = colorEnd + 1;
				}
				else if (colorStart >= 0) writeUntil = colorStart + 1;
				
				//Vypsani casti textu
				Console.Write(text[..writeUntil]);
				text = text[writeUntil..];
			}
			Console.CursorVisible = visible;
			//Navraceni barvy na original
			Console.ForegroundColor = originalForegroundColor;
			Console.BackgroundColor = originalBackgroundColor;
		}
		//Vypise text a zalomi radek
		public static void WriteLine(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) => Write(text + "\n", foregroundColor, backgroundColor);
		//Vypise text a zbyvajici misto, ktere zbyva k danemu kurzoru, vyplni mezerami
		private static void WriteUntilCursor(string text, (int Left, int Top) upToCursor)
		{
			//Ziskani rozdilu mezi kurzory
			int difference = CursorDifference(upToCursor, Console.GetCursorPosition());
			//Kontrola, ze se cilovy kurzor nachazi za aktualnim
			if (difference > 0) text = text.PadRight(difference, ' ');

			Write(text);
		}
		//Vypise text az po konec radku
		private static void WriteUntilEnd(string text = default)
		{
			Write(text);
			Write("".PadRight(Console.WindowWidth - Console.GetCursorPosition().Left, ' '));
		}


		//Zalomi radek
		private static void BreakLine(bool really = true)
		{
			//Kontrola, ze radek ma byt zalomen
			if (!really) return;
			//Kontrola, ze se kurzor nachazi kdekoli jinde jenom ne na zacatku radku
			if (Console.GetCursorPosition().Left == 0) return;
			//Zalomeni radku
			Console.WriteLine();
		}
		//Vypise otazku uzivateli
		public static void PrintQuestion(string question, bool withInputCharacter = true, bool noClear = false, bool forceNewLine = true)
		{
			//Promazani obrazovky pokud povoleno
			if (!noClear) Console.Clear();
			//Pokud neni promazani obrazovky povoleno, zalomime pouze radek
			else BreakLine(forceNewLine);

			//Nova otazka => obnoveni kurzoru
			InitialCursorPosition = Console.GetCursorPosition();
			InputCursorPositions.Clear();
			Options.Clear();
			//Vypsani otazky
			ConsoleColor original = Console.ForegroundColor;
			Console.ForegroundColor = QuestionColor;
			WriteLine(question);
			Console.ForegroundColor = original;
			//Vypsani znaku oznacujici uzivatelsky vstup
			if (withInputCharacter) PrintInputCharacter();
		}
		//Vypise oduvodneni uzivateli
		public static void PrintReason(string reason, bool startOnNewLine = true, bool endOnNewLine = true, bool withInputCharacter = false)
		{
			//Zalomeni na novy radek
			BreakLine(startOnNewLine);
			//Vypsani duvodu
			ConsoleColor original = Console.ForegroundColor;
			Console.ForegroundColor = ReasonColor;
			
			if (!endOnNewLine) Write(ReasonCharacter + reason);
			else WriteLine(ReasonCharacter + reason);

			Console.ForegroundColor = original;
			//Opetovne vypsani znaku pro uzivatelsky vstup (pokud povoleno)
			if (withInputCharacter) PrintInputCharacter();
		}
		//Vypise uzivateli moznost
		public static void PrintOption(string option, bool available, string reason, bool selected = false, bool savePosition = true, bool forceNewLine = true)
		{
			//Vypsani znaku pro vstup
			PrintInputCharacter(forceNewLine, selected, savePosition);
			option = option.Replace("\n", "\n" + String.Join("", Enumerable.Repeat(" ", InputCharacter.Length)));
			reason = reason.Replace("\n", "\n" + String.Join("", Enumerable.Repeat(" ", InputCharacter.Length)));
			
			Options.Add((option, available, reason));
			ReprintOption(Options.Count - 1, selected, forceNewLine);
		}
		//Znovu vypise moznost
		private static void ReprintOption(int optionIndex, bool selected = false, bool forceNewLine = true)
		{
			//Kontrola existence
			if (optionIndex < 0 || optionIndex >= Options.Count || optionIndex > InputCursorPositions.Count) return;
			//Ziskani pozice kurzoru
			(int Left, int Top) position = InputCursorPositions.ElementAtOrDefault(optionIndex);
			//Ziskani moznosti
			(string text, bool available, string reason) = Options.ElementAt(optionIndex);

			//Nastaveni pozice
			Console.SetCursorPosition(position.Left, position.Top);
			//Vypsani znaku pro vstup
			PrintInputCharacter(forceNewLine, selected, false);
			//Vypsani moznosti
			ConsoleColor original = Console.ForegroundColor;
			Console.ForegroundColor = (selected ? SelectionColor : InputColor);
			Write(text);

			if (!available)
			{
				Console.ForegroundColor = ReasonColor;
				Write(" (" + reason + ")");
			}
			Write("\n");
			Console.ForegroundColor = original;
		}
		//Vybere moznost
		public static void SelectOption(int selectedOptionIndex)
		{
			//Kontrola poctu moznosti
			if (!Options.Any()) return;
			//Pocatecni pozice kurzoru
			(int Left, int Top) initialPosition = Console.GetCursorPosition();

			//Oznaceni moznosti
			for (int optionIndex = 0; optionIndex < Options.Count; optionIndex++)
			{
				ReprintOption(optionIndex, optionIndex == selectedOptionIndex, false);
			}
			Console.SetCursorPosition(initialPosition.Left, initialPosition.Top);
		}
		//Vypise znak oznacujici uzivatelsky vstup
		private static void PrintInputCharacter(bool forceNewLine = true, bool selected = false, bool savePosition = true)
		{
			//Zalomeni radku pokud nam to bylo narizeno
			BreakLine(forceNewLine);
			//Pridani kurzoru do seznamu
			if (savePosition) InputCursorPositions.Add(Console.GetCursorPosition());
			//Vypsani znaku pro vstup
			ConsoleColor original = Console.ForegroundColor;
			Console.ForegroundColor = (selected ? SelectionColor : InputColor);
			Write(InputCharacter);
			Console.ForegroundColor = original;
		}
		
		
		//Vstupy
		//Vstup pro textovy retezec s moznosti validace
		//Vzdy vrati bud validni odpoved nebo null
		public static string? ReadLine(Func<string, (bool Valid, string Message)>? validation = null)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = InputColor;
			
			//Pocatecni pozice kurzoru
			(int Left, int Top) initialPosition = Console.GetCursorPosition();
			(int Left, int Top) fullExtent = initialPosition;
			int consoleWidth = Console.WindowWidth;

			//Zda existuje validace
			bool hasValidation = (validation is not null);
			//Vysledny retezec
			string result = "";
			//Stavy
			(bool valid, string reason) = (hasValidation ? validation(result) : (true, default));
			bool end = false;
			//Pozice kurzoru v retezci
			int characterIndex = 0;

			//Cteni vstupu
			while (!(end && valid))
			{
				end = false;
				//Cteni dalsiho charakteru
				//Pokud o ziskani specialniho vstupu
				ConsoleKeyInfo next = ReadKey();
				Control special = ControlManager.KeyToControlInGroup(next.Key, ControlGroup.TextInput);

				//Pokud se nejedna o specialni vstup, klasicky pridame charakter do retezce a zvalidujeme ho
				if (special == Control.Unknown)
				{
					//Pridani charakteru
					result =
						result[..characterIndex] +
						next.KeyChar +
						result[characterIndex..];
					characterIndex += next.KeyChar.ToString().Length;
				}
				//Reakce na specialni vstupy
				//Zruseni vstupu
				else if (special == Control.TextInputCancel)
				{
					Console.ForegroundColor = originalColor;
					return null;
				}
				//Posunuti kurzoru vlevo
				else if (special == Control.TextInputLeft)
				{
					if (characterIndex > 0) characterIndex--;
				}
				//Posunuti kurzoru vpravo
				else if (special == Control.TextInputRight)
				{
					if (characterIndex < result.Length) characterIndex++;
				}
				//Posunuti kurzoru nahoru
				else if (special == Control.TextInputUp)
				{
					if (characterIndex >= consoleWidth) characterIndex -= consoleWidth;
				}
				//Posunuti kurzoru dolu
				else if (special == Control.TextInputDown)
				{
					if (characterIndex + consoleWidth <= result.Length) characterIndex += consoleWidth;
				}
				//Odebrani posledniho znaku
				else if (special == Control.Back)
				{
					if (characterIndex > 0)
					{
						//Odebrani charakteru na aktualni pozici
						result =
							result[..(characterIndex - 1)] +
							(characterIndex < result.Length ? result[characterIndex..] : "");
						//Zmena pozice kurzoru
						characterIndex--;
					}
				}
				//Odebrani nasledujiciho znaku
				else if (special == Control.Delete)
				{
					if (characterIndex < result.Length)
					{
						//Odebrani charakteru na aktualni pozici
						result =
							result[..characterIndex] +
							(characterIndex + 1 < result.Length ? result[(characterIndex + 1)..] : "");
					}
				}
				//Potvrzeni textu
				else if (special == Control.Confirm)
				{
					//Kontrola validity
					if (valid) end = true;
				}
				//Validace
				(valid, reason) = (hasValidation ? validation(result.Trim()) : (true, default));
				//Prerendrovani textu
				//Cokoliv co je za aktualni pozici kurzoru musi byt aktualizovano
				string renderText = result[characterIndex..];
				
				Console.CursorVisible = false;
				//Aktualizace pozice kurzoru
				(int Left, int Top) beforeRender = MoveCursor(initialPosition, characterIndex);
				Console.SetCursorPosition(beforeRender.Left, beforeRender.Top);

				//Aktualizace textu
				WriteUntilEnd(renderText);
				//Vypsani oduvodneni
				if (reason != default) PrintReason(reason, true, false);
				(int Left, int Top) newFullExtent = Console.GetCursorPosition();
				//Promazani textu po predchozi rozsah
				WriteUntilCursor("", fullExtent);
				fullExtent = newFullExtent;

				Console.SetCursorPosition(beforeRender.Left, beforeRender.Top);
				Console.CursorVisible = true;
			}
			Console.ForegroundColor = originalColor;

			return result.Trim();
		}
		//Precte dalsi charakter
		public static ConsoleKeyInfo ReadKey(bool showCursor = true, bool display = true)
		{
			bool original = Console.CursorVisible;
			Console.CursorVisible = showCursor;
			ConsoleKeyInfo result = Console.ReadKey(!display);
			Console.CursorVisible = original;
			return result;
		}
		//Ziska vstup, ktery nalezi urcite skupine
		public static Control GetControlOfGroup(ControlGroup group, byte maximumAttempts = 16)
		{
			//Kontrola, ze skupina existuje
			if (!ControlManager.HasGroup(group)) return default;
			//Vysledny vstup
			Control result = default;
			//Pocet pokusu
			int attempts = 0;
			do
			{
				//Precteni znaku
				result = ControlManager.KeyToControlInGroup(ReadKey(false, false).Key, group);
				attempts++;
			}
			while (result == Control.Unknown && (maximumAttempts <= 0 || attempts < maximumAttempts));
			//Pokud je vstup neznamy, znamena to, ze limit poctu pokusu byl prekrocen
			if (result == Control.Unknown)
			{
				//Vraceni prvniho mozneho vstupu
				return ControlManager.ControlGroups[group].First();
			}
			return result;
		}
	}
}