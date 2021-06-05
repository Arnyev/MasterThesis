using System;
using System.Linq;

namespace Master
{
	class Program
	{
		static void Main(string[] args)
		{
			//TestLib.RunExhaustiveTests(26, TestAll);
			TestLib.RunRandomTests(9, 300, 10000000, TestAll);
		}

		static void TestAll(string pattern, string text, Node root)
		{
			//WNodePrefix.Test(pattern, text, root);
			//WEdgeSuffix.Test(pattern, text, root);
			WNodeSuffix.Test(pattern, text, root);
		}
	}
}
