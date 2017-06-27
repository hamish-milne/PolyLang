using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolyLang
{
	class PolyObject
	{
		public virtual bool OpCondition() => false;
	}

    class Context : PolyObject
    {
		public Field[] Fields { get; private set; } = new Field[0];

	    public virtual bool Checked => false;

	    public Context(Context parent)
	    {
		    Parent = parent;
	    }

		public Context Parent { get; }

	    public void AddField(Field field)
	    {
		    var f = Fields;
			Array.Resize(ref f, f.Length + 1);
		    f[f.Length - 1] = field;
		    Fields = f;
	    }

	    public Field GetFieldByName(string name)
	    {
			// TODO: Use a dictionary
		    return Fields.First(f => f.Name == name);
	    }
		
    }

	class CheckedContext : Context
	{
		public override bool Checked => true;

		public CheckedContext(Context parent) : base(parent) { }
	}

	class FieldAccessException : Exception
	{
		public FieldAccessException(string fieldName, Context obj, Context ctx) : base(
			$"The field {fieldName} in {obj} can not be accessed by {ctx}")
		{
		}
	}

	class Field
	{
		public string Name { get; }
		public Context Object { get; }

		public bool IsProperty { get; set; }
		public Access GetAccess { get; set; }
		public Access SetAccess { get; set; }
		public bool Sealed { get; private set; }

		public object Value { get; set; }

		public Field(string name, Context obj)
		{
			Name = name;
			Object = obj;
		}

		public bool CanAccess(Access access, Context context)
		{
			switch (access)
			{
				case Access.Public: return true;
				case Access.Private: return Object == context;
				case Access.Protected:
					var ctx = Object;
					do
					{
						if (ctx == context)
							return true;
						ctx = ctx.Parent;
					} while (ctx != null);
					return false;
				default:
					throw new InvalidOperationException();
			}
		}

		public static object AccessorRead(object accessor)
		{
			return null; // TODO
		}

		public static void AccessorWrite(object accessor, object value)
		{
			// TODO
		}

		public object Read(Context context)
		{
			if (!CanAccess(GetAccess, context)) throw new FieldAccessException(Name, Object, context);
			return IsProperty ? AccessorRead(Value) : Value;
		}

		public void Seal()
		{
			Sealed = true;
		}

		public void Write(Context context, object value)
		{
			if(Sealed || !CanAccess(SetAccess, context)) throw new FieldAccessException(Name, Object, context);
			if (IsProperty) AccessorWrite(Value, value);
			else Value = value;
		}
	}

	abstract class Expression
	{
		
	}

	abstract class Statement
	{
		
	}

	enum OpCode
	{
		/// <summary>
		/// Pops 'A' values from the stack
		/// </summary>
		Pop,

		/// <summary>
		/// Pushes the literal 'B' to the stack
		/// </summary>
		Push,

		/// <summary>
		/// Pushes a reference to the current scope's 'A'th parent
		/// </summary>
		This,

		/// <summary>
		/// Pops a value, checks its op_condition; If 'True', jump ahead 'A' instructions
		/// </summary>
		BTrue,

		/// <summary>
		/// Pops a value, checks its op_condition; if 'False', jump ahead 'A' instructions
		/// </summary>
		BFalse,

		/// <summary>
		/// Pushes a new context, whose parent is the current one
		/// </summary>
		ContextBegin,

		/// <summary>
		/// Pushes a new context, whose parent is popped from the stack
		/// </summary>
		ContextBeginDerived,

		/// <summary>
		/// Ends the current context, returning control to the parent and pushing the object to the stack
		/// </summary>
		ContextEnd,

		/// <summary>
		/// Pushes a checked context, using exception filter 'B'.
		/// </summary>
		Try,

		/// <summary>
		/// Ends the preceding 'try' block; begins a 'catch' block, writing the exception to local var ID 'A' with filter 'B'
		/// </summary>
		Catch,

		/// <summary>
		/// Pops a field reference and prevents it from being changed in future
		/// </summary>
		Seal,

		/// <summary>
		/// Pushes the value of field ID 'A' in this context to the stack
		/// </summary>
		LdVar,

		/// <summary>
		/// Pops the stack and sets the field ID 'A' in this context to its value
		/// </summary>
		StVar,

		/// <summary>
		/// Pops an object from the stack and pushes the value of field ID 'A'
		/// </summary>
		LdField,

		/// <summary>
		/// Pops an object and value from the stack and sets its field ID 'A'
		/// </summary>
		StField,

		/// <summary>
		/// Pops an object from the stack and pushes its field with name 'B'. Searches parent contexts.
		/// </summary>
		LdFieldName,

		/// <summary>
		/// Pops the object and value and stores the latter in the object's field named 'B'. Searches parent contexts.
		/// </summary>
		StFieldName,

		/// <summary>
		/// Pops the callable object and 'A'-count arguments from the stack, pushing the return values
		/// </summary>
		Call,

		/// <summary>
		/// Breaks out of all contexts until
		/// </summary>
		Return,

		/// <summary>
		/// Pops an object and pushes a reference to field ID 'A'
		/// </summary>
		Ref,

		/// <summary>
		/// Pops an object and pushes a reference to its field named 'B'
		/// </summary>
		RefName,

		/// <summary>
		/// Pops a reference object and pushes its value
		/// </summary>
		RefRead,

		/// <summary>
		/// Pops a reference object and another value and writes the latter to the former
		/// </summary>
		RefWrite,

		/// <summary>
		/// Pops an object and throws it as an exception
		/// </summary>
		Throw,

		/// <summary>
		/// Pops one operand, pushes the result of a unary operation determined by 'A'
		/// </summary>
		UnaryOp,

		/// <summary>
		/// Pops two operands, pushes the result of a binary operation determined by 'A'
		/// </summary>
		BinaryOp,

		/// <summary>
		/// Pops 'A' stack values and adds them to a new array object
		/// </summary>
		NewArray,

		/// <summary>
		/// Defines a local field with declaration object 'B'
		/// </summary>
		DefField,

		/// <summary>
		/// Defines a local field with declaration object 'B', pops the stack and uses the value as an accessor object
		/// </summary>
		DefProp,

		/// <summary>
		/// Pushes a function object that consists of the following 'A'-count instructions, which are skipped.
		/// </summary>
		DefFunc,
	}

	struct Instruction
	{
		public OpCode OpCode;
		public int OpA;
		public object OpB;
	}

	enum Access
	{
		Public,
		Protected,
		Private,
	}

	

	struct FieldDefinition
	{
		public string Name;
		public Access GetAccess;
		public Access SetAccess;
		public object StrongType;
	}
	
}
