using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PolyLang
{
	public class Lexer
	{
		#region Token definitions

		private static readonly Dictionary<string, TokenType> keywords
			= new Dictionary<string, TokenType>
			{
				{ "var", TokenType.Var },
				{ "import", TokenType.Import },
				{ "if", TokenType.If },
				{ "else", TokenType.Else },
				{ "while", TokenType.While },
				{ "for", TokenType.For },
				{ "do", TokenType.Do },
				{ "end", TokenType.End },
				{ "unpack", TokenType.Unpack },
				{ "func", TokenType.Func },
				{ "return", TokenType.Return },
				{ "ref", TokenType.Ref },
				{ "is", TokenType.Is },
				{ "as", TokenType.As },
				{ "in", TokenType.In },

				{ "get", TokenType.Get },
				{ "set", TokenType.Set },
				{ "public", TokenType.Public },
				{ "private", TokenType.Private },
				{ "protected", TokenType.Protected },
				{ "typeof", TokenType.Typeof },

				{ "and", TokenType.And },
				{ "or", TokenType.Or },
				{ "not", TokenType.Not },
				{ "xor", TokenType.Xor },
			};

		private static readonly Dictionary<string, TokenType> operators
			= new Dictionary<string, TokenType>
			{
				{ ";", TokenType.EndStatement },
				{ ":", TokenType.Colon },
				{ ",", TokenType.Comma },
				{ ".", TokenType.Dot },
				{ "=>", TokenType.Expr },
				{ "\"", TokenType.String },
				{ "'", TokenType.String },

				{ "+", TokenType.Add },
				{ "-", TokenType.Sub },
				{ "*", TokenType.Mul },
				{ "/", TokenType.Div },
				{ "%", TokenType.Mod },
				
				{ "==", TokenType.Eq },
				{ "!=", TokenType.Neq },
				{ ">", TokenType.Gt },
				{ "<", TokenType.Lt },
				{ ">=", TokenType.Geq },
				{ "<=", TokenType.Leq },

				{ "!", TokenType.Not },
				{ "&", TokenType.BitwiseAnd },
				{ "|", TokenType.BitwiseOr },
				{ "~", TokenType.BitwiseNot },
				{ "^", TokenType.BitwiseXor },

				{ "(", TokenType.ParenOpen },
				{ ")", TokenType.ParenClose },
				{ "{", TokenType.ObjectOpen },
				{ "}", TokenType.ObjectClose },
				{ "[", TokenType.ArrayOpen },
				{ "]", TokenType.ArrayClose },
			};

		struct CharToken
		{
			public TokenType TokenType;
			public char SecondChar;
		}

		private static readonly Dictionary<char, TokenType> singleChar
			= new Dictionary<char, TokenType>(); 
		private static readonly Dictionary<char, CharToken> twoChar
			= new Dictionary<char, CharToken>();

		static Lexer()
		{
			foreach (var pair in operators)
			{
				if(pair.Key.Length == 1)
					singleChar.Add(pair.Key[0], pair.Value);
				else if (pair.Key.Length == 2)
					twoChar.Add(pair.Key[0], new CharToken {SecondChar = pair.Key[1], TokenType = pair.Value});
				else
					Debug.Assert(false, "Invalid token definition");
			}
		}

		#endregion

		private readonly Stream stream;
		private readonly StreamReader reader;
		private readonly Stack<char> stack = new Stack<char>();
		private readonly StringBuilder sb = new StringBuilder();

		public Lexer(Stream stream)
		{
			this.stream = stream;
			reader = new StreamReader(stream);
		}

		bool SkipWhitespace()
		{
			char c;
			bool hasNewLine = false;
			while (char.IsWhiteSpace(c = PopChar()))
				hasNewLine = c == '\r' || c == '\n';
			PushChar(c);
			return hasNewLine;
		}

		char PopChar()
		{
			if (stack.Count > 0)
				return stack.Pop();
			return (char)reader.Read();
		}

		void PushChar(char c)
		{
			stack.Push(c);
		}

		bool IsIdentifier(char c, bool first)
		{
			return c == '_' || (first ? char.IsLetter(c) : char.IsLetterOrDigit(c));
		}

		public virtual bool ReadToken(out Token token)
		{
			token = new Token();
			var tokenType = TokenType.None;
			if (SkipWhitespace())
				tokenType = TokenType.EndLine;
			else
			{
				var c = PopChar();
				string str = null;
				double d = 0;
				ulong i = 0;
				CharToken charToken;
				if (twoChar.TryGetValue(c, out charToken))
				{
					var c2 = PopChar();
					if (c2 == charToken.SecondChar)
						tokenType = charToken.TokenType;
					else
						PushChar(c);
				}
				if (tokenType == TokenType.None)
					singleChar.TryGetValue(c, out tokenType);
				if (tokenType == TokenType.None)
				{
					sb.Remove(0, sb.Length);
					if (IsIdentifier(c, true))
					{
						do
						{
							sb.Append(c);
						} while (IsIdentifier(c = PopChar(), false));
						PushChar(c);
						str = sb.ToString();
						if (!keywords.TryGetValue(sb.ToString(), out tokenType))
							tokenType = TokenType.Identifier;
						else
							str = null;
					}
					else if (char.IsDigit(c))
					{
						sb.Append(c);
						bool isFloat = false;
						bool validChar;
						do
						{
							validChar = false;
							sb.Append(c = PopChar());
							if (c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
							{
								isFloat = true;
								validChar = true;
							}
						} while (validChar || char.IsDigit(c));
						PushChar(c);
						str = sb.ToString();
						if (isFloat)
							d = double.Parse(str);
						else
							i = ulong.Parse(str);
						tokenType = isFloat ? TokenType.Float : TokenType.Integer;
					}
				}
				if (tokenType == TokenType.String)
				{
					sb.Remove(0, sb.Length);
					char c2;
					while ((c2 = PopChar()) != c)
					{
						if (c2 == '\\')
						{
							c2 = PopChar();
							switch (c2)
							{
								case 'n':
									c2 = '\n';
									break;
								case 'r':
									c2 = '\r';
									break;
								case '0':
									c2 = '\0';
									break;
								case 't':
									c2 = '\t';
									break;
								case 'f':
									c2 = '\f';
									break;
								case '\\':
									c2 = '\\';
									break;
								case '"':
									c2 = '"';
									break;
							}
						}
						sb.Append(c2);
					}
					PushChar(c2);
					str = sb.ToString();
				}
				token.Float = d;
				token.Integer = i;
				token.String = str;
			}
			token.TokenType = tokenType;
			return true;
		}
	}
}
