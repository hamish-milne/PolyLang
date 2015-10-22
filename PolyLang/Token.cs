using System;
using System.Collections.Generic;
using System.Text;

namespace PolyLang
{
	public struct Token
	{
		public TokenType TokenType;
		public string String;
		public ulong Integer;
		public double Float;
	}
}
