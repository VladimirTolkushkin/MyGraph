using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MyGraph
{
	class Game
	{
		const int Size = 5;
		public int[,] Data { get; set; }
		public Game(int[,] data)
		{
			Data = data;
		}
		public Game(Game original)
			: this((int[,])original.Data.Clone())
		{

		}

		IEnumerable<Point> Rectangle(int xmin, int xmax, int ymin, int ymax)
		{
			for (int x = xmin; x <= xmax; x++)
				for (int y = ymin; y <= ymax; y++)
					yield return new Point { X = x, Y = y };
		}

		IEnumerable<Point> GamePoints
		{
			get
			{
				return Rectangle(0, Size - 1, 0, Size - 1);
			}
		}

		public Game Move(int dx, int dy)
		{
			var point = new Point();
			var found = GamePoints
				.Where(p => Data[p.X, p.Y] == 0)
				.Where(p => p.X + dx >= 0 && p.X + dx < Size && p.Y + dy >= 0 && p.Y + dy < Size)
				;
			if (found.Count() == 0) return null;
			else point = found.First();

			var newGame = new Game(this);
			newGame.Data[point.X, point.Y] = Data[point.X + dx, point.Y + dy];
			newGame.Data[point.X + dx, point.Y + dy] = Data[point.X, point.Y];
			return newGame;
		}

		public IEnumerable<Game> AllAdjacentGames()
		{
			return
				Rectangle(-1, 1, -1, 1)
				.Where(point => point.X == 0 || point.Y == 0)
				.Select(point => Move(point.X, point.Y))
				.Where(game => game != null);
		}

		public override bool Equals(object obj)
		{
			var game = obj as Game;
			return GamePoints
				.All(point => Data[point.X, point.Y] == game.Data[point.X, point.Y]);
		}

		public override int GetHashCode()
		{
			return GamePoints
				.Select(point => Data[point.X, point.Y])
				.Aggregate((sum, val) => sum * 97 + val);
		}

		public void Print()
		{
			var str = GamePoints
				.GroupBy(z => z.X)
				.Select(row => row.Select(point => Data[point.X, point.Y].ToString()).Aggregate((a, b) => a + " " + b))
				.Aggregate((a, b) => a + "\n" + b);
			Console.WriteLine(str);
			Console.WriteLine();
		}

	}
}
