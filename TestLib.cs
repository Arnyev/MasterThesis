using System;

namespace Master
{
	public class TestLib
	{
		public static void RunExhaustiveTests(int totalLength, string alphabet, Action<string, string, SuffixTreeNode> test)
		{
			Console.WriteLine("Exhaustive tests start");

			for (int len = 5; len <= totalLength; len++)
			{
				Console.WriteLine($"Length {len} started.");
				var pow = (long)Math.Pow(alphabet.Length, len);
				var chars = new char[len];

				for (int i = 0; i < pow; i++)
				{
					var word = i;
					for (int ind = 0; ind < len; ind++)
					{
						chars[ind] = alphabet[word % alphabet.Length];
						word /= alphabet.Length;
					}

					for (int patternLength = 1; patternLength < len / 2; patternLength++)
					{
						var pattern = new string(chars.AsSpan(0, patternLength));
						var text = new string(chars.AsSpan(patternLength)) + SuffixTreeNode.EndChar;

						if (!text.Contains(pattern))
							continue;

						var root = SuffixTreeNode.Build(text);
						test(pattern, text, root);
					}
				}
			}

			Console.WriteLine("Exhaustive tests end");
		}

		public static void RunRandomTests(int patternLength, int textLength, string alphabet, int testCount, Action<string, string, SuffixTreeNode> test)
		{
			var random = new Random();

			Console.WriteLine("Random tests start");
			var hundredth = testCount / 100;

			for (int i = 0; i < testCount; i++)
			{
				var pattern = Lib.GetRandomString(patternLength, alphabet, random);
				var text = Lib.GetRandomString(textLength, alphabet, random) + SuffixTreeNode.EndChar;

				if (!text.Contains(pattern))
					continue;

				var root = SuffixTreeNode.Build(text);
				test(pattern, text, root);

				if (i % hundredth == hundredth - 1)
					Console.WriteLine($"{i / hundredth + 1}% of tests done.");
			}

			Console.WriteLine("Random tests end");
		}
	}
}
