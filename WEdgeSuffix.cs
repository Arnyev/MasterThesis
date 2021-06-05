using System;
using System.Collections.Generic;
using System.Linq;
using static System.Linq.Enumerable;

namespace Master
{
	public class WEdgeSuffix
	{
		public struct Match
		{
			public readonly Node Ancestor;
			public readonly Node Descendant;
			public readonly int StartDepth;
			public readonly int Count;

			public Match(Node ancestor, Node descendant, int startDepth, int periods)
			{
				Ancestor = ancestor;
				Descendant = descendant;
				StartDepth = startDepth;
				Count = periods;
			}

			public override string ToString() => Ancestor.ToString().Substring(0, StartDepth);
		}

		public static void Test(string pattern, string text, Node root)
		{
			var borders = KMP.Borders(pattern);
			var patternPeriod = pattern.Length - borders[^1];

			var matches = GetMatches(pattern, text, root);
			var bruteSol = GetMatchesBrute(pattern, text, root);

			var n = text.Length;

			var sol = matches.SelectMany(m => Range(0, m.Count)
					.Select(p => m.Ancestor.ToString().Substring(0, m.StartDepth + p * patternPeriod)))
					.OrderBy(x => x).ToArray();

			var ok = sol.SequenceEqual(bruteSol);

			if (!ok)
			{
				var tooMany = sol.Where(x => !bruteSol.Contains(x)).ToList();
				var missing = bruteSol.Where(x => !sol.Contains(x)).ToList();
				Console.WriteLine(text);
				Console.WriteLine(pattern);
				Console.WriteLine();
			}
		}

		private static List<string> GetMatchesBrute(string pattern, string text, Node trie)
		{
			var trieNodeStrings = trie.Nodes.Select(x => x.ToString()).ToList();
			var allWithSuffixHs = new HashSet<string>();
			var ind = 0;
			var bruteSol = new List<string>();

			while (ind < text.Length)
			{
				var next = text.IndexOf(pattern, ind);
				if (next == -1)
					break;

				ind = next + 1;

				for (int i = 0; i <= next; i++)
				{
					var substring = text.Substring(i, next - i);
					if (!allWithSuffixHs.Add(substring))
						continue;

					var substrPattern = substring + pattern;

					for (int j = 0; j < trieNodeStrings.Count; j++)
					{
						var trieString = trieNodeStrings[j];
						if (trieString == substring)
						{
							bruteSol.Add(substring);
							break;
						}

						if (substrPattern != trieString && trieString.StartsWith(substring) && substrPattern.StartsWith(trieString))
						{
							bruteSol.Add(substring);
							break;
						}
					}
				}
			}

			bruteSol.Sort();
			return bruteSol;
		}

		public static List<Match> GetMatches(string pattern, string text, Node root)
		{
			var borders = KMP.Borders(pattern);
			var matchLengths = KMP.Match(pattern, borders, text);
			var nextMatches = Lib.GetNextMatch(matchLengths, pattern.Length);
			var previousMatches = Lib.GetPreviousMatch(matchLengths, pattern.Length);
			var patternPeriod = pattern.Length - borders[^1];
			var nodesInStack = new Node[text.Length + 2];

			var output = new List<Match>();
			SearchForMatches(root, root, nextMatches, previousMatches, patternPeriod, nodesInStack, pattern.Length, output);

			return output;
		}

		static void SearchForMatches(Node node, Node root, int[] next, int[] previous, int period, Node[] stack, int wlen, List<Match> output)
		{
			// the node's edge just corresponds to the end of suffix special character, which doesn't exist in the original string
			if (node.EdgeStart == node.Word.Length - 1)
				return;

			if (node != root)
				OutputMatches(node, next, previous, stack, wlen, period, output);

			stack[node.Depth] = node;

			foreach (var child in node.Children)
				SearchForMatches(child.Value, root, next, previous, period, stack, wlen, output);

			stack[node.Depth] = null;
		}

		private static void OutputMatches(Node node, int[] next, int[] previous,
			Node[] stack, int wlen, int period, List<Match> output)
		{
			if (node.Depth < wlen) // no possible match
				return;

			var offset = node.Offset;
			var firstIndex = Math.Max(node.EdgeStart, offset + wlen - 1);
			var firstOcc = next[firstIndex];
			var lastIndex = Math.Min(node.EdgeStart + wlen - 1, node.EdgeEnd - 1);
			var lastOcc = previous[lastIndex];

			int firstDepth = firstOcc + 1 - wlen - offset;
			int lastDepth = lastOcc + 1 - wlen - offset;

			if (firstDepth < 0 || firstDepth > lastDepth) // no match
				return;

			output.Add(new Match(Successor(firstDepth, stack), node, firstDepth, 1));

			if (firstOcc == lastOcc) // only 1 match
				return;

			output.Add(new Match(Successor(lastDepth, stack), node, lastDepth, 1));

			if (next[firstOcc + 1] == lastOcc) // only 2 matches
				return;

			var secondDepth = firstDepth + period;
			var matchCount = (lastDepth - secondDepth) / period;

			for (int i = 0; i < matchCount; i++)
			{
				var depth = secondDepth + i * period;
				var ancestor = Successor(depth, stack);
				var remainingLength = ancestor.Depth - depth;
				var repeats = Math.Min(remainingLength / period, matchCount - i - 1);
				output.Add(new Match(ancestor, node, depth, repeats + 1));
				i += repeats; // at this point i is at the last match of matchNode
			}
		}

		private static Node Successor(int depth, Node[] nodesInStack)
		{
			for (int i = depth; i < nodesInStack.Length; i++)
				if (nodesInStack[i] != null)
					return nodesInStack[i];

			return null;
		}
	}
}
