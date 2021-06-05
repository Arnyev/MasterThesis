using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
	class WNodePrefix
	{
		public static void Test(string pattern, string text, Node root)
		{
			var pairs = GetPairs(pattern, text, root);
			var bruteSol = root.Nodes.Select(x => x.ToString()).Where(x => text.Contains(pattern + x)).ToList();
			var sol = pairs.Select(x => x.Item1.ToString()).OrderBy(x => x).ToList();

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
			var output = new List<(Node, Node)>();
			if (!text.Contains(pattern))
				return output;

			if (!pattern.StartsWith(root.ToString()))
				return output;

			var prefixRoot = root;
			var expected = pattern + root.ToString();

			while (prefixRoot.Depth < expected.Length)
			{
				var nextChar = expected[prefixRoot.Depth];
				if (!prefixRoot.Children.TryGetValue(nextChar, out var next))
					return output;

				var nextStr = next.ToString();
				if (nextStr.Length > expected.Length)
					nextStr = nextStr.Substring(0, pattern.Length);

				if (!expected.StartsWith(nextStr))
					return output;

				prefixRoot = next;
			}

			SearchForPairs(root, prefixRoot, pattern, text, output);

			return output;
		}

		public static void SearchForPairs(Node node, Node prefixNode, string pattern, string text, List<(Node, Node)> output)
		{
			while (node.Depth < prefixNode.Depth - pattern.Length)
			{
				output.Add((node, prefixNode));
				var depthDiff = prefixNode.Depth - pattern.Length - node.Depth;
				var nextChar = text[prefixNode.EdgeEnd - depthDiff];
				node = node.Children[nextChar];
			}

			output.Add((node, prefixNode));
			foreach (var prefixChild in prefixNode.Children)
			{
				var nodeChild = node.Children[prefixChild.Key];
				SearchForPairs(nodeChild, prefixChild.Value, pattern, text, output);
			}
		}
	}
}
