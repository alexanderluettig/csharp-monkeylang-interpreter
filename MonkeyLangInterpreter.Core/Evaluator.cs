using MonkeyLangInterpreter.Core.Interfaces;
using MonkeyLangInterpreter.Core.Nodes;
using MonkeyLangInterpreter.Core.Objects;

namespace MonkeyLangInterpreter.Core;

public class Evaluator
{
    private static readonly BooleanObject TRUE = new(true);
    private static readonly BooleanObject FALSE = new(false);
    private static readonly NullObject NULL = new();
    private static Dictionary<string, Builtin> BUILTINS = new()
    {
        ["len"] = new Builtin(BuiltinFunctions.Len),
    };
    public static IObject Eval(INode node, VariableEnvironment environment)
    {
        switch (node)
        {
            case Program program:
                return EvalProgram(program, environment);
            case PrefixExpression prefixExpression:
                var expression = Eval(prefixExpression.Right, environment);
                if (IsError(expression))
                {
                    return expression;
                }
                return EvalPrefixExpression(prefixExpression.Operator, expression);
            case InfixExpression infixExpression:
                var leftSide = Eval(infixExpression.Left, environment);
                if (IsError(leftSide))
                {
                    return leftSide;
                }
                var rightSide = Eval(infixExpression.Right, environment);
                if (IsError(rightSide))
                {
                    return rightSide;
                }
                return EvalInfixExpression(infixExpression.Operator, leftSide, rightSide);
            case BlockStatement blockStatement:
                return EvalBlockStatements(blockStatement, environment);
            case ReturnStatement returnStatement:
                var value = Eval(returnStatement.ReturnValue, environment);
                if (IsError(value))
                {
                    return value;
                }
                return new ReturnValue(value);
            case LetStatement letStatement:
                var evaluated = Eval(letStatement.Value, environment);
                if (IsError(evaluated))
                {
                    return evaluated;
                }
                environment.Set(letStatement.Name.Value, evaluated);
                return evaluated;
            case Identifier identifier:
                return EvalIdentifier(identifier, environment);
            case IfExpression ifExpression:
                return EvalIfExpression(ifExpression, environment);
            case ExpressionStatement expressionStatement:
                return Eval(expressionStatement.Expression, environment);
            case IntegerLiteral integerLiteral:
                return new IntegerObject(integerLiteral.Value);
            case StringLiteral stringLiteral:
                return new StringObject(stringLiteral.Value);
            case BooleanExpression booleanExpression:
                return booleanExpression.Value ? TRUE : FALSE;
            case FunctionLiteral functionLiteral:
                return new Function(functionLiteral.Parameters, functionLiteral.Body, environment);
            case CallExpression callExpression:
                var function = Eval(callExpression.Function, environment);
                if (IsError(function))
                {
                    return function;
                }
                var args = EvalExpressions(callExpression.Arguments, environment);
                if (args.Count == 1 && IsError(args[0]))
                {
                    return args[0];
                }
                return ApplyFunction(function, args);
            default:
                return NULL;
        };
    }

    private static IObject ApplyFunction(IObject function, List<IObject> args)
    {
        switch (function)
        {
            case Function fn:
                var extendedEnvironment = ExtendFunctionEnvironment(fn, args);
                var evaluated = Eval(fn.Body, extendedEnvironment);
                return UnwrapReturnValue(evaluated);
            case Builtin builtin:
                return builtin.Fn(args);
            default:
                return new ErrorObject($"not a function: {function.Type()}"); ;
        }
    }

    private static VariableEnvironment ExtendFunctionEnvironment(Function function, List<IObject> args)
    {
        var environment = new VariableEnvironment(function.Environment);

        for (int i = 0; i < function.Parameters.Count; i++)
        {
            environment.Set(function.Parameters[i].Value, args[i]);
        }

        return environment;
    }

    private static IObject UnwrapReturnValue(IObject obj)
    {
        return obj is ReturnValue returnValue ? returnValue.Value : obj;
    }

    private static List<IObject> EvalExpressions(List<IExpression> arguments, VariableEnvironment environment)
    {
        var result = new List<IObject>();

        foreach (var argument in arguments)
        {
            var evaluated = Eval(argument, environment);
            if (IsError(evaluated))
            {
                return [evaluated];
            }
            result.Add(evaluated);
        }

        return result;
    }

