using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGraph
{

	public static class ExtentionsGraph
	{
		public static IEnumerable<Node> BreadthSearch(this Graph graph, Node startNode)
		{
			var visited = new HashSet<Node>();
			var queue = new Queue<Node>();
			visited.Add(startNode);
			queue.Enqueue(startNode);
			while (queue.Count != 0)
			{
				var node = queue.Dequeue();
				yield return node;
				foreach (var nextNode in node.IncidentNodes.Where(n => !visited.Contains(n)))
				{
					visited.Add(nextNode);
					queue.Enqueue(nextNode);
				}
			}
		}

		public static IEnumerable<Node> DepthSearch(this Graph graph, Node startNode)
		{
			var visited = new HashSet<Node>();
			var stack = new Stack<Node>();
			visited.Add(startNode);
			stack.Push(startNode);
			while (stack.Count != 0)
			{
				var node = stack.Pop();
				yield return node;
				foreach (var nextNode in node.IncidentNodes.Where(n => !visited.Contains(n)))
				{
					visited.Add(nextNode);
					stack.Push(nextNode);
				}
			}
		}

		public static IEnumerable<IEnumerable<Node>> FindConnectedComponents(this Graph graph)
		{
			var result = new List<List<Node>>();
			var markedNodes = new HashSet<Node>();
			while (true)
			{
				var nextNode = graph.Nodes.Where(node => !markedNodes.Contains(node)).FirstOrDefault();
				if (nextNode == null) break;
				var breadthSearch = nextNode.BreadthSearch().ToList(); ;
				result.Add(breadthSearch);
				foreach (var node in breadthSearch)
					markedNodes.Add(node);
			}
			return result;
		}

		
	}


	public class Edge
	{
		public readonly Node From;
		public readonly Node To;
		public Edge(Node first, Node second)
		{
			this.From = first;
			this.To = second;
		}
		public bool IsIncident(Node node)
		{
			return From == node || To == node;
		}
		public Node OtherNode(Node node)
		{
			if (!IsIncident(node)) throw new ArgumentException();
			if (From == node) return To;
			return From;
		}
	}

	public class Node
	{
		readonly List<Edge> edges = new List<Edge>();
		public readonly int NodeNumber;

		public Node(int number)
		{
			NodeNumber = number;
		}

		public IEnumerable<Node> IncidentNodes
		{
			get
			{
				return edges.Select(z => z.OtherNode(this));
			}
		}
		public IEnumerable<Edge> IncidentEdges
		{
			get
			{
				foreach (var e in edges) yield return e;
			}
		}
		public static Edge Connect(Node node1, Node node2, Graph graph)
		{
			if (!graph.Nodes.Contains(node1) || !graph.Nodes.Contains(node2)) throw new ArgumentException();
			var edge = new Edge(node1, node2);
			node1.edges.Add(edge);
			node2.edges.Add(edge);
			return edge;
		}
		public static void Disconnect(Edge edge)
		{
			edge.From.edges.Remove(edge);
			edge.To.edges.Remove(edge);
		}
	}

	public class Graph
	{
		private Node[] nodes;

		public Graph(int nodesCount)
		{
			nodes = Enumerable.Range(0, nodesCount).Select(z => new Node(z)).ToArray();
		}

		public int Length { get { return nodes.Length; } }

		public Node this[int index] { get { return nodes[index]; } }

		public IEnumerable<Node> Nodes
		{
			get
			{
				foreach (var node in nodes) yield return node;
			}
		}

		public void Connect(int index1, int index2)
		{
			Node.Connect(nodes[index1], nodes[index2], this);
		}

		public void Delete(Edge edge)
		{
			Node.Disconnect(edge);
		}

		public IEnumerable<Edge> Edges
		{
			get
			{
				return nodes.SelectMany(z => z.IncidentEdges).Distinct();
			}
		}

		public static Graph MakeGraph(params int[] incidentNodes)
		{
			var graph = new Graph(incidentNodes.Max() + 1);
			for (int i = 0; i < incidentNodes.Length - 1; i += 2)
				graph.Connect(incidentNodes[i], incidentNodes[i + 1]);
			return graph;
		}

		public static List<Node> FindPath(Node start, Node end)
		{
			var track = new Dictionary<Node, Node>();
			track[start] = null;
			var queue = new Queue<Node>();
			queue.Enqueue(start);
			while (queue.Count != 0)
			{
				var node = queue.Dequeue();
				foreach (var nextNode in node.IncidentNodes)
				{
					if (track.ContainsKey(nextNode)) continue;
					track[nextNode] = node;
					queue.Enqueue(nextNode);
				}
				if (track.ContainsKey(end)) break;
			}
			var pathItem = end;
			var result = new List<Node>();
			while (pathItem != null)
			{
				result.Add(pathItem);
				pathItem = track[pathItem];
			}
			result.Reverse();
			return result;
		}
	}
}
