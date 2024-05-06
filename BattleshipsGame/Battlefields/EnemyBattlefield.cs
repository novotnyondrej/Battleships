using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battleships;

namespace Battleships.BattleshipsGame.Battlefields
{
	//Bitevni pole nepritele. Oproti klasickemu bitevnimu poli nema tento typ pole prehled o pozici lodi
	abstract class EnemyBattlefield
	{
		//Velikost bitevniho pole
		public byte Width { get; }
		public byte Height { get; }

		//Mapa souradnic
		public IEnumerable<IEnumerable<Coordinate>> CoordinateMap { get; }
		//Sada lodi pouzita v bitevnim poli <velikost lodi, pocet>
		public IReadOnlyDictionary<BattleshipSize, byte> BattleshipSet { get; }
		//Souradnice, na ktere se strilelo
		private protected Dictionary<Coordinate, AttackResult> _AttackedCoordinates { get; }
		public IReadOnlyDictionary<Coordinate, AttackResult> AttackedCoordinates { get => _AttackedCoordinates; }
		//Seznam plne odhalenych lodi
		private protected List<Battleship> _SunkenBattleships;
		public IEnumerable<Battleship> SunkenBattleships { get => _SunkenBattleships; }

		//Vyhodnoti, zda jsou vsechny lode potopeny
		//Vsechny lode jsou potopeny, pokud se pocet potopenych lodi shoduje s poctem lodi v sade
		public bool AllShipsSunken
		{
			get => SunkenBattleships.GroupBy(
				ship => ship.Size
			).All(
				group => BattleshipSet[group.First().Size] <= group.Count()
			);
		}

