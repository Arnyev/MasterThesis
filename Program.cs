using System;
using System.Linq;

namespace Master
{
	class Program
	{
		static void Main(string[] args)
		{
			//var alphabet = new string(Range(0, SuffixTreeNode.AlphabetSize).Select(x => (char)(x + 'A')).ToArray());
			//TestLib.RunExhaustiveTests(26, alphabet, TestAll);
			//TestLib.RunRandomTests(9, 30, alphabet, 10000000, TestAll);
		}

		static void TestAll(string pattern, string text, SuffixTreeNode root)
		{
			WNodePrefix.Test(pattern, text, root);
			WEdgeSuffix.Test(pattern, text, root);
			WNodeSuffix.Test(pattern, text, root);
		}
	}
}
