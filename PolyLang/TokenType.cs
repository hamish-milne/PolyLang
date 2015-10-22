
namespace PolyLang
{
	public enum TokenType
	{
		None,
		Identifier,
		Integer,
		Float,
		String,

		Var,
		Import,
		If,
		Else,
		While,
		For,
		Do,
		End,
		Unpack,
		Func,
		Return,
		Ref,
		Is,
		As,

		EndLine,
		EndStatement,
		Colon,
		Comma,
		Dot,
		Expr,
		In,

		Add,
		Sub,
		Mul,
		Div,
		Mod,

		Eq,
		Neq,
		Gt,
		Lt,
		Geq,
		Leq,

		And,
		Or,
		Not,
		Xor,
		BitwiseAnd,
		BitwiseOr,
		BitwiseNot,
		BitwiseXor,

		ParenOpen,
		ParenClose,
		ObjectOpen,
		ObjectClose,
		ArrayOpen,
		ArrayClose,

		Get,
		Set,
		Public,
		Private,
		Protected,
		Typeof,
	}
}
