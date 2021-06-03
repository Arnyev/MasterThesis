using System;

namespace Master
{
	public class KMP
	{
		public static int[] Borders(string pattern)
		{
			var borderLengths = new int[pattern.Length];
			int current = 0;

			for (int i = 1; i < pattern.Length; i++)
			{
				while (pattern[i] != pattern[current] && current != 0)
					current = borderLengths[current - 1];

				if (pattern[i] == pattern[current])
					current++;

				borderLengths[i] = current;
			}

			return borderLengths;
		}

		public static int[] Match(string pattern, int[] borders, string text)
		{
			if (text.Length == 0)
				return Array.Empty<int>();

			if (pattern.Length == 0)
				return new int[text.Length];

			int[] matchLengths = new int[text.Length];
			matchLengths[0] = text[0] == pattern[0] ? 1 : 0;

			for (int i = 1; i < text.Length; i++)
			{
				var matchLength = matchLengths[i - 1];
				if (matchLength == pattern.Length)
					matchLength = borders[^1];

				while (matchLength != 0 && text[i] != pattern[matchLength])
					matchLength = borders[matchLength - 1];

				if (text[i] == pattern[matchLength])
					matchLength++;

				matchLengths[i] = matchLength;
			}

			return matchLengths;
		}
	}
}
