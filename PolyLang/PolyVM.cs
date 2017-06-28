using System;
using System.Collections.Generic;

namespace PolyLang
{
	enum UnaryOp
	{
		Plus,
		Minus,
		BitwiseNot,
		BooleanNot,
	}

	enum BinaryOp
	{
		Equal,
		NotEqual,
		Greater,
		Less,
		GreaterEqual,
		LessEqual,
		BitwiseAnd,
		BitwiseOr,
		BitwiseNot,
		BitwiseXor,
		Add,
		Sub,
		Mul,
		Div,
		Mod,
		BooleanAnd,
		BooleanOr,
		BooleanXor,
	}

	class PolyException : Exception
	{
		public PolyException(object e) : base(e.ToString())
		{
		}
	}

	static class BoxCache
	{
		private static readonly object NaN = double.NaN;
		private static readonly object IntZero = (long) 0;
		private static readonly object FloatZero = 0.0;
		private static readonly object True = true;
		private static readonly object False = false;

		public static object Box(bool b)
		{
			return b ? True : False;
		}

		public static object Box(long i)
		{
			return i == 0 ? IntZero : i;
		}

		public static object Box(double d)
		{
			return double.IsNaN(d) ? NaN : (d == 0.0 ? FloatZero : d);
		}
	}

	class ReturnPoint
	{
		public Context Context { get; }
		public int Address { get; }

		public ReturnPoint(int address, Context context)
		{
			Address = address;
			Context = context;
		}
	}

	class Program
	{
		private readonly List<Instruction> instructions = new List<Instruction>();

		public virtual void AddInstruction(Instruction inst)
		{
			instructions.Add(inst);
		}

		public virtual Instruction GetInstruction(int pc)
		{
			return instructions[pc];
		}
	}

    class PolyVM
    {
	    private readonly Instruction[] instructions;
		private readonly Stack<object> stack = new Stack<object>();

		public Context Context { get; set; }
		public int ProgramCounter { get; set; }
		

	    public static bool OpCondition(object obj)
	    {
		    switch (obj)
			{
				case bool b:
					return b;
				case PolyObject po:
					return po.OpCondition();
				case null:
					return false;
				case long i:
					return i != 0;
				case double d:
					return !double.IsNaN(d) && d != 0.0;
				case string s:
					return s != "";
				default:
					throw new ArgumentException("Invalid stack object!");
		    }
	    }

	    public void Step()
	    {
		    try
		    {
			    StepInternal();
		    }
		    catch (Exception e)
		    {
			    HandleException(e);
		    }
	    }

	    private void HandleException(Exception e)
	    {
		    
	    }

	    private void Call(Function func, int numArgs)
	    {
		    var args = new Stack<object>();
			for(int i = 0; i < numArgs; i++)
				args.Push(stack.Pop());
			stack.Push(new ReturnPoint(ProgramCounter, Context));
		    Context = func.Parent;
		    ProgramCounter = func.Address;
			while(args.Count > 0)
				stack.Push(args.Pop());
	    }

