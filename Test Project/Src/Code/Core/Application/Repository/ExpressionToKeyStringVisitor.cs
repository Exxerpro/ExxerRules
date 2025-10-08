// <copyright file="ExpressionToKeyStringVisitor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repository;

using System.Text;

internal sealed class ExpressionToKeyStringVisitor : ExpressionVisitor
{
    private readonly StringBuilder builder = new();

    public static string ToKeyString(Expression expression)
    {
        var visitor = new ExpressionToKeyStringVisitor();
        visitor.Visit(expression);
        return visitor.builder.ToString();
    }

    /// <inheritdoc/>
    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        this.builder.Append($"{node.Parameters[0].Name} => ");
        this.Visit(node.Body);
        return node;
    }

    /// <inheritdoc/>
    protected override Expression VisitBinary(BinaryExpression node)
    {
        this.builder.Append("(");
        this.Visit(node.Left);
        this.builder.Append($" {node.NodeType} ");
        this.Visit(node.Right);
        this.builder.Append(")");
        return node;
    }

    /// <inheritdoc/>
    protected override Expression VisitMember(MemberExpression node)
    {
        // Check if it's a captured constant from a closure (e.g., machineId)
        if (node.Expression is ConstantExpression constExpr)
        {
            var value = GetValueFromClosure(constExpr, node.Member);
            this.builder.Append(ValueToString(value));
            return node;
        }

        if (node.Expression is ParameterExpression param)
        {
            this.builder.Append($"{param.Name}.{node.Member.Name}");
            return node;
        }

        // Fallback to member name
        this.builder.Append(node.Member.Name);
        return node;
    }

    /// <inheritdoc/>
    protected override Expression VisitConstant(ConstantExpression node)
    {
        this.builder.Append(ValueToString(node.Value));
        return node;
    }

    private static object? GetValueFromClosure(ConstantExpression constantExpression, MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.GetValue(constantExpression.Value),
            PropertyInfo prop => prop.GetValue(constantExpression.Value),
            _ => null,
        };
    }

    private static string ValueToString(object? value)
    {
        return value switch
        {
            null => "null",
            string s => $"\"{s}\"",
            DateTime dt => dt.ToString("O"), // ISO format
            _ => value.ToString() ?? string.Empty,
        };
    }
}

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate expression visitor logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
