using System;
using System.Collections.Generic;
using System.Linq;
using static System.Linq.Enumerable;

namespace Master
{
	public class WNodeSuffix
	{
		public static void Test(string pattern, string text, Node root)
		{
			var pairs = GetPairs(pattern, text, root);
			var sol = pairs.Select(x => x.Item1).OrderBy(x => x.ToString()).ToList();
			var bruteSol = root.Nodes.Where(x => text.Contains(x.ToString() + pattern)).ToList();

			var passed = sol.SequenceEqual(bruteSol);

			if (!passed)
			{
				var tooMany = sol.Where(x => !bruteSol.Contains(x)).ToList();
				var missing = bruteSol.Where(x => !sol.Contains(x)).ToList();

				Console.WriteLine(pattern);
				Console.WriteLine(text);
				Console.WriteLine();
			}
		}

		public static List<(Node, Node)> GetPairs(string pattern, string text, Node root)
		{
			var borders = KMP.Borders(pattern);

			var patternPeriod = pattern.Length - borders[^1];

			var matchW = KMP.Match(pattern, borders, text);
			var nextMatches = Lib.GetNextMatch(matchW, pattern.Length);
			var previousMatches = Lib.GetPreviousMatch(matchW, pattern.Length);

			var buffer = new int[text.Length + 2];
			Array.Fill(buffer, int.MaxValue);
			var nodesInStack = new Node[text.Length + 1];

			List<(Node, Node)> output = new List<(Node, Node)>();

			SearchForPairs(root, matchW, nextMatches, previousMatches, buffer, 0, 0, patternPeriod, nodesInStack, pattern.Length, output);

			return output;
		}

		static void SearchForPairs(Node node, int[] matchW, int[] next, int[] previous, int[] buffer, int start, int end,
			int period, Node[] stack, int wlen, List<(Node, Node)> output)
		{
			start = OutputPairs(node, next, previous, buffer, start, stack, wlen, output);

			var match = node.EdgeEnd == 0 ? 0 : matchW[node.EdgeEnd - 1];

			if (match < period + node.EdgeLen)
				start = end;

			var shouldAdd = match % period == 0 && match >= period;
			if (shouldAdd)// the node is along the match 
			{
				buffer[end] = node.Depth;
				end++;
			}

			stack[node.Depth] = node;
			foreach (var child in node.Children)
				SearchForPairs(child.Value, matchW, next, previous, buffer, start, end, period, stack, wlen, output);

			stack[node.Depth] = null;

			if (shouldAdd)
				buffer[end - 1] = int.MaxValue;
		}

		private static int OutputPairs(Node node, int[] next, int[] previous, int[] buffer, int start, Node[] stack, int wlen, List<(Node, Node)> output)
		{
			if (node.Depth < wlen) // no possible match
				return start;

			var offset = node.Offset;
			var firstIndex = Math.Max(node.EdgeStart, offset + wlen - 1);
			var firstOcc = next[firstIndex];
			var lastIndex = Math.Min(node.EdgeStart + wlen - 1, node.EdgeEnd - 1);
			var lastOcc = previous[lastIndex];

			int firstDepth = firstOcc + 1 - wlen - offset;
			int lastDepth = lastOcc + 1 - wlen - offset;

			if (firstDepth < 0 || firstDepth > lastDepth) // no match
				return start;

			if (buffer[start] == firstDepth)
				start++;

			if (stack[firstDepth] != null)
				output.Add((stack[firstDepth], node));

			if (firstOcc == lastOcc) // only 1 match
				return start;

			if (stack[lastDepth] != null)
				output.Add((stack[lastDepth], node));

			if (buffer[start] == lastDepth)
				start++;

			if (next[firstOcc + 1] == lastOcc) // only 2 matches
				return start;

			for (; buffer[start] < lastDepth; start++)
				output.Add((stack[buffer[start]], node));

			if (buffer[start] == lastDepth)
				start++;

			return start;
		}
	}
}
