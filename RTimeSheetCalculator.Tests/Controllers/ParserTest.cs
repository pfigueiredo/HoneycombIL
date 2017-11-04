using RLang.Calculation.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RTimeSheetCalculator.Tests.Controllers {

    
    public class ParserTest {


        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression) {
            var member = expression.Body as MethodCallExpression;

            if (member != null)
                return member.Method;

            throw new ArgumentException("Expression is not a method", "expression");
        }


        [Fact(DisplayName = "Setup the Parser")]
        public void SetupParser() {
            var parser = new rLangParser();
            parser.Setup();
        }

        [Theory(DisplayName = "Invoke Parse")]
        [InlineData("=1")]
        public void SimpleParse(string expression) {
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.IL);
        }

        [Fact(DisplayName = "Assign variable (Int)")]
        public void AssignInt() {
            string expression = "var1 = { 1 }";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)1);
        }

        [Fact(DisplayName = "Assing variable (FlotingPoint)")]
        public void AssignDouble() {
            string expression = "var1 = { 1.1 }";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)1.1);
        }

        [Fact(DisplayName = "Assing variable (String)")]
        public void AssignString() {
            string expression = "var1 = { \"Hello World.\" }";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, "Hello World.");
        }

        [Theory(DisplayName = "Add and Subtract Values")]
        [InlineData("=1 +1")]
        [InlineData("=1 + 2-1")]
        [InlineData("=3- 1")]
        [InlineData("=-2 + 4")]
        [InlineData("=4 + -2")]
        public void AddExpression(string expression) {
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)2);
        }

        [Theory(DisplayName = "Multiply and Divide Values")]
        [InlineData("=1 *1")]
        [InlineData("=1 * 2/2")]
        [InlineData("=4/ 2/2")]
        [InlineData("=-1 * -1")]
        [InlineData("=-2 / -2")]
        [InlineData("=2 / 2")]
        public void MultiExpression(string expression) {
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)1);
        }

        [Theory(DisplayName = "AND OR Values")]
        [InlineData("=1 > 0 AND 0 < 1")]
        [InlineData("=1 > 1 OR 0 < 1")]
        [InlineData("=1 > 0 OR 0 >= 1")]
        [InlineData("=NOT (1 < 0)")]
        //Excel type boolean operations
        [InlineData("=AND(1 > 0, 0 < 1)")]
        [InlineData("=OR(1 > 1, 0 < 1)")]
        [InlineData("=OR(1 > 0, 0 >= 1)")]
        public void BoolExpression(string expression) {
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(true, ret.Value);
        }


        [Fact(DisplayName = "Pow Values")]
        public void OperatorPow() {

            string expression = "=2 ^ 8";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)(Math.Pow(2.0, 8.0)));
        }

        [Fact(DisplayName = "Percent Values")]
        public void OperatorPercent() {

            string expression = "=123 * 10%";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)( 123.0 * 0.10 ) );
        }

        [Fact(DisplayName = "Percentage supports decimal places")]
        public void OperatorPercent2() {

            string expression = "= 123.10 * 12.5%";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)(123.10 * 0.125));

        }

        [Theory(DisplayName = "Contact Strings")]
        [InlineData("\"Foo\"", "\"bar\"")]
        [InlineData("\"Foo\"", 123)]
        [InlineData(4, 2.1)]
        [InlineData(42, "\":Live, The universe and everything\"")]
        public void ContactStrings(object token1, object token2) {

            string expression = string.Format("={0} & {1}", token1, token2);
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, string.Concat(
                (token1.GetType() == typeof(string)) ? ((string)token1).Replace("\"", "") : token1,
                (token2.GetType() == typeof(string)) ? ((string)token2).Replace("\"", "") : token2
            ));
        }


        [Fact(DisplayName = "Operator Precedence (Add,Subtract,Mult,Divide) A")]
        public void Precedence1() {

            string expression = "=2 * 2 - 4 / 3 + 2";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)(2.0 * 2.0 - 4.0 / 3.0 + 2.0));
        }


        [Fact(DisplayName = "Operator Precedence (Add,Subtract,Mult,Divide) B")]
        public void Precedence2() {

            string expression = "=2 * (2 - 4) / 3 + 2";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)(2.0 * (2.0 - 4.0) / 3.0 + 2.0));
        }

        [Fact(DisplayName = "Operator Precedence (Add,Subtract,Mult,Divide) C")]
        public void Precedence3() {

            string expression = "=2 * ((2 - 4) / (3 + 2))";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)( 2.0 * ((2.0 - 4.0) / (3.0 + 2.0)) ));
        }


        [Fact(DisplayName = "Operator Precedence (Add,Subtract,Mult,Divide,Pow,Percent) D")]
        public void Precedence4() {

            string expression = "=2 * ((2^3 - 4) / (3 + 200 * 15%))";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)(2.0 * ((Math.Pow(2.0, 3.0) - 4.0) / (3.0 + 200.0 * 0.15))));

        }

        [Fact(DisplayName = "Read Variable")]
        public void Variable1() {

            string expression = "var1 = {10} var2 = {var1 * 2}";
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)(20));

        }


        [Fact(DisplayName = "Context Variable")]
        public void Variable2() {

            string expression = "@var1 = {10} @var2 = {@var1 * 2}";

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, (double)(10.0 * 2.0));

            Assert.Equal(ret.ExecutionContext["var1"], 10.0);
            Assert.Equal(ret.ExecutionContext["var2"], 20.0);

        }


        [Fact(DisplayName = "Context String Variable")]
        public void Variable3() {
       
            string expression = "@var1 = { \"Foo\" } @var2 = {\"Bar\"}";

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, "Bar");

            Assert.Equal(ret.ExecutionContext["var1"], "Foo");
            Assert.Equal(ret.ExecutionContext["var2"], "Bar");

        }

        [Fact(DisplayName = "Context String Variable (2)")]
        public void Variable4() {

            string expression = "@var1 = { \"Foobar\" } @var2 = { @var1 }";

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(ret.Value, "Foobar");

            Assert.Equal(ret.ExecutionContext["var1"], "Foobar");
            Assert.Equal(ret.ExecutionContext["var2"], "Foobar");

        }


        [Theory(DisplayName = "Custom Function")]
        [InlineData("=Mult(2,6)")]
        [InlineData("=Mult(1 + 1,(3 *2))")]
        [InlineData("=Mult((10 - 6) / 2,6)")]
        [InlineData("=Mult(2*1,6/3*3)")]
        [InlineData("=Mult(2,\"6\")")] //Convert string
        public void Function1(string expression) {

            Dictionary<string, MethodInfo> builtinMethods = new Dictionary<string, MethodInfo>();

            builtinMethods.Add("Mult", (new dMult(Mult)).Method);

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC, builtinMethods);
            Assert.Equal(ret.Value, (double)12);

        }

        [Theory(DisplayName = "Wrong number of Arguments")]
        [InlineData("=Mult(2)")]
        [InlineData("=Mult(1 + 1,(3 *2), 3)")]
        [InlineData("=Mult((10 - 6) / 2,6,2)")]
        [InlineData("=Mult(2*1)")]
        public void Function2(string expression) {

            Dictionary<string, MethodInfo> builtinMethods = new Dictionary<string, MethodInfo>();

            builtinMethods.Add("Mult", (new dMult(Mult)).Method);

            var parser = new rLangParser();
            parser.Setup();
            try {
                var ret = parser.Parse(expression, ParserBackend.EXEC, builtinMethods);
            } catch (FunctionArgumentsLenghtException ex) {
                Assert.Equal(2, ex.ExpectedNumOfArgs);
                Assert.True(true, String.Format("Expected {0} got {1}", ex.ExpectedNumOfArgs, ex.NumOfArgs));
            }
        }

        delegate double dMult(double a, double b);
        private static Double Mult(Double a, Double b) {
            return a * b;
        }

        [Fact(DisplayName = "Inject context into external function")]
        public void Function3() {

            Dictionary<string, MethodInfo> builtinMethods = new Dictionary<string, MethodInfo>();
            builtinMethods.Add("MultContext", (new dMultContext(MultContext)).Method);

            string expression = "=MultContext(\"Foo\", 2)";

            ExecutionContext ctx = new ExecutionContext();
            ctx["Foo"] = 10.0;

            var parser = new rLangParser();
            parser.Setup();
            
            var ret = parser.Parse(expression, ParserBackend.EXEC, ctx, builtinMethods);
            Assert.Equal(20.0, ret.Value);
            
        }

        delegate double dMultContext(ExecutionContext context, string var, double a);
        private static Double MultContext(ExecutionContext context, string var, double a) {
            return (double)context[var] * a;
        }

        [Theory(DisplayName = "Syntax Error")]
        [InlineData("1 +")]
        [InlineData("1 + (")]
        [InlineData("(1 + 2 / 3 * ) 3")]
        [InlineData("(1 + 2 / 3 * ( 3")]
        [InlineData("NN(2")]
        [InlineData("N N(2")]
        public void SyntaxError1 (string expression) {
            var parser = new rLangParser();
            parser.Setup();
            try {
                var ret = parser.Parse(expression, ParserBackend.IL);
            } catch (SyntaxErrorException ex) {
                Assert.True(true, ex.Error.ToString());
            }
        }

        [Theory(DisplayName = "Function Not Found")]
        [InlineData("=Foo()")]
        [InlineData("=Bar(2)")]
        [InlineData("=FooBar(2, 2)")]
        public void SyntaxError2(string expression) {
            var parser = new rLangParser();
            parser.Setup();
            try {
                var ret = parser.Parse(expression, ParserBackend.IL);
            } catch (FunctionNotFoundException) {
                Assert.True(true);
            }
        }

        [Fact(DisplayName = "Execution over context list (x100)")]
        public void ExecContext() {

            var expression = "@r1 = { @var1 * @var2 } @r2 = { @var1 + @var3 } @r3 = { Mult(@var1, @var2) }";

            var parser = new rLangParser();
            parser.Setup();

            Dictionary<string, MethodInfo> builtinMethods = new Dictionary<string, MethodInfo>();

            builtinMethods.Add("Mult", (new dMult(Mult)).Method);
            var ctx = PrepareContextList();

            var ret = parser.Parse(expression, ParserBackend.EXEC, ctx, builtinMethods);

            Assert.NotNull(ret);
            ctx = ret.ExecutionContext;

            while (ctx != null) {

                Assert.Equal((double)Convert.ChangeType(ctx["var1"], typeof(double)) * (double)Convert.ChangeType(ctx["var2"], typeof(double)), Convert.ChangeType(ctx["r1"], typeof(double)));
                Assert.Equal((double)Convert.ChangeType(ctx["var1"], typeof(double)) + (double)Convert.ChangeType(ctx["var3"], typeof(double)), Convert.ChangeType(ctx["r2"], typeof(double)));
                Assert.Equal((double)Convert.ChangeType(ctx["var1"], typeof(double)) * (double)Convert.ChangeType(ctx["var2"], typeof(double)), Convert.ChangeType(ctx["r3"], typeof(double)));

                ctx = ctx.Next;

            }

        }


        public ExecutionContext PrepareContextList() {

            List<ExecutionContext> list = new List<ExecutionContext>();

            for (int i = 0; i < 100; i++) {
                var ctx = new ExecutionContext();

                ctx["var1"] = i;
                ctx["var2"] = 2;
                ctx["var3"] = 5;
                ctx["r1"] = 0;
                ctx["r2"] = 0;
                ctx["r3"] = 0;

                list.Add(ctx);
            }

            return ExecutionContext.LinkContexts(list);

        }

        //Tabular data

        public ExecutionContext CreateContextWithTabularData_10k() {

            var ctx = new ExecutionContext();
            ctx.Data.AddColumn("Num");
            ctx.Data.AddColumn("Quantity");
            ctx.Data.AddColumn("Value");
            ctx.Data.AddColumn("Result");

            Random rand = new Random();

            for (int i = 0; i < 10000; i++) {
                ctx.Data.AddRow();
                ctx.Data[i, "Num"] = i;
                ctx.Data[i, "Quantity"] = rand.Next(1, 100);
                ctx.Data[i, "Value"] = rand.NextDouble() * 1000;
                ctx.Data[i, "Result"] = 0;

            }

            return ctx;

        }

        [Fact(DisplayName = "Tabular Calculation (All)")]
        public void TabularData1() {

            string expression = "var1 = { 1 + 1 } $Result = { $Quantity * $Value + var1}";
            var ctx = CreateContextWithTabularData_10k();

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC, ctx, null);

            for (int i = 0; i < ret.ExecutionContext.Data.Count; i++) {

                double result = (double)ret.ExecutionContext.Data[i, "Result"];
                double quant = (double)Convert.ChangeType(ret.ExecutionContext.Data[i, "Quantity"], typeof(double));
                double value = (double)ret.ExecutionContext.Data[i, "Value"];
                Assert.Equal(quant * value + 2, result);

            }

        }

        [Fact(DisplayName = "Tabular Calculation (Range)")]
        public void TabularData2() {

            string expression = "$Result[2:100] = { $Quantity * $Value }";
            var ctx = CreateContextWithTabularData_10k();

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC, ctx, null);

            for (int i = 0; i < ret.ExecutionContext.Data.Count; i++) {

                if (i >= 2 && i <= 100) {

                    double result = (double)ret.ExecutionContext.Data[i, "Result"];
                    double quant = (double)Convert.ChangeType(ret.ExecutionContext.Data[i, "Quantity"], typeof(double));
                    double value = (double)ret.ExecutionContext.Data[i, "Value"];
                    Assert.Equal(quant * value, result);
                } else {
                    int result = (int)ret.ExecutionContext.Data[i, "Result"];
                    Assert.Equal(0, result);
                }

            }

        }

        [Theory(DisplayName = "Tabular Calculation (Sum Range)")]
        [InlineData("@Result = { MySum($Num[1:4]) }")]
        [InlineData("@Result = { MySum(Num[1:4]) }")]
        [InlineData("@Result = { MySum(Num1:Num4) }")]
        [InlineData("@Result = { MySum($Num1:$Num4) }")]
        [InlineData("@Result = { MySum($Num[1]:$Num[4]) }")]
        public void TabularData3(string expression) {

            Dictionary<string, MethodInfo> builtinMethods = new Dictionary<string, MethodInfo>();
            builtinMethods.Add("MySum", (new dSUM(Sum)).Method);

            var ctx = CreateContextWithTabularData_10k();

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC, ctx, builtinMethods);
            Assert.Equal(1 + 2 + 3 + 4, (double)ctx["Result"]);

        }

        delegate double dSUM(ContextTable.Range range);
        private static Double Sum(ContextTable.Range range) {
            double result = 0;
            foreach (var o in range.GetValues()) {
                result += (double)(Convert.ChangeType(o, typeof(double)));
            }
            return result;
        }


        [Theory(DisplayName = "If Then Else")]
        //True
        [InlineData("=IF(2>1,10,20)")]
        [InlineData("=IF(1.0 < 2.0,10,20)")]
        [InlineData("=IF(2>1) THEN 10 ELSE 20 END")]
        [InlineData("=IF(2>1 AND 23.1 > 23) THEN 10 ELSE 20 END")]
        [InlineData("=IF(AND(2>1,23.1 > 23),10,20)")]
        //false
        [InlineData("=IF(2<1,20,10)")]
        [InlineData("=IF(1.0 > 2.0,20,10)")]
        [InlineData("=IF(2<1) THEN 20 ELSE 10 END")]
        [InlineData("=IF(2<1 AND 23.1 > 23) THEN 20 ELSE 10 END")]
        [InlineData("=IF(AND(2<1,23.1 > 23),20,10)")]

        [InlineData("=IF(\"A\" = \"A\", 10, 0)")]
        [InlineData("=IF(10.2 = \"10.2\", 10, 0)")]
        [InlineData("=IF(10.3 > \"10.2\", 10, 0)")]
        [InlineData("=IF(10.1 < \"10.2\", 10, 0)")]
        [InlineData("=IF(\"10.3\" > 10.2, 10, 0)")]
        [InlineData("=IF(\"10.1\" < 10.2, 10, 0)")]
        public void IfThenElse1(string expression) {
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(10.0, ret.Value);

        }

        [Fact(DisplayName = "Assigning a tabular $var without tabular data")]
        public void TabularError1() {

            var expression = "$va1 = { 10 }";
            var parser = new rLangParser();
            parser.Setup();
            try {
                var ret = parser.Parse(expression, ParserBackend.EXEC);
            } catch (VariableNotFoundException) {
                Assert.True(true);
            } catch (VariableOutOfRangeException) {
                Assert.True(true);
            }
            
        } 

    }
}