    private static IObject EvalIdentifier(Identifier identifier, VariableEnvironment environment)
    {
        var value = environment.Get(identifier.Value);
        if (value is not ErrorObject)
        {
            return value;
        }

        if (BUILTINS.TryGetValue(identifier.Value, out var builtin))
        {
            return builtin;
        }

        return value;
    }

    private static IObject EvalProgram(Program program, VariableEnvironment environment)
    {
        IObject result = new NullObject();

        foreach (var statement in program.Statements)
        {
            result = Eval(statement, environment);

            switch (result)
            {
                case ReturnValue returnValue:
                    return returnValue.Value;
                case ErrorObject error:
                    return error;
            }
        }

        return result;
    }

    private static IObject EvalBlockStatements(BlockStatement block, VariableEnvironment environment)
    {
        IObject result = new NullObject();

        foreach (var statement in block.Statements)
        {
            result = Eval(statement, environment);

            if (result is not NullObject && (result.Type() == ObjectType.RETURN_VALUE || result.Type() == ObjectType.ERROR))
            {
                return result;
            }
        }

        return result;
    }

    private static IObject EvalIfExpression(IfExpression ifExpression, VariableEnvironment environment)
    {
        var condition = Eval(ifExpression.Condition, environment);

        if (IsError(condition))
        {
            return condition;
        }

        if (IsTruthy(condition))
        {
            return Eval(ifExpression.Consequence, environment);
        }
        else if (ifExpression.Alternative is not null)
        {
            return Eval(ifExpression.Alternative, environment);
        }
        else
        {
            return NULL;
        }
    }

    private static IObject EvalInfixExpression(string @operator, IObject left, IObject right)
    {
        if (left is IntegerObject leftInteger && right is IntegerObject rightInteger)
        {
            return @operator switch
            {
                "+" => new IntegerObject(leftInteger.Value + rightInteger.Value),
                "-" => new IntegerObject(leftInteger.Value - rightInteger.Value),
                "*" => new IntegerObject(leftInteger.Value * rightInteger.Value),
                "/" => new IntegerObject(leftInteger.Value / rightInteger.Value),
                "<" => leftInteger.Value < rightInteger.Value ? TRUE : FALSE,
                ">" => leftInteger.Value > rightInteger.Value ? TRUE : FALSE,
                "==" => leftInteger.Value == rightInteger.Value ? TRUE : FALSE,
                "!=" => leftInteger.Value != rightInteger.Value ? TRUE : FALSE,
                _ => new ErrorObject($"unknown operator: {left.Type()} {@operator} {right.Type()}")
            };
        }

        if (left is BooleanObject leftBoolean && right is BooleanObject rightBoolean)
        {
            return @operator switch
            {
                "==" => leftBoolean.Value == rightBoolean.Value ? TRUE : FALSE,
                "!=" => leftBoolean.Value != rightBoolean.Value ? TRUE : FALSE,
                _ => new ErrorObject($"unknown operator: {left.Type()} {@operator} {right.Type()}")
            };
        }

        if (left is StringObject leftString && right is StringObject rightString)
        {
            return @operator switch
            {
                "+" => new StringObject(leftString.Value + rightString.Value),
                _ => new ErrorObject($"unknown operator: {left.Type()} {@operator} {right.Type()}")
            };
        }

        if (left.Type() != right.Type())
        {
            return new ErrorObject($"type mismatch: {left.Type()} {@operator} {right.Type()}");
        }

        return new ErrorObject($"unknown operator: {left.Type()} {@operator} {right.Type()}");
    }

    private static IObject EvalPrefixExpression(string @operator, IObject right)
    {
        return @operator switch
        {
            "!" => EvalBangOperatorExpression(right),
            "-" => EvalMinusPrefixOperatorExpression(right),
            _ => new ErrorObject($"unknown operator: {@operator}{right.Type()}")
        };
    }

    private static BooleanObject EvalBangOperatorExpression(IObject right)
    {
        return right switch
        {
            BooleanObject boolean => boolean.Value ? FALSE : TRUE,
            NullObject => TRUE,
            _ => FALSE
        };
    }

    private static IObject EvalMinusPrefixOperatorExpression(IObject right)
    {
        if (right is not IntegerObject integer)
        {
            return new ErrorObject($"unknown operator: -{right.Type()}");
        }

        return new IntegerObject(-integer.Value);
    }

    private static bool IsTruthy(IObject obj)
    {
        return obj switch
        {
            BooleanObject boolean => boolean.Value,
            NullObject => false,
            _ => true
        };
    }

    private static bool IsError(IObject obj)
    {
        return obj.Type() == ObjectType.ERROR;
    }
}
