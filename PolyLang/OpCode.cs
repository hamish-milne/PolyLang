namespace PolyLang
{
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
		/// Ends the current context (ID 'A') with no push
		/// </summary>
		BlockEnd,

		ConditionBegin,

		ConditionElse,

		IterateBegin,

		LoopBegin,

		DoWhileBegin,

		DoWhileEnd,

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
}