	    private void StepInternal()
	    {
		    var instr = instructions[++ProgramCounter];

		    switch (instr.OpCode)
		    {
			    case OpCode.Pop:
					while(instr.OpA-- > 0)
						stack.Pop();
				    break;
			    case OpCode.Push:
					stack.Push(instr.OpB);
				    break;
			    case OpCode.This:
				    var ctx = Context;
				    while (--instr.OpA >= 0)
					    ctx = Context.Parent;
					stack.Push(ctx);
				    break;
			    case OpCode.ContextBegin:
					Context = new Context(Context);
				    break;
			    case OpCode.ContextEnd:
				    stack.Push(Context);
				    Context = Context.Parent;
				    break;
				case OpCode.BlockEnd:
					Context = Context.Parent;
					break;
			    case OpCode.Try:
				    Context = new CheckedContext(Context);
				    break;
			    case OpCode.Catch:
				    break;
			    case OpCode.Seal:
					Context.Fields[instr.OpA].Seal();
				    break;
			    case OpCode.LdVar:
					stack.Push(Context.Fields[instr.OpA].Value);
				    break;
			    case OpCode.StVar:
				    Context.Fields[instr.OpA].Write(Context, stack.Pop());
				    break;
			    case OpCode.LdField:
				    var ctx1 = (Context) stack.Pop();
				    stack.Push(ctx1.Fields[instr.OpA].Read(Context));
				    break;
			    case OpCode.StField:
				    var ctx2 = (Context)stack.Pop();
				    ctx2.Fields[instr.OpA].Write(Context, stack.Pop());
					break;
			    case OpCode.LdFieldName:
				    Context.GetFieldByName(instr.OpB.ToString()).Read(Context);
				    break;
			    case OpCode.StFieldName:
					Context.GetFieldByName(instr.OpB.ToString()).Write(Context, stack.Pop());
				    break;
			    case OpCode.Call:
				    break;
			    case OpCode.Ref:
				    var ctx3 = (Context) stack.Pop();
					stack.Push(ctx3.Fields[instr.OpA]);
				    break;
			    case OpCode.RefName:
				    var ctx4 = (Context) stack.Pop();
					stack.Push(ctx4.GetFieldByName(instr.OpB.ToString()));
				    break;
			    case OpCode.RefRead:
					stack.Push(((Field)stack.Pop()).Read(Context));
				    break;
			    case OpCode.RefWrite:
				    var fld = (Field) stack.Pop();
					fld.Write(Context, stack.Pop());
				    break;
			    case OpCode.Throw:
					throw new PolyException(stack.Pop());
			    case OpCode.UnaryOp:
				    break;
			    case OpCode.BinaryOp:
				    break;
			    case OpCode.NewArray:
				    break;
			    case OpCode.DefField:
				    break;
			    case OpCode.DefProp:
				    break;
			    case OpCode.DefFunc:
				    break;
			    case OpCode.ContextBeginDerived:
					Context = new Context((Context)stack.Pop());
				    break;
				case OpCode.LoopBegin:
					break;
				case OpCode.LoopEnd:
					Instruction i3;
					do
					{
						i3 = instructions[--ProgramCounter];
					} while (!((i3.OpCode == OpCode.LoopBegin || i3.OpCode == OpCode.ConditionForward) && i3.OpA == instr.OpA));
					break;
			    case OpCode.ConditionForward:
				    if (!OpCondition(stack.Pop()))
				    {
						Instruction i1;
					    do
					    {
						    i1 = instructions[++ProgramCounter];
					    } while (!((i1.OpCode == OpCode.ConditionElse || i1.OpCode == OpCode.BlockEnd || i1.OpCode == OpCode.LoopEnd) && i1.OpA == instr.OpA));
				    }
				    break;
			    case OpCode.ConditionBackward:
				    if (OpCondition(stack.Pop()))
				    {
					    Instruction i1;
					    do
					    {
						    i1 = instructions[--ProgramCounter];
					    } while (!((i1.OpCode == OpCode.LoopBegin) && i1.OpA == instr.OpA));
				    }
				    break;
				case OpCode.ConditionElse:
				    Instruction i2;
				    do
				    {
					    i2 = instructions[++ProgramCounter];
				    } while (!(i2.OpCode == OpCode.BlockEnd && i2.OpA == instr.OpA));
					break;
			    case OpCode.IterateBegin:
				    break;
			    case OpCode.Return:
				    var firstValue = stack.Pop();
				    if (firstValue is ReturnPoint r)
				    {
					    stack.Push(null);
					    ProgramCounter = r.Address;
					    Context = r.Context;
				    }
				    else
				    {
					    var secondValue = (ReturnPoint) stack.Pop();
						stack.Push(firstValue);
					    ProgramCounter = secondValue.Address;
					    Context = secondValue.Context;
				    }
				    break;
			    default:
				    throw new ArgumentOutOfRangeException();
		    }
	    }
    }
}
