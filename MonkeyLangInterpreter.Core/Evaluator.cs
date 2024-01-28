using MonkeyLangInterpreter.Core.Interfaces;
using MonkeyLangInterpreter.Core.Nodes;
using MonkeyLangInterpreter.Core.Objects;

namespace MonkeyLangInterpreter.Core;

public class Evaluator
{
    private static readonly BooleanObject TRUE = new(true);
    private static readonly BooleanObject FALSE = new(false);
    private static readonly NullObject NULL = new();
    public static IObject Eval(INode node)
    {
        switch (node)
        {
            case Program program:
                return EvalProgram(program);
            case PrefixExpression prefixExpression:
                var expression = Eval(prefixExpression.Right);
                return EvalPrefixExpression(prefixExpression.Operator, expression);
            case InfixExpression infixExpression:
                var leftSide = Eval(infixExpression.Left);
                var rightSide = Eval(infixExpression.Right);
                return EvalInfixExpression(infixExpression.Operator, leftSide, rightSide);
            case BlockStatement blockStatement:
                return EvalBlockStatements(blockStatement);
            case ReturnStatement returnStatement:
                var value = Eval(returnStatement.ReturnValue);
                return new ReturnValue(value);
            case IfExpression ifExpression:
                return EvalIfExpression(ifExpression);
            case ExpressionStatement expressionStatement:
                return Eval(expressionStatement.Expression);
            case IntegerLiteral integerLiteral:
                return new IntegerObject(integerLiteral.Value);
            case BooleanExpression booleanExpression:
                return booleanExpression.Value ? TRUE : FALSE;
            default:
                return NULL;
        };
    }

    private static IObject EvalProgram(Program program)
    {
        IObject result = new NullObject();

        foreach (var statement in program.Statements)
        {
            result = Eval(statement);

            if (result is ReturnValue returnValue)
            {
                return returnValue.Value;
            }
        }

        return result;
    }

    private static IObject EvalBlockStatements(BlockStatement block)
    {
        IObject result = new NullObject();

        foreach (var statement in block.Statements)
        {
            result = Eval(statement);

            if (result is not NullObject && result.Type() == ObjectType.RETURN_VALUE)
            {
                return result;
            }
        }

        return result;
    }

    private static IObject EvalIfExpression(IfExpression ifExpression)
    {
        var condition = Eval(ifExpression.Condition);

        if (IsTruthy(condition))
        {
            return Eval(ifExpression.Consequence);
        }
        else if (ifExpression.Alternative is not null)
        {
            return Eval(ifExpression.Alternative);
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
                _ => NULL
            };
        }

        if (left is BooleanObject leftBoolean && right is BooleanObject rightBoolean)
        {
            return @operator switch
            {
                "==" => leftBoolean.Value == rightBoolean.Value ? TRUE : FALSE,
                "!=" => leftBoolean.Value != rightBoolean.Value ? TRUE : FALSE,
                _ => NULL
            };
        }

        return @operator switch
        {
            "==" => left == right ? TRUE : FALSE,
            "!=" => left != right ? TRUE : FALSE,
            _ => NULL
        };
    }

    private static IObject EvalPrefixExpression(string @operator, IObject right)
    {
        return @operator switch
        {
            "!" => EvalBangOperatorExpression(right),
            "-" => EvalMinusPrefixOperatorExpression(right),
            _ => NULL
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
            return NULL;
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
}
