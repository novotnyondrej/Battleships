using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.Inputs.Controls;
using Battleships.Content;
using Battleships.BattleshipsGame.Battleships;
using Battleships.BattleshipsGame.Battlefields;

namespace Battleships.Inputs
{
	//Trida pro uzivatelske vstupy
	static class Input
	{
		//Velikost stranky pro vyber moznosti
		public static int SelectionInputPageSize => 5;

		//Textovy vstup s minimalni a maximalni delkou textu
		public static string? TextInput(TranslationKey questionTranslationKey, uint minLength = 0, uint maxLength = 0, Func<string, (bool valid, string message)> customValidation = null)
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
				//Vlastni validace
				if (customValidation != null) return customValidation.Invoke(response);
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
		
		
		//Funkce pro zpracovani uzivatelskeho vstupu pri vyberu objektu
		private static (bool end, int selectedOptionIndex) DefaultSelectionInputHandler(Control control, int selectedOptionIndex, int optionsCount)
		{
			if (control == Control.Cancel)
			{
				//Zruseni vyberu
				return (true, -1);
			}
			else if (control == Control.Confirm)
			{
				//Potvrzeni vyberu
				return (true, selectedOptionIndex);
			}
			else if (control == Control.Up)
			{
				//O moznost nahoru
				selectedOptionIndex--;
				if (selectedOptionIndex < 0) selectedOptionIndex += optionsCount;
			}
			else if (control == Control.Down)
			{
				//O moznosti dolu
				selectedOptionIndex++;
				if (selectedOptionIndex >= optionsCount) selectedOptionIndex -= optionsCount;
			}
			return (false, selectedOptionIndex);
		}
		//Upravena funkce pro zpracovani uzivatelskeho vstupu pri vyberu objektu podporujici strankovani
		private static (bool end, int selectedOptionIndex) PaginableSelectionInputHandler(Control control, int selectedOptionIndex, int optionsCount, int previousPageIndex = -1, int nextPageIndex = -1)
		{
			if (control == Control.Right)
			{
				//Dalsi stranka
				if (nextPageIndex > 0) return (true, nextPageIndex);
				else return (false, selectedOptionIndex);
			}
			else if (control == Control.Left)
			{
				if (previousPageIndex > 0) return (true, previousPageIndex);
				else return (false, selectedOptionIndex);
			}
			return DefaultSelectionInputHandler(control, selectedOptionIndex, optionsCount);
		}
		//Vstup pro vyber z moznosti (vraci index vybrane moznosti)
		public static int SelectionInput(
			TranslationKey questionTranslationKey,
			IEnumerable<(string option, bool available, TranslationKey reasonTranslationKey)> options,
			int selectedOptionIndex = 0,
			ControlGroup? controlGroup = null,
			Func<Control, int, int, (bool end, int selectedOptionIndex)> inputHandler = null)
		{
			controlGroup ??= ControlGroup.SelectionInput;
			inputHandler ??= DefaultSelectionInputHandler;

			//Celkovy pocet moznosti
			int optionsCount = options.Count();
			if (optionsCount <= 0) return -1;

			if (selectedOptionIndex < 0) selectedOptionIndex = 0;
			if (selectedOptionIndex >= optionsCount) selectedOptionIndex = optionsCount - 1;
			
			bool visible = Console.CursorVisible;
			Console.CursorVisible = false;

			//Vypsani otazky
			InputManager.PrintQuestion(ContentManager.GetTranslation(questionTranslationKey), false);
			//Vypsani vsech moznosti
			for (int optionIndex = 0; optionIndex < optionsCount; optionIndex++)
			{
				//Ziskani moznosti
				(string option, bool available, TranslationKey reasonTranslationKey) = options.ElementAt(optionIndex);
				//Vypsani moznosti
				InputManager.PrintOption(option, available, ContentManager.GetTranslation(reasonTranslationKey));
			}
			//Zda byla moznost vybrana
			bool selected = false;
			do
			{
				//Vybrani moznosti
				InputManager.SelectOption(selectedOptionIndex);
				//Ziskani vstupu
				Control control = InputManager.GetControlOfGroup((ControlGroup)controlGroup);
				//Reakce na vstup
				(selected, selectedOptionIndex) = inputHandler.Invoke(control, selectedOptionIndex, optionsCount);
				//Pokud byla moznost vybrana, otestujeme jeji dostupnost
				if (selected && selectedOptionIndex > 0)
				{
					//Kontrola dostupnosti
					selected = options.ElementAt(selectedOptionIndex).available;
				}
			}
			while (!selected);

			Console.CursorVisible = visible;
			return selectedOptionIndex;
		}
		//Zjednoduseny vstup pro vyber z moznosti kde se predpoklada ze jsou vsechny moznosti dostupne
		public static int SelectionInput(
			TranslationKey questionTranslationKey,
			IEnumerable<string> options,
			int selectedOptionIndex = 0,
			ControlGroup? controlGroup = null,
			Func<Control, int, int, (bool end, int selectedOptionIndex)> inputHandler = null
		)
		{
			return SelectionInput(
				questionTranslationKey,
				options.Select((option) => (option, true, TranslationKey.Unknown)),
				selectedOptionIndex,
				controlGroup,
				inputHandler
			);
		}
		//Vstup pro vyber objektu z nabidky (strankovatelny)
		public static OfType ObjectSelectionInput<OfType>(
			TranslationKey questionTranslationKey,
			Func<IEnumerable<OfType>> getObjects,
			int selectedObjectIndex = 0,
			Func<OfType, (bool available, TranslationKey reasonTranslationKey)> isObjectAvailable = null,
			Func<OfType> createNewObject = null,
			TranslationKey? newObjectTranslationKey = null
		)
		{
			bool visible = Console.CursorVisible;
			Console.CursorVisible = false;

			//Velikost stranky
			int pageSize = SelectionInputPageSize;
			//Ziskani vsech objektu
			List<OfType> objects = getObjects.Invoke().ToList();
			//Celkovy pocet stranek
			int pagesCount = (int)Math.Ceiling(objects.Count / (float)pageSize);
			//Kontrola vybraneho objektu
			if (selectedObjectIndex < 0) selectedObjectIndex = 0;
			else if (selectedObjectIndex >= objects.Count) selectedObjectIndex = 0;
			//Aktualni stranka
			int pageIndex = selectedObjectIndex / pageSize;
			int pageChange = 0;
			bool selected = false;
			do
			{
				pageIndex += pageChange;

				//Ziskani moznosti pro tuto stranku
				List<(string option, bool available, TranslationKey reasonTranslationKey)> pageOptions = (
					pagesCount == 0
					? new()
					: objects.GetRange(
						pageIndex * pageSize,
						(pageIndex + 1 == pagesCount) ? objects.Count - pageIndex * pageSize : pageSize
					)
				).Select(
					(obj) =>
					{
						(bool available, TranslationKey reasonTranslationKey) = isObjectAvailable is null ? (true, default) : isObjectAvailable.Invoke(obj);
						return (obj.ToString(), available, reasonTranslationKey);
					}
				).ToList();
				//Indexy specialnich moznosti
				int insertedOptions = 0;
				int createObjectIndex = -1;
				int previousPageIndex = -1;
				int nextPageIndex = -1;
				//Moznost vytvoreni noveho objektu
				if (createNewObject is not null)
				{
					createObjectIndex = insertedOptions;
					pageOptions.Insert(insertedOptions++, (ContentManager.GetTranslation(newObjectTranslationKey ?? TranslationKey.NewObject), true, default));
				}
				//Moznost vraceni se na predchozi stranku
				if (pageIndex > 0)
				{
					previousPageIndex = insertedOptions;
					pageOptions.Insert(insertedOptions++, (ContentManager.GetTranslation(TranslationKey.PreviousPage), true, default));
				}
				//Moznost prejiti na nasledujici stranku
				if (pageIndex + 1 < pagesCount)
				{
					nextPageIndex = pageOptions.Count;
					pageOptions.Add((ContentManager.GetTranslation(TranslationKey.NextPage), true, default));
				}
				//Celkovy pocet moznosti
				int pageOptionsCount = pageOptions.Count();
				if (pageOptionsCount <= 0) return default;

				//Ziskani moznosti
				int selectedOptionIndex = SelectionInput(
					questionTranslationKey,
					pageOptions,
					pageChange > 0
					? previousPageIndex
					: (
						pageChange < 0
						? nextPageIndex
						: insertedOptions + (selectedObjectIndex > 0 ? selectedObjectIndex % pageSize : 0)
					),
					ControlGroup.PaginableSelectionInput,
					(Control control, int selectedOptionIndex, int _) => PaginableSelectionInputHandler(
						control,
						selectedOptionIndex,
						pageOptionsCount,
						previousPageIndex,
						nextPageIndex
					)
				);
				selectedObjectIndex = -1;
				pageChange = 0;
				//Reakce na vyber
				if (selectedOptionIndex == -1)
				{
					//Vyber zrusen
					selected = true;
				}
				else if (selectedOptionIndex == createObjectIndex)
				{
					//Vytvoreni objektu
					OfType obj = createNewObject.Invoke();
					if (obj != null)
					{
						//Vraceni noveho objektu
						Console.CursorVisible = visible;
						return obj;
					}
				}
				else if (selectedOptionIndex == previousPageIndex)
				{
					//Predchozi stranka
					pageChange--;
				}
				else if (selectedOptionIndex == nextPageIndex)
				{
					//Nasledujici stranka
					pageChange++;
				}
				else
				{
					//Vybran objekt
					selected = true;
					selectedObjectIndex = pageIndex * pageSize + selectedOptionIndex - insertedOptions;
				}
			}
			while (!selected);

			Console.CursorVisible = visible;
			return objects.ElementAtOrDefault(selectedObjectIndex);
		}
		//Vyber souradnice, podporuje i umistovani lodi
		private static (Coordinate coordinate, BattleshipOrientation orientation) GetCoordinate(TranslationKey questionTranslationKey, Battlefield battlefield, ControlGroup controlGroup = ControlGroup.CoordinateSelection, BattleshipSize battleshipSize = BattleshipSize.Battleship)
		{
			//Zda se poklada lod
			bool placingBattleship = controlGroup == ControlGroup.ShipPlacement;

			bool visible = Console.CursorVisible;
			Console.CursorVisible = false;
			Console.Clear();
			//Vypsani textu
			InputManager.PrintQuestion(ContentManager.GetTranslation(questionTranslationKey), false);
			(int Left, int Top) initialPosition = Console.GetCursorPosition();

			//Ruzne smery
			List<(BattleshipOrientation orientation, (int x, int y) direction)> directions = new()
			{
				(BattleshipOrientation.North, (0, -1)),
				(BattleshipOrientation.East, (1, 0)),
				(BattleshipOrientation.South, (0, 1)),
				(BattleshipOrientation.West, (-1, 0))
			};
			//Index orientace lode
			int directionIndex = 0;
			//Oznaceny radek a sloupec
			int row = (battlefield.Height / 2);
			int column = (battlefield.Width / 2);
			//Povolene meze
			int minRow = 0;
			int maxRow = battlefield.Height - 1;
			int minColumn = 0;
			int maxColumn = battlefield.Width - 1;
			//Dokud nebylo potvrzeno, tak pokracuj
			bool selected = false;
			while (!selected)
			{
				//Ziskani souradnice
				Coordinate coordinate = battlefield.GetCoordinate((byte)column, (byte)row);
				//Vyznacena policka
				IEnumerable<Coordinate> highlightedCoordinates = Enumerable.Empty<Coordinate>();
				//Barva zvyrazneni
				ConsoleColor highlightColor = ConsoleColor.White;
				if (placingBattleship)
				{
					//Ziskani vyznacenych souradnic
					highlightedCoordinates = Battleship.GetTotalPosition(battlefield, coordinate, battleshipSize, directions.ElementAt(directionIndex).orientation);
					highlightColor = battlefield.CanPlaceBattleship(coordinate, battleshipSize, directions.ElementAt(directionIndex).orientation) ? ConsoleColor.DarkBlue : ConsoleColor.DarkRed;
				}
				//Vypsani bitevniho pole
				InputManager.RevertCursor(initialPosition, false);
				InputManager.Write(battlefield.ToString(coordinate, highlightedCoordinates, highlightColor));
				//Cekani na vstup
				Control control = InputManager.GetControlOfGroup(controlGroup);
				//Reakce na vstup
				if (control == Control.Cancel) return (default, default);
				else if (control == Control.Confirm)
				{
					//Kontrola, ze lod muze byt polozena
					if (placingBattleship && !battlefield.CanPlaceBattleship(coordinate, battleshipSize, directions.ElementAt(directionIndex).orientation)) continue;
					//Potvrzeni souradnice
					selected = true;
				}
				else if (control == Control.Up)
				{
					//O radek vyse
					if (row > minRow) row--;
				}
				else if (control == Control.Right)
				{
					//O sloupec doprava
					if (column < maxColumn) column++;
				}
				else if (control == Control.Down)
				{
					//O radek nize
					if (row < maxRow) row++;
				}
				else if (control == Control.Left)
				{
					//O sloupec doleva
					if (column > minColumn) column--;
				}
				if (placingBattleship)
				{
					if (control == Control.RotateRight)
					{
						//Otocit doprava
						directionIndex++;
						if (directionIndex >= directions.Count) directionIndex -= directions.Count;
					}
					else if (control == Control.RotateLeft)
					{
						//Otocit doleva
						directionIndex--;
						if (directionIndex < 0) directionIndex += directions.Count;
					}
					(int x, int y) = directions.ElementAt(directionIndex).direction;
					//Prepocet mezi
					minRow = 0 + (y < 0 ? (int)battleshipSize - 1 : 0);
					maxRow = battlefield.Height - 1 - (y > 0 ? (int)battleshipSize - 1 : 0);
					minColumn = 0 + (x < 0 ? (int)battleshipSize - 1 : 0);
					maxColumn = battlefield.Width - 1 - (x > 0 ? (int)battleshipSize - 1 : 0);
					//Kontrola mezi
					if (row < minRow) row = minRow;
					else if (row > maxRow) row = maxRow;
					if (column < minColumn) column = minColumn;
					else if (column > maxColumn) column = maxColumn;
				}
			}
			Console.CursorVisible = visible;
			return (battlefield.GetCoordinate((byte)column, (byte)row), directions.ElementAt(directionIndex).orientation);
		}
		//Vybere souradnici na hracim poli
		public static Coordinate GetCoordinate(TranslationKey questionTranslationKey, Battlefield battlefield) => GetCoordinate(questionTranslationKey, battlefield, ControlGroup.CoordinateSelection).coordinate;
		//Vybere souradnici pro polozeni lodi
		public static (Coordinate coordinate, BattleshipOrientation orientation) PlaceBattleship(Battlefield battlefield, BattleshipSize size) => GetCoordinate(TranslationKey.PlaceBattleship, battlefield, ControlGroup.ShipPlacement, size);
	}
}