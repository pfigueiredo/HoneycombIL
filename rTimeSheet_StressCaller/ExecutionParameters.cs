using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rTimeSheet_StressCaller {

    public class ExpressionDefinition {
        public string Variable { get; set; }
        public string Expression { get; set; }
        public int Order { get; set; }
    }

    public class ExecutionContextData {
        public List<ExpandoObject> Data { get; set; }
        public ExpandoObject Variables { get; set; }
        public bool DebugExecutionResult { get; set; }
    }

    public class ExecutionParameters {
        public List<ExpressionDefinition> Expressions { get; set; }
        public List<ExecutionContextData> Context { get; set; }
    }
}
