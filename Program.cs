using System;
using System.Collections.Generic;
using Battleships.BattleshipsGame;
using Battleships.BattleshipsGame.Players;
using Battleships.Inputs;
using Battleships.Inputs.Controls;

//Straveny cas: 14.5h
//https://github.com/novotnyondrej/Battleships
namespace Battleships
{
	class Program
	{
		static void Main()
		{
			/*Game game = Game.Create(new Player());
			while (!game.Ended)
			{
				if (game.Progress() == null) break;
			}*/
			//Input.IntInput("Zadej cislo", null, null);
			Console.WriteLine("\n" + Input.SelectionInput<int>("test", new List<int>() { 4, 5, 6 }));
		}
	}
}