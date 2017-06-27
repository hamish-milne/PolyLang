using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PolyLang
{
	public struct Token
	{
		public TokenType TokenType;
		public object Value;
	}

	public class Lexer
	{
		private readonly StreamReader reader;

		public Lexer(Stream stream)
		{
			reader = new StreamReader(stream, Encoding.UTF8);
		}

		private static TokenType CheckSingle(char c)
		{
			switch (c)
			{
				case '(': return TokenType.ParenOpen;
				case ')': return TokenType.ParenClose;
				case '{': return TokenType.BraceOpen;
				case '}': return TokenType.BraceClose;
				case '[': return TokenType.BracketOpen;
				case ']': return TokenType.BracketClose;
				case ',': return TokenType.Comma;
				case '.': return TokenType.Dot;
				case ':': return TokenType.Comma;
				case ';': return TokenType.Semicolon;
				case '\n': return TokenType.Newline;
				case '&': return TokenType.OpBAnd;
				case '|': return TokenType.OpBOr;
				case '~': return TokenType.OpBNot;
				case '^': return TokenType.OpBXor;
				case '+': return TokenType.OpAdd;
				case '-': return TokenType.OpSub;
				case '*': return TokenType.OpMul;
				case '/': return TokenType.OpDiv;
				case '%': return TokenType.OpMod;
				case '>':
				case '<':
				case '=':
				case '!':
					return TokenType.Unknown;
				default: return TokenType.None;
			}
		}

		private static TokenType CheckDouble(char c1, char? c2, out bool consume)
		{
			consume = false;
			switch (c1)
			{
				case '>':
					if (c2 == '=')
					{
						consume = true;
						return TokenType.OpGtEq;
					}
					return TokenType.OpGt;
				case '<':
					if (c2 == '=')
					{
						consume = true;
						return TokenType.OpLtEq;
					}
					return TokenType.OpLt;
				case '=':
					consume = true;
					switch (c2)
					{
						case '=': return TokenType.OpEq;
						case '>': return TokenType.Expr;
						default:
							consume = false;
							return TokenType.OpAssign;
					}
				case '!':
					if (c2 == '=')
					{
						consume = true;
						return TokenType.OpNeq;
					}
					return TokenType.Not;
				default:
					throw new Exception("Not an ambiguous character");
			}
		}

		private static TokenType MatchKeyword(string str)
		{
			switch (str)
			{
				case "var": return TokenType.Var;
				case "func": return TokenType.Func;
				case "if": return TokenType.If;
				case "then": return TokenType.Then;
				case "else": return TokenType.Else;
				case "for": return TokenType.For;
				case "while": return TokenType.While;
				case "do": return TokenType.Do;
				case "end": return TokenType.End;
				case "ref": return TokenType.Ref;
				case "interface": return TokenType.Interface;
				case "prop": return TokenType.Prop;
				case "import": return TokenType.Import;
				case "try": return TokenType.Try;
				case "catch": return TokenType.Catch;
				case "any": return TokenType.Any;
				case "true": return TokenType.LiteralTrue;
				case "false": return TokenType.LiteralFalse;
				case "null": return TokenType.LiteralNull;
				case "and": return TokenType.And;
				case "or": return TokenType.Or;
				case "not": return TokenType.Not;
				case "xor": return TokenType.Xor;
				default: return TokenType.Identifier;
			}
		}

		public Token Next()
		{
			// Skip whitespace; return EOF if needed
			var c = reader.Peek();
			if (c < 0) return new Token {TokenType = TokenType.Eof};
			while ((char) c != '\n' && Char.IsWhiteSpace((char) c))
			{
				reader.Read();
				c = reader.Peek();
				if (c < 0) return new Token { TokenType = TokenType.Eof };
			}

			// Comments
			if (c == '#')
			{
				reader.ReadLine();
				return new Token {TokenType = TokenType.Newline};
			}

			// Single or double character tokens
			var ttype = CheckSingle((char)c);
			if (ttype == TokenType.Unknown)
			{
				reader.Read();
				var c2 = reader.Peek();
				ttype = CheckDouble((char)c, c2 < 0 ? null : (char?) c2, out var consume);
				if (consume) reader.Read();
				return new Token {TokenType = ttype};
			}
			else if (ttype != TokenType.None) return new Token { TokenType = ttype };

			// Identifiers/keywords
			if (c == '_' || Char.IsLetter((char) c))
			{
				var sb = new StringBuilder();
				do
				{
					sb.Append((char) reader.Read());
					c = reader.Peek();
				} while (c >= 0 && (c == '_' || Char.IsLetter((char) c)));
				var str = sb.ToString();
				var tok = new Token {TokenType = MatchKeyword(str)};
				switch (tok.TokenType)
				{
					case TokenType.LiteralTrue:
						tok.Value = true;
						break;
					case TokenType.LiteralFalse:
						tok.Value = false;
						break;
					case TokenType.LiteralNull:
						tok.Value = null;
						break;
					case TokenType.Identifier:
						tok.Value = str;
						break;
				}
			}

			// Numbers
			if (Char.IsDigit((char) c))
			{
				// TODO: Binary and octal
				var sb = new StringBuilder();
				char prev;
				do
				{
					sb.Append(prev = (char) reader.Read());
					c = reader.Peek();
				} while (c >= 0 && (c == '.' || c == 'e' || Char.IsDigit((char) c) || (prev == 'e' && (c == '+' || c == '-'))));
				var str = sb.ToString();
				if (str.Any(d => d == '.' || d == 'e'))
				{
					var tok = new Token {TokenType = TokenType.LiteralFloat};
					if (Double.TryParse(str, out var d))
						tok.Value = d;
					return tok;
				}
				else
				{
					var tok = new Token {TokenType = TokenType.LiteralInt};
					if (Int64.TryParse(str, out var l))
						tok.Value = l;
					return tok;
				}
			}

			// Strings
			if (c == '\'' || c == '"')
			{
				var begin = (char) c;
				var sb = new StringBuilder();
				reader.Read();
				while (reader.Peek() >= 0 && reader.Peek() != begin)
				{
					sb.Append((char) reader.Peek());
					reader.Read();
				}
				var tok = new Token {TokenType = TokenType.LiteralString};
				if (reader.Read() == begin)
					tok.Value = sb.ToString();
				return tok;
			}

			return new Token {TokenType = TokenType.Unknown};
		}
	}
}
