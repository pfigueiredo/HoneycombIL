using RLang.Calculation.Engine;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTimeSheetCalculator.Models.Engine {


    public class ExpressionDefinition {
        public string Variable { get; set; }
        public string Expression { get; set; }
        public int Order { get; set; }
    }


    public class ExecutionContextData {
        public List<ExpandoObject> Data { get; set; }
        public ExpandoObject Variables { get; set; }
        public bool DebugExecutionResult { get; set; }
        public string Id { get; set; }

        public ExecutionContext BuildExecutionContext() {

            var context = new ExecutionContext();

            context.ContextId = this.Id;

            foreach (var pair in this.Variables) {
                context.ImportObject(pair.Key, pair.Value);
            }

            for (int row = 0; row < this.Data.Count; row++) {
                foreach (var pair in this.Data[row]) {
                    context.Data[row, pair.Key] = pair.Value;
                }
            }

            return context;
        }

    }

    public class ExecutionParameters {
        public List<ExpressionDefinition> Expressions { get; set; }
        public List<ExecutionContextData> Context { get; set; }

        public string BuildExpression() {
            StringBuilder sBuilder = new StringBuilder();

            foreach (var expression in Expressions.OrderBy(e => e.Order)) {
                sBuilder.AppendFormat("{0}={{{1}}}\n", expression.Variable, expression.Expression);
            }

            return sBuilder.ToString();
        }


    }
}
