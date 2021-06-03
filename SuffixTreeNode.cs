﻿using System.Collections.Generic;

namespace Master
{
	public class SuffixTreeNode
	{
		public const int AlphabetSize = 2;
		public const char EndChar = (char)('A' + AlphabetSize);

		public readonly string Word;
		public readonly int Depth;
		public readonly int EdgeStart;
		public readonly int EdgeEnd;

		public readonly Dictionary<char, SuffixTreeNode> Children = new Dictionary<char, SuffixTreeNode>();
		public int EdgeLen => EdgeEnd - EdgeStart;
		public int Offset => EdgeEnd - Depth;
		public bool IsLeaf => EdgeEnd == Word.Length;

		public SuffixTreeNode(string word, List<string> suffixes, int previousDepth)
		{
			Word = word;

			if (suffixes.Count == 1)
				Depth = suffixes[0].Length;
			else
			{
				bool found = false;
				var depth = previousDepth + 1;

				while (true)
				{
					for (int j = 1; j < suffixes.Count; j++)
						if (suffixes[j][depth] != suffixes[0][depth])
						{
							found = true;
							break;
						}

					if (found)
						break;

					depth++;
				}

				Depth = depth;
			}

			var firstStart = word.Length - suffixes[0].Length;
			EdgeStart = firstStart + previousDepth + (previousDepth == -1 ? 1 : 0);
			EdgeEnd = firstStart + Depth;

			if (suffixes.Count == 1)
				return;

			var lists = new List<string>[AlphabetSize + 1];
			for (int i = 0; i < lists.Length; i++)
				lists[i] = new List<string>();

			foreach (var suff in suffixes)
			{
				int firstInd = suff[Depth] == EndChar ? AlphabetSize : suff[Depth] - 'A';
				lists[firstInd].Add(suff);
			}

			for (int i = 0; i < lists.Length; i++)
				if (lists[i].Count != 0)
				{
					var nextChar = i == AlphabetSize ? EndChar : (char)('A' + i);
					Children.Add(nextChar, new SuffixTreeNode(word, lists[i], Depth));
				}
		}

		public override string ToString() => Word.Substring(EdgeEnd - Depth, Depth);

		public static SuffixTreeNode Build(string s)
		{
			var suffixes = new List<string> { s };
			for (int i = 1; i < s.Length; i++)
				suffixes.Add(s[i..]);

			var root = new SuffixTreeNode(s, suffixes, -1);
			return root;
		}

		public List<SuffixTreeNode> Nodes
		{
			get
			{
				var stack = new Stack<(SuffixTreeNode, int)>();
				stack.Push((this, 0));

				var list = new List<SuffixTreeNode>();

				while (stack.Count != 0)
				{
					var (node, index) = stack.Pop();

					if (index == 0)
						list.Add(node);

					if (node.Children.Count == 0)
						continue;

					bool added = false;
					for (; index < AlphabetSize; index++)
					{
						var nextChar = (char)('A' + index);
						if (node.Children.TryGetValue(nextChar, out var child))
						{
							stack.Push((node, index + 1));
							stack.Push((child, 0));
							added = true;
							break;
						}
					}

					if (!added && node.Children.TryGetValue(EndChar, out var lastChild))
						stack.Push((lastChild, 0));
				}

				return list;
			}
		}
	}
}
