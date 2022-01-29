using System;
using System.Collections.Generic;
using System.Linq;

namespace MyGraph
{
	/// <summary>
	/// This property always returns a value &lt; 1.
	/// </summary>

	static class NodeExtentions
	{
		public static IEnumerable<Node> BreadthSearch(this Node startNode)
		{
			var queue = new Queue<Node>();
			queue.Enqueue(startNode);
			var visited = new HashSet<Node>();
			while (queue.Count != 0)
			{
				var node = queue.Dequeue();
				if (visited.Contains(node)) continue;
				visited.Add(node);
				yield return node;
				foreach (var nextNode in node.IncidentNodes)
					queue.Enqueue(nextNode);
			}
		}

		public static IEnumerable<Node> DepthSearch(this Node startNode)
		{
			var stack = new Stack<Node>();
			stack.Push(startNode);
			var visited = new HashSet<Node>();
			while (stack.Count != 0)
			{
				var node = stack.Pop();
				if (visited.Contains(node)) continue;
				visited.Add(node);
				yield return node;
				foreach (var nextNode in node.IncidentNodes)
					stack.Push(nextNode);
			}
		}
	}

	class Program
	{

		static void Main()
		{

			var start = new Game(new[,] {
				{0, 2, 3, 4, 5},
			   {1, 6, 8, 9, 10},
			   {11, 7, 12, 14, 15},
			   {16, 17, 13, 18, 20},
			   {21, 22, 23, 19, 24}
		});

			var target = new Game(new[,]{
			   {1, 2, 3, 4, 5},
			   {6, 7, 8, 9, 10},
			   {11, 12, 13, 14, 15},
			   {16, 17, 18, 19, 20},
			   {21, 22, 23, 24, 0}
		});

			Dictionary<Game, Game> path = new Dictionary<Game, Game>();
			path[start] = null;
			var queue = new Queue<Game>();
			queue.Enqueue(start);
			while (queue.Count != 0)
			{
				var game = queue.Dequeue();

				var nextGames = game
					.AllAdjacentGames()
					.Where(g => !path.ContainsKey(g));
				foreach (var nextGame in nextGames)
				{
					path[nextGame] = game;
					queue.Enqueue(nextGame);
				}
				if (path.ContainsKey(target)) break;
			}
			var steps = new List<Game>();
			while (target != null)
			{
				steps.Add(target);
				target = path[target];
			}
			steps.Reverse();
			foreach (var step in steps)
				step.Print();
		}
	}
}

