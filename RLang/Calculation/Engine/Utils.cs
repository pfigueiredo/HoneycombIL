using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RLang.Calculation.Engine {
    public class LexicalError {
        int line;
        int column;
        string token;

        public int Line { get { return line; } }
        public int Column { get { return column; } }
        public string Token { get { return token; } }

        public LexicalError(int line, int column, string token) {
            this.line = line;
            this.column = column;
            this.token = token;
        }

        public override string ToString() {
            return string.Format(
                "Lexical error in line {0} column {1} near {2}",
                line, column, token
            );
        }

    }

    public class SyntaxError {
        int line;
        int column;
        string token;
        string expectedTokens;

        public int Line { get { return line; } }
        public int Column { get { return column; } }
        public string Token { get { return token; } }
        public string ExpectedTokens { get { return expectedTokens; } }

        public SyntaxError(int line, int column, string token, string expectedTokens) {
            this.line = line;
            this.column = column;
            this.token = token;
            this.expectedTokens = expectedTokens;
        }

        public override string ToString() {
            return string.Format(
                "Syntax error in line {0} column {1} near {2} expected {3}",
                line, column, token, expectedTokens
            );
        }
    }

    [Serializable()]
    public class SymbolException : System.Exception {
        public SymbolException(string message)
            : base(message) {
        }

        public SymbolException(string message,
            Exception inner)
            : base(message, inner) {
        }

        protected SymbolException(SerializationInfo info,
            StreamingContext context)
            : base(info, context) {
        }

    }

    [Serializable()]
    public class RuleException : System.Exception {

        public RuleException(string message)
            : base(message) {
        }

        public RuleException(string message,
                             Exception inner)
            : base(message, inner) {
        }

        protected RuleException(SerializationInfo info,
                                StreamingContext context)
            : base(info, context) {
        }

    }

    [Serializable()]
    public class FunctionArgumentsLenghtException : System.Exception {

        public int NumOfArgs { get; set; }
        public int ExpectedNumOfArgs { get; set; }
        public string FunctionName { get; set; }

        public FunctionArgumentsLenghtException(int numOfArgs, int expectedNumOfArgs, string functionName)
            : base(string.Format("{0} expects {1} number of arguments", functionName, expectedNumOfArgs)) {

            this.NumOfArgs = numOfArgs;
            this.ExpectedNumOfArgs = expectedNumOfArgs;
            this.FunctionName = functionName;

        }
    }

    [Serializable()]
    public class FunctionNotFoundException : System.Exception {

        public string FunctionName { get; set; }

        public FunctionNotFoundException(string functionName)
            : base(string.Format("function {0} was not found", functionName)) {

            this.FunctionName = functionName;

        }
    }

    [Serializable()]
    public class VariableOutOfRangeException : System.Exception {

        public string Value { get; set; }
        public string Range { get; set; }

        public VariableOutOfRangeException(string value, string range)
            : base(string.Format("The Variable/Location '{0}' is out of the range '{1}'", value, range)) {
            this.Value = value;
        }
    }

    [Serializable()]
    public class VariableNotFoundException : System.Exception {

        public string Value { get; set; }

        public VariableNotFoundException(string value)
            : base(string.Format("The Variable/Location '{0}' was not found", value)) {
            this.Value = value;
        }
    }

    [Serializable()]
    public class InvalidComplexSymbolException : System.Exception {

        public string Symbol { get; set; }

        public InvalidComplexSymbolException(string symbol)
            : base(string.Format("the complex simbol {0} as some invalid parameters", symbol)) {
            this.Symbol = symbol;
        }
    }

    [Serializable()]
    public class InvalidGlobalSymbolException : System.Exception {

        public string Symbol { get; set; }

        public InvalidGlobalSymbolException(string symbol)
            : base(string.Format("the global simbol {0} as an invalid declaration", symbol)) {
            this.Symbol = symbol;
        }

    }

    [Serializable()]
    public class LexicalErrorException : System.Exception {

        public LexicalError Error { get; set; }

        public LexicalErrorException(LexicalError error)
            : base(string.Format("Error at line {0} column {1} near {2}", error.Line, error.Column, error.Token)) {

            this.Error = error;

        }
    }

    [Serializable()]
    public class SyntaxErrorException : System.Exception {

        public SyntaxError Error { get; set; }

        public SyntaxErrorException(SyntaxError error)
            : base(string.Format("Error at line {0} column {1} near {2}", error.Line, error.Column, error.Token)) {

            this.Error = error;

        }
    }
}
