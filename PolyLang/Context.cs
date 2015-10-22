using System;
using System.Collections.Generic;
using System.Text;

namespace PolyLang
{
	class Context
	{
		private Lexer lexer;

		public void Process()
		{
			Token token;
			while(lexer.ReadToken(out token))
				ProcessToken(token);
		}

		void ProcessToken(Token token)
		{
			
		}

	}
}