		public EnemyBattlefield(byte width, byte? height = null, IReadOnlyDictionary<BattleshipSize, byte> battleshipSet = null)
		{
			byte trueHeight = height ?? width;
			//Nacteni atributu
			Width = width;
			Height = trueHeight;
			BattleshipSet = battleshipSet ?? EnemyBattlefield.GetAllBattleshipSets(width, trueHeight, true).First();

			//Vytvoreni mapy souradnic
			List<List<Coordinate>> coordinateMap = new();
			//Nacteni souradnic
			for (byte x = 0; x < width; x++)
			{
				List<Coordinate> row = new();

				for (byte y = 0; y < trueHeight; y++)
				{
					//Pridani souradnice
					row.Add(new(x, y));	
				}
				coordinateMap.Add(row);
			}
			CoordinateMap = coordinateMap;
			
			_AttackedCoordinates = new();
			_SunkenBattleships = new();
		}
		//Zda souradnice existuje
		public bool CoordinateExists(byte x, byte y)
		{
			return x < Width && y < Height;
		}
		public Coordinate GetCoordinate(byte x, byte y)
		{
			//Kontrola existence souradnice
			if (!CoordinateExists(x, y)) return null;
			//Vraceni souradnice
			return CoordinateMap.ElementAt(x).ElementAt(y);
		}
		//Ziska znak pro dane pole
		public virtual string GetColorAtCoordinate(Coordinate coordinate)
		{
			if (_SunkenBattleships.Any((battleship) => battleship.TotalPosition.Contains(coordinate)))
			{
				//Tato souradnice patri potopene lodi
				return "BackgroundDarkRed";
			}
			else if (_SunkenBattleships.Any((battleship) => battleship.TotalPosition.Any((position) => position.IsNeighborOf(coordinate))))
			{
				//Tato souradnice je v blizkosti potopene lodi
				return "BackgroundDarkGray";
			}
			//Pokus o nalezeni pokusu o utok
			KeyValuePair<Coordinate, AttackResult> attack = _AttackedCoordinates.FirstOrDefault((pair) => pair.Key == coordinate);
			//Pokud utok neexistuje, pak vrati prazdne pole
			return "";
			if (attack.Value == AttackResult.Hit)
			{
				//Tato souradnice je soucasti nepotopene lodi
				return "BackgroundRed";
			}
			return "";
		}
		//Prevede bitevni pole na text
		public override string ToString()
		{
			return ToString(null, null);
		}
		public string ToString(
			Coordinate selectedCoordinate = null,
			IEnumerable<Coordinate> highlightedCoordinates = null,
			ConsoleColor highlightColor = ConsoleColor.DarkBlue
		)
		{
			//Vysledek
			string result = "";
			string columnCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			//Maximalni sirka zahlavi
			int maximumHeaderWidth = Height.ToString().Length + 2;
			//Nacteni hlavicky
			if (selectedCoordinate != null) result += String.Join("", Enumerable.Repeat(" ", maximumHeaderWidth + 1 + selectedCoordinate.X * 2)) + "<BackgroundDarkGray>  </BackgroundDarkGray>" + String.Join("", Enumerable.Repeat(" ", (Width - 1 - selectedCoordinate.X) * 2));
			else result += String.Join("", Enumerable.Repeat(" ", maximumHeaderWidth + 1 + Width * 2));
			//Nacteni sloupcu
			string columns = String.Join(" ", (columnCharacters[..Width]).ToCharArray()) + " ";
			//Oznaceni sloupce
			if (selectedCoordinate != null) columns = columns.Replace(columnCharacters[selectedCoordinate.X] + " ", "<BackgroundDarkGray>" + columnCharacters[selectedCoordinate.X] + " </BackgroundDarkGray>");
			result += "\n" + String.Join("", Enumerable.Repeat(" ", maximumHeaderWidth)) + " " + columns;
			//Nacteni oddelovace
			string separator = String.Join("", Enumerable.Repeat("--", Width));
			//Oznaceni sloupce
			if (selectedCoordinate != null) separator = separator[..(selectedCoordinate.X * 2)] + "<BackgroundDarkGray>--</BackgroundDarkGray>" + separator[((selectedCoordinate.X + 1) * 2)..];
			result += "\n" + String.Join("", Enumerable.Repeat(" ", maximumHeaderWidth)) + " " + separator;
			//Nacitani policek
			for (byte y = 0; y < Height; y++)
			{
				//Pridani zahlavi
				string row = "\n" + (y + 1).ToString().PadLeft(maximumHeaderWidth, ' ') + "|";

				if (selectedCoordinate != null && selectedCoordinate.Y == y)
				{
					row = "<BackgroundDarkGray>" + row + "</BackgroundDarkGray>";
				}
				//Pridani sloupcu
				for (byte x = 0; x < Width; x++)
				{
					string character = "  ";
					//Pokud je ve zvyrazneni tak zvyraznit
					if (selectedCoordinate != null && highlightedCoordinates != null && highlightedCoordinates.Contains(GetCoordinate(x, y)))
					{
						character = "<Background" + highlightColor.ToString() + ">  </Background" + highlightColor.ToString() + ">";
					}
					else
					{
						//Ziskani znaku
						string color = GetColorAtCoordinate(GetCoordinate(x, y));
						//Pokud je radek nebo sloupec oznacen, pridame pozadi
						if (selectedCoordinate != null && selectedCoordinate.Y == y && selectedCoordinate.X == x)
						{
							character = "<DarkGray>[]</DarkGray>";
						}
						else if (selectedCoordinate != null && selectedCoordinate.X == x)
						{
							character = "  ";
							//character = "<" + color + "><DarkGray>[]</DarkGray></" + color + ">";
						}
						if (color.Length > 0)
						{
							character = "<" + color + ">" + character + "</" + color + ">";
						}
					}
					row += character;
				}
				if (selectedCoordinate != null && selectedCoordinate.Y == y)
				{
					row += "<BackgroundDarkGray>|</BackgroundDarkGray>";
				}
				else
				{
					row += "|";
				}
				if (selectedCoordinate != null && selectedCoordinate.Y == y)
				{
					//row = "<BackgroundDarkGray>" + row + "</BackgroundDarkGray>";
				}
				result += row;
			}
			//Nacteni oddelovace
			separator = String.Join("", Enumerable.Repeat("--", Width));
			//Oznaceni sloupce
			if (selectedCoordinate != null) separator = separator[..(selectedCoordinate.X * 2)] + "<BackgroundDarkGray>--</BackgroundDarkGray>" + separator[((selectedCoordinate.X + 1) * 2)..];
			result += "\n" + String.Join("", Enumerable.Repeat(" ", maximumHeaderWidth)) + " " + separator;

			return "<Gray>" + result + "<Gray>";
		}

		//Na zaklade velikosti mapy vygeneruje ruzne sady lodi
		//Sada je serazena od nejvetsi lodi po tu nejmensi
		public static IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> GetAllBattleshipSets(byte width, byte? height = null, bool generateFirstOnly = false)
		{
			return new List<Dictionary<BattleshipSize, byte>>()
			{
				new()
				{
					{ BattleshipSize.Carrier, 1 },
					{ BattleshipSize.Battleship, 1 },
					{ BattleshipSize.Submarine, 2 },
					//{ BattleshipSize.Cruiser, 1 },
					{ BattleshipSize.PatrolBoat, 1 }
				}
			};
		}
	}
}