using System;

namespace Master
{
	public class Lib
	{
		public static int[] GetNextMatch(int[] match, int patternLength)
		{
			var next = new int[match.Length];
			var nextMatch = -1;
			for (int i = match.Length - 1; i >= 0; i--)
			{
				if (match[i] == patternLength)
					nextMatch = i;

				next[i] = nextMatch;
			}

			return next;
		}

		public static int[] GetPreviousMatch(int[] match, int patternLength)
		{
			var previous = new int[match.Length];
			var previousMatch = -1;
			for (int i = 0; i < match.Length; i++)
			{
				if (match[i] == patternLength)
					previousMatch = i;

				previous[i] = previousMatch;
			}

			return previous;
		}

		public static string GetRandomString(int length, string alphabet, Random random)
		{
			var array = new char[length];

			for (int j = 0; j < array.Length; j++)
				array[j] = alphabet[random.Next(alphabet.Length)];

			return new string(array);
		}

		/// <summary>
		/// Assumes that test is true for min -1 and false for max.
		/// </summary>
		public static int UpperBound(int min, int max, Predicate<int> test)
		{
			while (min < max)
			{
				int mid = (min + max) / 2;
				if (test(mid))
					min = mid + 1;
				else
					max = mid;
			}

			return min - 1;
		}

		/// <summary>
		/// Assumes that test is true for max and false for min.
		/// </summary>
		public static int LowerBound(int min, int max, Predicate<int> test)
		{
			while (min < max)
			{
				int mid = (min + max) / 2;
				if (test(mid))
					max = mid;
				else
					min = mid + 1;
			}

			return max;
		}
	}
}
