using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RLang.Calculation.Engine {
    class CLRFunction {

        static object _locker = new object();
        static List<FunctionDefinition> builtInFunctions = null;

        public static List<FunctionDefinition> GetBuiltinFunctionDefinitions() {

            if (builtInFunctions == null) {
                lock (_locker) {
                    if (builtInFunctions == null) {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        builtInFunctions = BuildFunctionDefinitions(assembly);
                    }
                }
            }

            return builtInFunctions;
            
        }

        public static List<FunctionDefinition> BuildFunctionDefinitions(Assembly assembly) {
            List<FunctionDefinition> ret = new List<FunctionDefinition>();

            foreach (Type t in assembly.GetTypes()) {
                foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
                    var attr = m.GetCustomAttribute(typeof(BuiltinFunctionAttribute)) as BuiltinFunctionAttribute;
                    if (attr != null) {
                        string definition = (string.IsNullOrWhiteSpace(attr.FunctionName)) ? m.Name : attr.FunctionName;
                        var fnx = FunctionDefinition.FromMethod(definition, m);
                        fnx.UseAsConstant = attr.IsConstant;
                        ret.Add(fnx);
                    }
                }
            }
            return ret;
        }

    }
}
