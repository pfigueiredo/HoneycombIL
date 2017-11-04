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
    public class Bugs {


        [Theory(DisplayName = "Bug1: Row position not reseting")]
        [InlineData("$c[0:10] = { 2 } @R = { $c }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[0] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[1] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[2] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[3] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[4] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[5] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[6] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[7] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[8] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[9] }")]
        [InlineData("$c[0:10] = { 2 } @R = { $c[10] }")]
        public void Bug1(string expression) {
            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(2.0, ret.Value);
        }


        [Fact(DisplayName = "Bug Overloading functions")]
        public void Bug2() {

            var expression = "@P1 = { DATE() } @P2 = { DATE(2003,1,1) }";

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(new DateTime(2003, 1, 1), ret.Value);


        }

        [Fact(DisplayName = "Constant Functions Called as constants but assumed as variables")]
        public void Bug3() {

            var expression = "@PI = { PI }";

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(Math.PI, ret.Value);

        }

        [Fact(DisplayName = "Bug: Incorrect cast to double")]
        public void Bug4() {

            var expression = "$A[0:10] = {IF($row= \"aaa\",1,\"NO\")} @R = {$A[0]}";

            var parser = new rLangParser();
            parser.Setup();
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal("NO", ret.Value);

        }



    }
}
