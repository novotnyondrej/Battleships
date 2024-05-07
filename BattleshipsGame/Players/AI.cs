using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Battleships.BattleshipsGame.Battlefields;
using Battleships.BattleshipsGame.Battleships;
using Battleships.Inputs;
using Battleships.Content;

namespace Battleships.BattleshipsGame.Players
{
	//Umela inteligence
	class AI : IPlayer
	{
		//Jmeno hrace
		public string Name { get; set; }
		//Statistiky
		public PlayerStatistics Statistics { get; set; }
		//Obtiznost
		public AIDificulty Difficulty { get; set; }


		public AI(AIDificulty difficulty)
		{
			Name = "John";
			Statistics = new();
			Difficulty = difficulty;
		}

		//Ziska velikost bitevniho pole
		public byte? GetBattlefieldSize(byte minimumSize = 6, byte maximumSize = 16)
		{
			//Generace nahodneho cisla
			Random random = new();
			byte size = (byte)(random.Next(minimumSize, maximumSize));
			//Navraceni velikosti pole
			return size;
		}
		//Dava hracovi moznost vybrat sadu lodi
		public IReadOnlyDictionary<BattleshipSize, byte> PickBattleshipSet(IEnumerable<IReadOnlyDictionary<BattleshipSize, byte>> sets)
		{
			return sets.First();
		}
		//Ziska souradnici, na kterou chce hrac umistit lod
		public bool PlaceBattleship(Battlefield battlefield)
		{
			BattleshipSize size = battlefield.NextMissingBattleship ?? BattleshipSize.Battleship;
			BattleshipOrientation[] orientations = new BattleshipOrientation[]
			{
				BattleshipOrientation.North,
				BattleshipOrientation.East,
				BattleshipOrientation.South,
				BattleshipOrientation.West
			};
			//Ziskani vsech dostupnych souradnic
			IEnumerable<(Coordinate coordinate, BattleshipOrientation orientation)> options = battlefield.CoordinateMap.Select(
				(row) => row.Select(
					(coordinate) => orientations.Select(
						(orientation) => (coordinate, orientation)
					).Where(
						(info) => battlefield.CanPlaceBattleship(coordinate, size, info.orientation)
					)
				).SelectMany(
					(coordinate) => coordinate
				)
			).SelectMany(
				(coordinate) => (coordinate)
			);
			//Nahodny vyber volne souradnice
			Random random = new Random();
			(Coordinate coordinate, BattleshipOrientation orientation) = options.ElementAtOrDefault(random.Next(0, options.Count()));
			
			//Polozeni lode
			if (coordinate != default) return battlefield.PlaceNextBattleship(coordinate, orientation);
			return false;
		}
		public bool PlaceAllBattleships(Battlefield battlefield)
		{
			if (battlefield.Ready) return true;
			//Informujici zprava, ze hrac poklada lode
			Console.Clear();
			InputManager.WriteLine(String.Format(ContentManager.GetTranslation(TranslationKey.StartPlacingBattleships), Name));
			//Potvrzeni
			InputManager.WriteLine(ContentManager.GetTranslation(TranslationKey.AnyKeyToContinue));
			InputManager.ReadKey(true, false);

			while (!battlefield.Ready)
			{
				if (!PlaceBattleship(battlefield)) return false;
			}
			//Zprava informujici o konci procesu
			Console.Clear();
			InputManager.WriteLine(String.Format(ContentManager.GetTranslation(TranslationKey.EndPlacingBattleships), Name));
			//Potvrzeni
			InputManager.WriteLine(ContentManager.GetTranslation(TranslationKey.AnyKeyToContinue));
			InputManager.ReadKey(true, false);

			return true;
		}
		//Ziska souradnici, na kterou chce hrac zautocit
		public Coordinate Attack(EnemyBattlefield enemyBattlefield, Battlefield ownerBattlefield)
		{
			Random random = new Random();
			IEnumerable<Coordinate> options = Enumerable.Empty<Coordinate>();
			BattleshipOrientation[] orientations = new BattleshipOrientation[]
			{
				BattleshipOrientation.North,
				BattleshipOrientation.East,
				BattleshipOrientation.South,
				BattleshipOrientation.West
			};
			if (Difficulty == AIDificulty.Hard)
			{
				//Matematicky vypocitana strela
				(Coordinate coordinate, int value) = enemyBattlefield.CoordinateMap.Select(
					(row) => row.Where(
						(coordinate) => !enemyBattlefield.AttackedCoordinates.ContainsKey(coordinate)
					).Select(
						(coordinate) => (
							coordinate,
							enemyBattlefield.BattleshipSet.Sum(
								(pair) => (pair.Value - enemyBattlefield.SunkenBattleships.Count((battleship) => battleship.Size == pair.Key)) * orientations.Count(
									(orientation) => Battleship.GetTotalPosition(ownerBattlefield, coordinate, pair.Key, orientation).Count(
										(coordinate2) => !enemyBattlefield.AttackedCoordinates.ContainsKey(coordinate) || enemyBattlefield.AttackedCoordinates[coordinate] == AttackResult.Hit
									) == (int)pair.Key
								)
							)
						)
					)
				).SelectMany(
					(tuple) => tuple
				).OrderByDescending(
					(tuple) => tuple.Item2
				).FirstOrDefault();
				return coordinate;
			}
			if (!options.Any() && random.Next(0, 2) != 0 && Difficulty == AIDificulty.Normal)
			{
				//Pokus o nalezeni souradnic vedle trefenych mist
				options = enemyBattlefield.CoordinateMap.Select(
					(row) => row.Where(
						(coordinate) => !enemyBattlefield.AttackedCoordinates.ContainsKey(coordinate) && enemyBattlefield.AttackedCoordinates.Any((pair) => pair.Key.IsNeighborOf(coordinate))
					)
				).SelectMany(
					(coordinate) => coordinate
				);
			}
			if (!options.Any())
			{
				options = enemyBattlefield.CoordinateMap.Select(
					(row) => row.Where(
						(coordinate) => !enemyBattlefield.AttackedCoordinates.ContainsKey(coordinate)
					)
				).SelectMany(
					(coordinate) => coordinate
				);
			}
			//Nahodny vyber volne souradnice
			return options.ElementAtOrDefault(random.Next(0, options.Count()));
		}
	}
}