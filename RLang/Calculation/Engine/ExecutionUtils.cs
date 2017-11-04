using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RLang.Calculation.Engine {
    public class ExecutionUtils {

        public static ExpressionType ExpressionFromCLRType(Type t) {

            if (t == null)
                return ExpressionType.NONE;
            else if (t == typeof(DateTime))
                return ExpressionType.DATE;
            else if (t == typeof(string))
                return ExpressionType.STRING;
            else if (t == typeof(Int16) || t == typeof(Int32) || t == typeof(Int64))
                return ExpressionType.NUMBER;
            else if (t == typeof(UInt16) || t == typeof(UInt32) || t == typeof(UInt64))
                return ExpressionType.NUMBER;
            else if (t == typeof(double) || t == typeof(float))
                return ExpressionType.NUMBER;
            else if (t == typeof(ContextTable))
                return ExpressionType.TABLE;
            else if (t == typeof(ContextTable.Range))
                return ExpressionType.TABLE;
            else if (t == typeof(bool))
                return ExpressionType.BOOLEAN;
            else
                return ExpressionType.VARIANT;

        }


    }

    public class FxSymbolVariable {

        public static Expression CreateCellVariableExpression(IdentifierSymbol symbol, ParameterExpression ctxParam) {

            var dataTableProperty = Expression.Property(
                ctxParam,
                ExecutionContext.DataProperty()
            );

            Expression rowProperty = null;

            if (symbol.Row == null) {
                rowProperty = Expression.Property(
                    ctxParam,
                    ExecutionContext.RowNumberProperty()
                );
            } else {
                if (symbol.IsRelative) {
                    rowProperty = Expression.Add(
                        Expression.Constant((int)symbol.Row),
                        Expression.Property(ctxParam, ExecutionContext.RowNumberProperty())
                    );
                } else {
                    rowProperty = Expression.Constant((int)symbol.Row);
                }
            }

            var tableValueProperty = Expression.Property(
                dataTableProperty,
                ExecutionContext.TableValueProperty(),
                rowProperty,
                Expression.Constant(symbol.Definition)
            );

            return tableValueProperty;

        }

        public static Expression CreateAssignLoopExpression(IdentifierSymbol symbol, Expression body, 
            List<Expression> eList, List<ParameterExpression> pList, 
            ParameterExpression ctxParam, int start, int end
        ) {
            var rowProperty = Expression.Property(
                ctxParam,
                ExecutionContext.RowNumberProperty()
            );

            List<Expression> blockExpressions = new List<Expression>();

            blockExpressions.AddRange(eList);

            var symbolExpression = CreateCellVariableExpression(symbol, ctxParam);
            var target = Expression.Label();

            blockExpressions.Add(Expression.Assign(symbolExpression, Expression.Convert(body, typeof(object))));
            blockExpressions.Add(Expression.Assign(rowProperty, Expression.Add(rowProperty, Expression.Constant(1))));
            blockExpressions.Add(Expression.IfThen(
                Expression.GreaterThan(rowProperty, Expression.Constant(end)),
                Expression.Goto(target)
            ));

            var loopExpression = Expression.Loop(Expression.Block(pList, blockExpressions));

            pList.Clear();
            eList.Clear();

            return Expression.Block(
                Expression.Assign(rowProperty, Expression.Constant(start)),
                loopExpression,
                Expression.Label(target),
                Expression.Assign(rowProperty, Expression.Constant(0))
            );
        }

        public static Expression CreateAssignLoopExpression(IdentifierSymbol symbol, Expression body,
            List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {

            var rowProperty = Expression.Property(
                ctxParam,
                ExecutionContext.RowNumberProperty()
            );

            List<Expression> blockExpressions = new List<Expression>();

            blockExpressions.AddRange(eList);

            var symbolExpression = CreateCellVariableExpression(symbol, ctxParam);
            var target = Expression.Label();

            blockExpressions.Add(Expression.Assign(symbolExpression, Expression.Convert(body, typeof(object))));
            blockExpressions.Add(Expression.Assign(rowProperty, Expression.Add(rowProperty, Expression.Constant(1))));
            blockExpressions.Add(Expression.IfThen(
                Expression.Property(ctxParam, ExecutionContext.EODNumberProperty()),
                Expression.Goto(target)
            ));

            var loopExpression = Expression.Loop(Expression.Block(pList, blockExpressions));

            pList.Clear();
            eList.Clear();

            return Expression.Block(
                Expression.Assign(rowProperty, Expression.Constant(0)),
                loopExpression,
                Expression.Label(target),
                Expression.Assign(rowProperty, Expression.Constant(0))
            );
        }

        public static Expression GetRangeExpression(IdentifierSymbol symbol, ParameterExpression ctxParam) {

            var dataTableProperty = Expression.Property(
                ctxParam,
                ExecutionContext.DataProperty()
            );

            return Expression.Call(
                dataTableProperty,
                ExecutionContext.RangeMethod(),
                Expression.Constant(symbol.StartPosition.Column),
                Expression.Constant(symbol.StartPosition.Row),
                Expression.Constant(symbol.EndPosition.Column),
                Expression.Constant(symbol.EndPosition.Row)
            );

        }

        public static Expression GetVariableExpression(IdentifierSymbol symbol, ParameterExpression ctxParam, rLangExpression rlExpression) {

            Type t = typeof(object);

            switch (symbol.ExpressionType) {
                case ExpressionType.BOOLEAN: 
                case ExpressionType.BOOLEAN_CONTEXT: t = typeof(bool); break;
                case ExpressionType.DATE: t = typeof(DateTime); break;
                case ExpressionType.NUMBER: t = typeof(double); break;
                case ExpressionType.STRING: t = typeof(string); break;
            }

            if (symbol.LinqExpression == null) {

                switch (symbol.IdentifierType) {
                    case IdentifierSymbolType.Variable:
                        symbol.LinqExpression = Expression.Variable(t, symbol.Definition);
                        break;
                    case IdentifierSymbolType.GlobalVariable:

                        symbol.ExpressionType = ExpressionType.VARIANT;
                        if (rlExpression != null) {
                            rlExpression.ExpressionType = ExpressionType.VARIANT;
                        }

                        symbol.LinqExpression = Expression.Property(
                            ctxParam,
                            ExecutionContext.ValueProperty(),
                            Expression.Constant(symbol.ObjectName),
                            Expression.Constant(symbol.ObjectProperty)
                        );

                        break;

                    case IdentifierSymbolType.RangeVariable:
                        symbol.ExpressionType = ExpressionType.TABLE;
                        if (rlExpression != null) {
                            rlExpression.ExpressionType = ExpressionType.TABLE;
                        }

                        symbol.LinqExpression = GetRangeExpression(symbol, ctxParam);
                        break;

                    case IdentifierSymbolType.CellVariable:

                        symbol.ExpressionType = ExpressionType.VARIANT;
                        if (rlExpression != null) {
                            rlExpression.ExpressionType = ExpressionType.VARIANT;
                        }

                        symbol.LinqExpression = CreateCellVariableExpression(symbol, ctxParam);

                        break;

                    case IdentifierSymbolType.Function:
                        throw new SyntaxErrorException(
                            new SyntaxError(symbol.Line, symbol.Column, symbol.Definition, "[Not a Function]")
                        );
                }
            }

            return symbol.LinqExpression;
        }

        //public static System.Linq.Expressions.Expression GetVariableExpression(string definition, SymbolTable table) {
        //    return System.Linq.Expressions.Expression.Variable(typeof(object), definition);
        //} 

    }

    public class FxStaticMethodShell {

        

        public static MethodInfo GetMethodShell(string definition) {
            throw new NotImplementedException();
        }


    } 
}
