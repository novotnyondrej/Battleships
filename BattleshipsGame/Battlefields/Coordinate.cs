using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Battleships.BattleshipsGame.Battlefields
{
	//Souradnice v bitevnim poli
	class Coordinate
	{
		//Ciselne 
		public byte X { get; }
		public byte Y { get; }
		//Textove souradnice
		[JsonIgnore]
		public string Row { get => Coordinate.RowToString(Y); }
		[JsonIgnore]
		public string Column { get => Coordinate.ColumnToString(X); }

		//https://www.daniweb.com/programming/software-development/threads/368070/array-of-letters-of-the-alphabet-in-c
		private static IEnumerable<char> ColumnChars = Enumerable.Range('A', 'Z' - 'A' + 1).Select(number => (char)number);
		private static byte ColumnCharsCount; //= 26;

		static Coordinate()
		{
			ColumnCharsCount = (byte)ColumnChars.Count();
		}
		//Prevede radek na string
		static string RowToString(byte y)
		{
			return y.ToString();
		}
		//Prevede sloupec na string
		static string ColumnToString(byte x)
		{
			string result = "";
			while (x > 0)
			{
				result += x % ColumnCharsCount;
				x /= ColumnCharsCount;
			}
			return result;
		}
		public Coordinate(byte x, byte y)
		{
			X = x;
			Y = y;
		}
		public override bool Equals(object obj)
		{
			//Kontrola typu
			if (obj.GetType() != GetType()) base.Equals(obj);
			//Kontrola souradnic
			Coordinate coordinate = (Coordinate)obj;
			return (X == coordinate.X && Y == coordinate.Y);
		}
		//Zda souradnice sousedi s jinou souradnici
		public bool IsNeighborOf(Coordinate coordinate)
		{
			//Ziskani vzdalenosti
			int xDiff = Math.Abs(X - coordinate.X);
			int yDiff = Math.Abs(Y - coordinate.Y);
			//Vyhodnoceni sousedstvi
			return (xDiff <= 1 && yDiff <= 1 && coordinate != this);
		}
	}
}