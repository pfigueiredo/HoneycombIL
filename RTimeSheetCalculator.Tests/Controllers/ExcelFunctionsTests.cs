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
    public class ExcelFunctionsTests {

        [Fact(DisplayName = "Excel ROUND")]
        public void Round() {
            var parser = new rLangParser();
            parser.Setup();
            string expression = "=ROUND(12.52, 1)";
            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(12.5, ret.Value);
        }


        [Fact(DisplayName = "Named Range / Range Variable")]
        public void NamedRange() {

            var expression = "$P[0:9] = { 1 } Range1 = { P:P } Result = { SUM(P:P) + SUM(Range1)}";
            var parser = new rLangParser();
            parser.Setup();

            var ret = parser.Parse(expression, ParserBackend.EXEC);
            Assert.Equal(20.0, ret.Value);

        }


    }
}
