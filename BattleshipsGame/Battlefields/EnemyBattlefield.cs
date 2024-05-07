using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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

		[JsonIgnore]
		//Mapa souradnic
		public IEnumerable<IEnumerable<Coordinate>> CoordinateMap { get; }
		//Sada lodi pouzita v bitevnim poli <velikost lodi, pocet>
		public IReadOnlyDictionary<BattleshipSize, byte> BattleshipSet { get; }
		//Souradnice, na ktere se strilelo
		private protected Dictionary<Coordinate, AttackResult> _AttackedCoordinates { get; }
		[JsonIgnore]
		public IReadOnlyDictionary<Coordinate, AttackResult> AttackedCoordinates { get => _AttackedCoordinates; }
		//Seznam plne odhalenych lodi
		private protected List<Battleship> _SunkenBattleships;
		public IEnumerable<Battleship> SunkenBattleships { get => _SunkenBattleships; }

		//Vyhodnoti, zda jsou vsechny lode potopeny
		//Vsechny lode jsou potopeny, pokud se pocet potopenych lodi shoduje s poctem lodi v sade
		[JsonIgnore]
		public bool AllShipsSunken
		{
			get => BattleshipSet.All(
				(pair) => pair.Value <= SunkenBattleships.Count((battleship) => battleship.Size == pair.Key)
			);
		}

		
		public EnemyBattlefield(byte width, byte? height = null, IReadOnlyDictionary<BattleshipSize, byte> battleshipSet = null)
		{
			byte trueHeight = height ?? width;
			//Nacteni atributu
			Width = width;
			Height = trueHeight;
			BattleshipSet = battleshipSet ?? GetAllBattleshipSets(width, trueHeight).First();

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
		//Zda muze byt souradnice napadena
		public bool CanBeAttacked(Coordinate coordinate)
		{
			return !_AttackedCoordinates.ContainsKey(coordinate);
		}
		//Ziska znak pro dane pole
		public virtual string GetColorAtCoordinate(Coordinate coordinate, bool secure = true)
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
			if (attack.Key == coordinate && attack.Value == AttackResult.Hit)
			{
				//Tato souradnice je soucasti nepotopene lodi
				return "BackgroundRed";
			}
			else if (attack.Key == coordinate && attack.Value == AttackResult.Miss)
			{
				//Tato souradnice jiz byla ozkousena
				return "BackgroundDarkGray";
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
			ConsoleColor highlightColor = ConsoleColor.DarkBlue,
			bool secure = true
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
						string color = GetColorAtCoordinate(GetCoordinate(x, y), secure);
						//Pokud je radek nebo sloupec oznacen, pridame pozadi
						if (selectedCoordinate != null && selectedCoordinate.Y == y && selectedCoordinate.X == x)
						{
							character = "<Gray>[]</Gray>";
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
		public static IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> GetAllBattleshipSets(byte width, byte? height = null, int percentageCovered = 15, int maximumOfEach = 5)
		{
			if (percentageCovered > 100) percentageCovered = 100;
			else if (percentageCovered < 0) percentageCovered = 0;

			byte trueHeight = height ?? width;
			//Vypocet celkoveho poctu policek
			int fieldsCount = width * trueHeight;
			//Vypocet policek s lodema
			int shipFieldsCount = (int)(fieldsCount * (percentageCovered / (float)100));

			//Ruzne variace sestav
			List<Dictionary<BattleshipSize, byte>> sets = new();
			//Co jeste zbyva k vypocteni
			List<(BattleshipSize level, Dictionary<BattleshipSize, byte> set, int remaining)> toCheck = new()
			{
				(
					BattleshipSize.Carrier,
					new() { },
					shipFieldsCount
				)
			};
			//Nacitani ruznych variaci
			while (toCheck.Count > 0 && sets.Count < 7)
			{
				//Ziskani dalsiho navrhu
				(BattleshipSize level, Dictionary<BattleshipSize, byte> set, int remaining) = toCheck[0];
				toCheck.RemoveAt(0);
				//Pro kazdou lod co je mensi nebo stejne velka se nactou moznosti
				for (int size = (int)level; size >= 2; size--)
				{
					//Ziskani velikosti
					BattleshipSize currentLevel = (BattleshipSize)size;
					//Ziskani aktualniho poctu
					set.TryGetValue(currentLevel, out byte count);
					//Kontrola poctu
					if (count >= maximumOfEach) continue;
					//Kontrola poctu dostupnych policek
					if (remaining - size < 0) continue;
					
					Dictionary<BattleshipSize, byte> currentSet = set.ToDictionary((pair) => pair.Key, (pair) => pair.Value);
					int currentRemaining = remaining - size;
					//Pridani lode
					if (!currentSet.ContainsKey(currentLevel)) currentSet[currentLevel] = 0;
					currentSet[currentLevel]++;
					//Pridani do kontroly
					if (currentRemaining == 0) sets.Add(currentSet);
					else toCheck.Add((currentLevel, currentSet, currentRemaining));
				}
			}
			return sets;
		}
	}
}