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
