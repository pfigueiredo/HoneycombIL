using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RLang.Calculation.Engine {

    public enum ComparisonOperator { EQ, NEQ, GT, LT, LTEQ, GTEQ }
    public enum BooleanOperator { AND, OR, NOT }
    public enum MathOperator { ADD, SUBTR, MULT, DIVD, POW, CONCATENATE }

    #region interface IExpression ITerminal
    public interface IrLangExpression {
        //string GetExpression(ExpressionType destination);
        object CompileToIL(ExpressionType destination, SymbolTable table);
        Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam);
    }

    public interface ITerminal {
        int Line { get; }
        int Column { get; }
    }

    public interface ITerminalExpression : ITerminal, IrLangExpression { }

    #endregion

    public enum ConversionType { None, Implicit, Explicit, Impossible }

    public abstract class rLangExpression : IrLangExpression {

        public ConversionType CanConvertTo(ExpressionType type) {

            if (type == this.ExpressionType || type == ExpressionType.NONE)
                return ConversionType.Implicit;

            if (type == ExpressionType.TABLE)
                return ConversionType.Impossible;

            return ConversionType.Explicit;
        }
        private static object _locker = new object();

        //private static MethodInfo _convertChangeTypeMethod = null;
        //protected static MethodInfo GetConvertChangeTypeMethod() {
        //    if (_convertChangeTypeMethod == null) {
        //        lock (_locker) {
        //            if (_convertChangeTypeMethod == null) {
        //                _convertChangeTypeMethod = typeof(Convert).GetMethod("ChangeType",
        //                    BindingFlags.Public | BindingFlags.Static,
        //                    Type.DefaultBinder, new Type[] { typeof(object), typeof(Type) }, null);
        //            }
        //        }
        //    }

        //    return _convertChangeTypeMethod;
        //}

        private static MethodInfo _stringConcatMethod = null;
        protected static MethodInfo GetStringConcatMethod() {
            if (_stringConcatMethod == null) {
                lock (_locker) {
                    if (_stringConcatMethod == null) {
                        _stringConcatMethod = typeof(String).GetMethod("Concat",
                            BindingFlags.Public | BindingFlags.Static,
                            Type.DefaultBinder, new Type[] { typeof(string), typeof(string)}, null);
                    }
                }
            }

            return _stringConcatMethod;

        }

        public Expression GetConvertedExpression(Expression expression, ExpressionType type) {

            //var convertMethod = GetConvertChangeTypeMethod();
            Expression convertExpr = null; Type convertType = typeof(object);

            switch (type) {
                case ExpressionType.ANY: return expression;
                case ExpressionType.NONE: return expression;
                case ExpressionType.TABLE:
                    convertExpr = Expression.Call(typeof(ExecutionContext), "ChangeType", 
                        new Type[] { typeof(ContextTable) }, Expression.Convert(expression, typeof(object)));
                    convertType = typeof(ContextTable);
                    break;
                case ExpressionType.VARIANT: return Expression.Convert(expression, typeof(object));
                case ExpressionType.BOOLEAN_CONTEXT:
                case ExpressionType.BOOLEAN:
                    convertExpr = Expression.Call(typeof(ExecutionContext), "ChangeType", 
                        new Type[] { typeof(bool) }, Expression.Convert(expression, typeof(object)));
                    convertType = typeof(bool);
                    break;
                case ExpressionType.NUMBER:
                    convertExpr = Expression.Call(typeof(ExecutionContext), "ChangeType", 
                        new Type[] { typeof(double) }, Expression.Convert(expression, typeof(object)));
                    convertType = typeof(double);
                    break;
                case ExpressionType.DATE:
                    convertExpr = Expression.Call(typeof(ExecutionContext), "ChangeType", 
                        new Type[] { typeof(DateTime) }, Expression.Convert(expression, typeof(object)));
                    convertType = typeof(DateTime);
                    break;
                case ExpressionType.STRING:
                    convertExpr = Expression.Call(typeof(ExecutionContext), "ChangeType", 
                        new Type[] { typeof(string) }, Expression.Convert(expression, typeof(object)));
                    convertType = typeof(string);
                    break;

                default: return expression;
            }

            return Expression.Convert(convertExpr, convertType);

            #region old code
            //switch (type) {
            //    case ExpressionType.ANY: return expression;
            //    case ExpressionType.NONE: return expression;
            //    case ExpressionType.TABLE: return expression; //Let it give error
            //    case ExpressionType.VARIANT: return Expression.Convert(expression, typeof(object));
            //    case ExpressionType.BOOLEAN_CONTEXT:
            //    case ExpressionType.BOOLEAN:
            //        convertExpr = Expression.Call(null, convertMethod, 
            //            Expression.Convert(expression, typeof(object)), 
            //            Expression.Constant(typeof(bool), typeof(Type)));
            //        convertType = typeof(bool);
            //        break;
            //    case ExpressionType.NUMBER:
            //        convertExpr = Expression.Call(null, convertMethod, 
            //            Expression.Convert(expression, typeof(object)), 
            //            Expression.Constant(typeof(double), typeof(Type)));
            //        convertType = typeof(double);
            //        break;
            //    case ExpressionType.DATE:
            //        convertExpr = Expression.Call(null, convertMethod, 
            //            Expression.Convert(expression, typeof(object)), 
            //            Expression.Constant(typeof(DateTime), typeof(Type)));
            //        convertType = typeof(DateTime);
            //        break;
            //    case ExpressionType.STRING:
            //        convertExpr = Expression.Call(null, convertMethod, 
            //            Expression.Convert(expression, typeof(object)), 
            //            Expression.Constant(typeof(string), typeof(Type)));
            //        convertType = typeof(string);
            //        break;

            //    default: return expression;
            //}

            //return Expression.Convert(convertExpr, convertType);
            #endregion

        }

        public BlockExpression LinqBlockExpression { get; set; }

        public virtual ExpressionType ExpressionType { get; set; }
        public virtual rLangExpression Next { get; set; }
        public virtual rLangExpression Inner { get; set; }

        #region IExpression Members

        public object CompileToIL(ExpressionType destination, SymbolTable table) { return null; }
        public abstract Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam); // { return null; }

        #endregion

        public static Expression CreateLinkExpressionTree(
            rLangExpression expression,
            ExpressionType destination, 
            List<Expression> eList, 
            List<ParameterExpression> pList, 
            ParameterExpression ctxParam
        ) {


            var ret = expression.CreateLinkExpression(destination, eList, pList, ctxParam);

            if (expression.Next != null) {
                rLangExpression e = expression;
                while (e.Next != null) {
                    var rNext = e.Next.CreateLinkExpression(ExpressionType.ANY, eList, pList, ctxParam);
                    if (rNext != null) { eList.Add(rNext); }
                    e = e.Next;
                }
            }

            return ret;

        }
    }

    public class ComparisonExpression : rLangExpression {

        public rLangExpression A { get; set; }
        public rLangExpression B { get; set; }
        public ComparisonOperator Op { get; set; }

        public ComparisonExpression(rLangExpression a, rLangExpression b, ComparisonOperator op) {
            this.A = a;
            this.B = b;
            this.Op = op;
            base.ExpressionType = ExpressionType.BOOLEAN;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {

            ExpressionType cExType = (A.ExpressionType == B.ExpressionType) ? A.ExpressionType : ExpressionType.NUMBER;

            var rA = A.CreateLinkExpression(cExType, eList, pList, ctxParam);
            var rB = B.CreateLinkExpression(cExType, eList, pList, ctxParam);
            Expression expression = null;
            switch (this.Op) {
                case ComparisonOperator.GT: expression = Expression.GreaterThan(rA, rB); break;
                case ComparisonOperator.LT: expression = Expression.LessThan(rA, rB); break;
                case ComparisonOperator.EQ: expression = Expression.Equal(rA, rB); break;
                case ComparisonOperator.NEQ: expression = Expression.NotEqual(rA, rB); break;
                case ComparisonOperator.GTEQ: expression = Expression.GreaterThanOrEqual(rA, rB); break;
                case ComparisonOperator.LTEQ: expression = Expression.LessThanOrEqual(rA, rB); break;
                default:
                    throw new ArgumentException(string.Format("Invalid Operator: '{0}'", this.Op));
            }

            

            var conversion = this.CanConvertTo(destination);
            if (conversion == ConversionType.Implicit)
                return expression;
            else
                return GetConvertedExpression(expression, destination);

            
        }

    }

    public class BooleanExpression : rLangExpression {

        public rLangExpression A { get; set; }
        public rLangExpression B { get; set; }
        public BooleanOperator Op { get; set; }

        public BooleanExpression(List<rLangExpression> list, BooleanOperator op) {
            if (list.Count > 2)
                throw new FunctionArgumentsLenghtException(list.Count, 2, op.ToString());
            A = list[0];
            B = list[1];
            this.Op = op;
        }

        public BooleanExpression(rLangExpression a, rLangExpression b, BooleanOperator op) {
            this.A = a;
            this.B = b;
            this.Op = op;
            base.ExpressionType = ExpressionType.BOOLEAN;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {

            if (destination == ExpressionType.BOOLEAN_CONTEXT)
                this.ExpressionType = ExpressionType.BOOLEAN_CONTEXT;

            Expression expression = null;

            if (ExpressionType == ExpressionType.BOOLEAN_CONTEXT) {

                var rA = A.CreateLinkExpression(ExpressionType.BOOLEAN_CONTEXT, eList, pList, ctxParam);
                var rB = (this.Op != BooleanOperator.NOT) ? B.CreateLinkExpression(ExpressionType.BOOLEAN_CONTEXT, eList, pList, ctxParam) : null;

                switch (this.Op) {
                    case BooleanOperator.AND: expression = Expression.AndAlso(rA, rB); break;
                    case BooleanOperator.OR: expression = Expression.OrElse(rA, rB); break;
                    case BooleanOperator.NOT: expression = Expression.Not(rA); break;
                    default: throw new ArgumentException(string.Format("Invalid Operator: '{0}'", this.Op));
                }
            } else {

                var rA = A.CreateLinkExpression(ExpressionType.BOOLEAN, eList, pList, ctxParam);
                var rB = (this.Op != BooleanOperator.NOT) ? B.CreateLinkExpression(ExpressionType.BOOLEAN, eList, pList, ctxParam) : null;

                switch (this.Op) {
                    case BooleanOperator.AND: expression = Expression.And(rA, rB); break;
                    case BooleanOperator.OR: expression = Expression.Or(rA, rB); break;
                    case BooleanOperator.NOT: expression = Expression.Not(rA); break;
                    default: throw new ArgumentException(string.Format("Invalid Operator: '{0}'", this.Op));
                }
            }

            ConversionType conversion = this.CanConvertTo(destination);
            if (conversion == ConversionType.Implicit)
                return expression;
            else
                return GetConvertedExpression(expression, destination);

            
        }

    }

    public class ConditionalExpression : rLangExpression {

        public rLangExpression C { get; set; }
        public rLangExpression T { get; set; }
        public rLangExpression E { get; set; }
        private bool UseIfFunction = false;

        public ConditionalExpression(rLangExpression c, rLangExpression t, rLangExpression e, bool useEqFunction) {
            this.C = c;
            this.T = t;
            this.E = e;
            base.ExpressionType = ExpressionType.VARIANT;
            this.UseIfFunction = useEqFunction;
        }

        public ConditionalExpression(List<rLangExpression> args) {
            this.C = args[0];
            this.T = (args.Count > 1) ? args[1] : null;
            this.E = (args.Count > 2) ? args[2] : null; ;
            base.ExpressionType = ExpressionType.VARIANT;
            this.UseIfFunction = true;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {
            Expression expression = null;

            var ret = Expression.Variable(typeof(object));

            //Expression.IfThen
            var rC = C.CreateLinkExpression(ExpressionType.BOOLEAN_CONTEXT, eList, pList, ctxParam);
            var rT = (T != null) ? Expression.Assign(ret, T.CreateLinkExpression(ExpressionType.VARIANT, eList, pList, ctxParam)) : null;
            var rE = (E != null) ? Expression.Assign(ret, E.CreateLinkExpression(ExpressionType.VARIANT, eList, pList, ctxParam)) : null;

            if (rT != null && rE != null) {
                expression = Expression.IfThenElse(rC, rT, rE);
            } else if (rT != null) {
                expression = Expression.IfThen(rC, rT);
            } else
                throw new ArgumentException("the 'Then' in IF/THEN/ELSE statement can't be null");

            pList.Add(ret);
            eList.Add(expression);

            ConversionType conversion = this.CanConvertTo(destination);
            if (conversion == ConversionType.Implicit)
                return ret;
            else
                return GetConvertedExpression(ret, destination);
        }

    }

    public class MathExpression : rLangExpression {

        public rLangExpression A { get; set; }
        public rLangExpression B { get; set; }
        public MathOperator Op { get; set; }

        public MathExpression(rLangExpression a, rLangExpression b, MathOperator op) {
            this.A = a;
            this.B = b;
            this.Op = op;
            if (op == MathOperator.CONCATENATE)
                base.ExpressionType = ExpressionType.STRING;
            else
                base.ExpressionType = ExpressionType.NUMBER;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {
            ConversionType conversion = this.CanConvertTo(destination);
            Expression expression = null;

            var rA = A.CreateLinkExpression(this.ExpressionType, eList, pList, ctxParam);
            var rB = B.CreateLinkExpression(this.ExpressionType, eList, pList, ctxParam);

            switch (this.Op) {
                case MathOperator.CONCATENATE: expression = Expression.Call(null, rLangExpression.GetStringConcatMethod(), rA, rB); break;
                case MathOperator.ADD: expression = Expression.Add(rA, rB); break;
                case MathOperator.SUBTR: expression = Expression.Subtract(rA, rB); break;
                case MathOperator.MULT: expression = Expression.Multiply(rA, rB); break;
                case MathOperator.DIVD: expression = Expression.Divide(rA, rB); break;
                case MathOperator.POW: expression = Expression.Power(rA, rB); break;
            }

            if (conversion == ConversionType.Implicit)
                return expression;
            else
                return GetConvertedExpression(expression, destination);

        }

    }

    public class PercentExpression : rLangExpression {

        public rLangExpression A { get; set; }

        public PercentExpression(rLangExpression a) {
            this.A = a;
            base.ExpressionType = ExpressionType.NUMBER;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {
            
            Expression expression = null;

            var rA = A.CreateLinkExpression(ExpressionType.NUMBER, eList, pList, ctxParam);
            var rB = Expression.Constant((double)100);

            expression = Expression.Divide(rA, rB);

            ConversionType conversion = this.CanConvertTo(destination);
            if (conversion == ConversionType.Implicit)
                return expression;
            else
                return GetConvertedExpression(expression, destination);

        }

    }

    public class NegateExpression : rLangExpression {

        public rLangExpression A { get; set; }

        public NegateExpression(rLangExpression a) {
            this.A = a;
            base.ExpressionType = ExpressionType.NUMBER;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {
            
            Expression expression = null;

            var rA = A.CreateLinkExpression(ExpressionType.NUMBER, eList, pList, ctxParam);
            var rB = Expression.Constant((double)-1);

            expression = Expression.Multiply(rA, rB);

            ConversionType conversion = this.CanConvertTo(destination);
            if (conversion == ConversionType.Implicit)
                return expression;
            else
                return GetConvertedExpression(expression, destination);

            

        }

    }

    public class ParenthesesExpression : rLangExpression {

        public rLangExpression A { get; set; }

        public ParenthesesExpression(rLangExpression a) {
            this.A = a;
            base.ExpressionType = a.ExpressionType;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {
            return A.CreateLinkExpression(destination, eList, pList, ctxParam);
        }

    }

    public class FunctionExpression : rLangExpression {

        public List<rLangExpression> Args { get; set; }
        public IdentifierSymbol Identifier { get; set; }
        public FunctionDefinition FunctionDefinition { get; set; }

        public FunctionExpression(IdentifierSymbol identifier, List<rLangExpression> args) {
            this.Args = args;
            this.Identifier = identifier;
            this.Identifier.IdentifierType = IdentifierSymbolType.Function;
            this.Identifier.NumOfArguments = args.Count;

            if (this.Identifier.Definition.IndexOfAny(new char[] { '*', '?' }) >= 0)
                throw new LexicalErrorException(
                    new LexicalError(Identifier.Line, Identifier.Column, Identifier.Definition)
                );

            this.FunctionDefinition = this.Identifier.GetFunctionDefinition();
            this.Identifier.ValidateFunctionDefinition();
            this.Identifier.ExpressionType = this.FunctionDefinition.ReturnType;
            base.ExpressionType = this.FunctionDefinition.ReturnType;

        }


        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {

            var fncDef = Identifier.GetFunctionDefinition();
            if (fncDef == null || fncDef.Method == null)
                throw new SyntaxErrorException(
                    new SyntaxError(this.Identifier.Line, this.Identifier.Column, this.Identifier.Definition, "Valid Function")
                );

            List<Expression> fParams = new List<Expression>();

            if (fncDef.InjectContext) fParams.Add(ctxParam);

            for (int i = 0; i < Args.Count; i++) {

                ExpressionType paramType = ExpressionType.VARIANT;
                Type fncParamType = typeof(object);

                if (this.FunctionDefinition.ParamTypes.Count > i)
                    paramType = this.FunctionDefinition.ParamTypes[i];

                var methodParamTypes = fncDef.Method.GetParameters();

                int p = i + ((fncDef.InjectContext) ? 1 : 0);

                if (methodParamTypes.Count() > p)
                    fncParamType = methodParamTypes[p].ParameterType;

                var pExp = Args[i].CreateLinkExpression(paramType, eList, pList, ctxParam);

                if (fncParamType != pExp.Type)
                    pExp = Expression.Convert(pExp, fncParamType);

                fParams.Add(pExp);

            }

            Expression expression = Expression.Call(null, fncDef.Method, fParams.ToArray());

            ConversionType conversion = this.CanConvertTo(destination);
            if (conversion != ConversionType.Implicit)
                expression = GetConvertedExpression(expression, destination);

            return expression;

        }

    }

    public class NumberExpression : rLangExpression {

        public Double Number { get; set; }

        public NumberExpression(Double number) {
            this.Number = number;
            base.ExpressionType = ExpressionType.NUMBER;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {
            var expression = Expression.Constant(this.Number);

            ConversionType conversion = this.CanConvertTo(destination);
            if (conversion == ConversionType.Implicit)
                return expression;
            else
                return GetConvertedExpression(expression, destination);

        }

    }

    public class StringExpression : rLangExpression {

        public string Value { get; set; }

        public StringExpression(string value) {
            this.Value = value;
            base.ExpressionType = ExpressionType.STRING;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {
            var expression = Expression.Constant(this.Value);

            ConversionType conversion = this.CanConvertTo(destination);
            if (conversion == ConversionType.Implicit)
                return expression;
            else
                return GetConvertedExpression(expression, destination);

        }

    }

    public class IdentifierExpression : rLangExpression {

        public IdentifierSymbol Symbol { get; set; }
        public override ExpressionType ExpressionType {
            get { return Symbol.ExpressionType; }
            set { Symbol.ExpressionType = value; }
        }

        public IdentifierExpression(IdentifierSymbol symbol) {
            this.Symbol = symbol;
        }

        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {

            Expression expression = null;

            if (this.Symbol.IsConstantFunction) {

                var fncDef = Symbol.GetFunctionDefinition();
                if (fncDef == null || fncDef.Method == null)
                    throw new SyntaxErrorException(
                        new SyntaxError(this.Symbol.Line, this.Symbol.Column, this.Symbol.Definition, "Valid Function")
                    );

                expression = Expression.Call(null, fncDef.Method, new Expression[0]);

            } else
                expression = FxSymbolVariable.GetVariableExpression(this.Symbol, ctxParam, this);


            ConversionType conversion = this.CanConvertTo(destination);
            if (conversion == ConversionType.Implicit)
                return expression;
            else
                return GetConvertedExpression(expression, destination);
        }

    }

    public class AssignExpression : rLangExpression {

        public IdentifierSymbol Symbol { get; set; }
        public SymbolTable SymbolTable { get; set; }

        public override ExpressionType ExpressionType {
            get { return Symbol.ExpressionType; }
            set { Symbol.ExpressionType = value; }
        }

        public AssignExpression(IdentifierSymbol symbol, rLangExpression expression, SymbolTable symbolTable) {
            this.Symbol = symbol;
            this.Symbol.IsValid = true;
            this.SymbolTable = symbolTable;
            this.SymbolTable.AddSymbol(symbol);
            base.Inner = expression;
            if (!Symbol.IsExecutionContextVariable)
                this.Symbol.ExpressionType = expression.ExpressionType;
            else
                this.Symbol.ExpressionType = ExpressionType.VARIANT;
        }


        public override Expression CreateLinkExpression(ExpressionType destination, List<Expression> eList, List<ParameterExpression> pList, ParameterExpression ctxParam) {

            List<Expression> subEList = new List<Expression>();
            List<ParameterExpression> subPList = new List<ParameterExpression>();

            Expression rAE = null, rA = null;

            if (Symbol.IdentifierType == IdentifierSymbolType.CellVariable && Symbol.Row == null) {
                var rB = Inner.CreateLinkExpression(Symbol.ExpressionType, subEList, subPList, ctxParam);
                rAE = FxSymbolVariable.CreateAssignLoopExpression(Symbol, rB, subEList, subPList, ctxParam);
            } else if (Symbol.IdentifierType == IdentifierSymbolType.RangeVariable) {
                var rB = Inner.CreateLinkExpression(Symbol.ExpressionType, subEList, subPList, ctxParam);
                rAE = FxSymbolVariable.CreateAssignLoopExpression(Symbol, rB, subEList, subPList, ctxParam, Symbol.StartPosition.Row, Symbol.EndPosition.Row);
            } else {
                rA = FxSymbolVariable.GetVariableExpression(Symbol, ctxParam, null);
                //needs to be after because the previews can change the type of the current expression...
                //have to correct this because its not as elegant as the variable itself to know its own type :P
                var expressionType = (Symbol.ExpressionType == ExpressionType.ANY) ? ExpressionType.VARIANT : Symbol.ExpressionType;
                var rB = Inner.CreateLinkExpression(expressionType, subEList, subPList, ctxParam);
                rAE = Expression.Assign(rA, rB);
            }

            subEList.Add(rAE);
            var block = Expression.Block(subPList, subEList);

            eList.Add(block);

            return null;

        }

        
    }


}
