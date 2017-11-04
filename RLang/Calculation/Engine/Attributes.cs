using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLang.Calculation.Engine {

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class BuiltinFunctionAttribute : Attribute {

        public string FunctionName { get; set; }
        public bool IsConstant { get; set; }

        public BuiltinFunctionAttribute() { }

    }


}
