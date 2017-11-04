using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RLang.Calculation.Engine {

    //BOOLEAN_CONTEXT is needed if one day we need to translate do TSQL... :D lalala
    public enum ExpressionType {
        NONE, BOOLEAN, NUMBER, DATE, STRING, VARIANT, ANY, BOOLEAN_CONTEXT, TABLE, REF, ERROR
    }

    public enum IdentifierSymbolType {
        Variable,
        GlobalVariable,
        RangeVariable,
        CellVariable,
        Function
    }

    public class FunctionDefinition {
        public string Definition { get; set; }
        public int NumParams { get; set; }
        public MethodInfo Method { get; set; }
        public List<ExpressionType> ParamTypes { get; private set; }
        public ExpressionType ReturnType { get; set; }
        public bool UseAsConstant { get; set; }
        public bool InjectContext { get; set; }

        private FunctionDefinition() { }
        private FunctionDefinition(string definition, MethodInfo method, bool useAsConstant) {

            var returnType = method.ReturnType;
            var parameters = method.GetParameters();

            this.Definition = definition;
            this.Method = method;
            this.NumParams = 0;
            this.ReturnType = ExecutionUtils.ExpressionFromCLRType(returnType);
            this.UseAsConstant = useAsConstant;

            this.ParamTypes = new List<ExpressionType>();

            for (int i = 0; i < parameters.Length; i++) {

                if (parameters[i].ParameterType == typeof(ExecutionContext)) {
                    if (i == 0) this.InjectContext = true;
                    else
                        throw new ArgumentException(
                            string.Format("To accept the execution context in the external function '{0}', the function must accept the context has the first parameter.", 
                            method.Name)
                        );
                } else {
                    this.NumParams++;
                    this.ParamTypes.Add(ExecutionUtils.ExpressionFromCLRType(
                        parameters[i].ParameterType
                    ));
                }
            }
        }

        public static FunctionDefinition FromMethod(string definition, MethodInfo method) {
            return new FunctionDefinition(definition, method, false);
        }

        public static FunctionDefinition FromMethod(string definition, MethodInfo method, bool useAsConstant) {
            return new FunctionDefinition(definition, method, useAsConstant);
        }

    }

    public class IdentifierSymbol {
        public string Definition { get; set; }
        public string PrivateDefinition { get; set; }
        public string FunctionName { get; set; }

        public System.Linq.Expressions.Expression LinqExpression { get; set; }

        public IdentifierSymbolType IdentifierType { get; set; }

        public bool IsExecutionContextVariable { get { return IdentifierType == IdentifierSymbolType.GlobalVariable; } }
        public bool IsFunction { get { return IdentifierType == IdentifierSymbolType.Function; } }
        public bool IsValid { get; set; }
        public bool IsReturn { get { return IdentifierType == IdentifierSymbolType.GlobalVariable; } }
        public bool IsGlobal { get { return IdentifierType == IdentifierSymbolType.GlobalVariable; } }
        public bool IsExternParameter { get; set; }
        public bool IsConstantFunction { get; set; }

        public string ObjectProperty { get; set; }
        public string ObjectName { get; set; }

        public class CellPosition {
            public string Column { get; set; }
            public int Row { get; set; }
            public bool IsRelative { get; set; }
        }

        public class Postion {
            public int Index { get; set; }
            public bool IsRelative { get; set; }
        }

        public CellPosition StartPosition { get; set; }
        public CellPosition EndPosition { get; set; }
        public int? Row { get; set; }
        public bool IsRelative { get; set; }

        public bool FxArgsMissmatch { get; set; }

        public SymbolTable SymbolTable { get; set; }
        public ExpressionType ExpressionType { get; set; }

        public int NumOfArguments { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        
        public FunctionDefinition GetFunctionDefinition() {
            return SymbolTable.GetFunctionDefinition(this.FunctionName, this.NumOfArguments);
        }
        public FunctionDefinition GetFunctionDefinition(string name, int numOfArguments) {
            return SymbolTable.GetFunctionDefinition(name, numOfArguments);
        }

        public void ValidateFunctionDefinition() {
            int status = SymbolTable.ValidateFunctionDefinition(this.FunctionName, this.NumOfArguments);
            switch (status) {
                case 0: this.IsValid = true; FxArgsMissmatch = false; break;
                case 1: this.IsValid = true; FxArgsMissmatch = true; break;
                case 2: this.IsValid = false; FxArgsMissmatch = false; break;
            }
        }

        private void FillObjectNameNProperty(string definition) {
            var tokens = definition.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 1) {
                this.ObjectProperty = "Value";
                this.ObjectName = tokens[0];
            } else if (tokens.Length == 2) {
                this.ObjectProperty = tokens[1];
                this.ObjectName = tokens[0];
            } else if (tokens.Length > 2) {
                this.ObjectProperty = tokens[tokens.Length -1];
                this.ObjectName = string.Join("_", tokens, 0, tokens.Length -1);
            }
        }

        public static IdentifierSymbol CreateSymbol(string privateDefinition, string definition, IdentifierSymbolType identifierType, int? row, bool isRelative, SymbolTable symbolTable, int line, int column) {
            return new IdentifierSymbol(privateDefinition, definition, identifierType, row, isRelative, null, null, symbolTable, line, column);
        }

        public static IdentifierSymbol CreateSymbol(string privateDefinition, string definition, CellPosition startPosition, CellPosition endPosistion, SymbolTable symbolTable, int line, int column) {
            return new IdentifierSymbol(privateDefinition, definition, IdentifierSymbolType.RangeVariable, null, false, startPosition, endPosistion, symbolTable, line, column);
        }

        private IdentifierSymbol(
            string privateDefinition,
            string definition, 
            IdentifierSymbolType identifierType,
            int? row,
            bool isRelative,
            CellPosition startPosition, CellPosition endPosistion,
            SymbolTable symbolTable,
            int line, int column
        ) {

            this.IdentifierType = identifierType;
            this.StartPosition = startPosition;
            this.EndPosition = endPosistion;

            if (this.IdentifierType == IdentifierSymbolType.GlobalVariable) {
                this.IsValid = true;
                this.ExpressionType = ExpressionType.ANY;
                FillObjectNameNProperty(definition);
            } else if (this.IdentifierType == IdentifierSymbolType.RangeVariable) {
                this.IsValid = true;
                this.ExpressionType = ExpressionType.TABLE;
            } else {
                this.ExpressionType = ExpressionType.ANY;
            }

            this.SymbolTable = symbolTable;

            foreach (var function in this.SymbolTable.AvailableFunctions) {
                if (function.Definition.Equals(definition, StringComparison.InvariantCultureIgnoreCase)) {
                    if (function.UseAsConstant && identifierType == IdentifierSymbolType.Variable) { //can be constant function
                        // I will not change the type of identifier because i don't know the it is in the expression
                        // It will be handled in the Expression to IL converter depending on the position
                        this.IsValid = true;
                        this.IsConstantFunction = true; //IsConstantFunction works as an hint
                        this.ExpressionType = function.ReturnType; //be carefull about variables that have the same definition as constant functions...
                    } else if (identifierType == IdentifierSymbolType.Function) { //ok expecting to find a match
                        this.ExpressionType = function.ReturnType;
                        this.IdentifierType = IdentifierSymbolType.Function;
                        this.IsValid = true;
                    }
                }
            }

            if (IsFunction) {;
                this.FunctionName = definition;
            } else if (IsConstantFunction) {
                this.FunctionName = definition;
            }

            this.PrivateDefinition = privateDefinition;
            this.Definition = definition;

            this.Line = line;
            this.Column = column;
            this.Row = row;
            this.IsRelative = isRelative;

            symbolTable.AddSymbol(this);

        }


        public static string GetPrivateDefinition(string definition, IdentifierSymbolType type, int? row, int numOfArguments, bool isRealtive) {
            if (type == IdentifierSymbolType.CellVariable) {
                return string.Format("{0}_t{1}r{2}{3}", definition.ToUpper(), (int)type, (row == null) ? 0 : row, (isRealtive) ? "r" : "a");
            } else if (type == IdentifierSymbolType.GlobalVariable) {
                return definition.ToUpper();
            } else if (type == IdentifierSymbolType.Function) { 
                return string.Format("{0}_t{1}_{2}", definition.ToUpper(), (int)type, numOfArguments);
            } else
                return string.Format("{0}_t{1}", definition.ToUpper(), (int)type);
        }

        public static string GetPrivateDefinition(string definitionStart, 
            int rangeStart, bool relativeStart, string definitionEnd, int rangeEnd, bool relativeEnd
        ) {
            return string.Format(
                "{0}[{1}]:{2}[{3}]t{4}", 
                definitionStart.ToUpper(), 
                string.Format("{0}{1}", (relativeStart) ? "r" : "", rangeStart), 
                definitionEnd.ToUpper(),
                string.Format("{0}{1}", (relativeEnd) ? "r" : "", rangeEnd),
                IdentifierSymbolType.RangeVariable
            );
        }



    }



    public class SymbolTable : MarshalByRefObject {

        private List<FunctionDefinition> availableFunctions = new List<FunctionDefinition>();
        public List<FunctionDefinition> AvailableFunctions {  get { return availableFunctions; } }

        private Dictionary<string, IdentifierSymbol> symbols = new Dictionary<string, IdentifierSymbol>();

        public SymbolTable(Dictionary<string, MethodInfo> builtInMethods) {

            foreach (var fnx in CLRFunction.GetBuiltinFunctionDefinitions()) {
                availableFunctions.Add(fnx);
            }

            if (builtInMethods != null) {
                foreach (var key in builtInMethods.Keys) {
                    availableFunctions.Add(FunctionDefinition.FromMethod(key, builtInMethods[key]));
                }
            }

        }

        public IEnumerable<IdentifierSymbol> Symbols {
            get {
                foreach (var s in this.symbols.Values)
                    yield return s;
            }
        }

        public FunctionDefinition GetFunctionDefinition(string name, int numOfArguments) {
            string strReturn = name;
            FunctionDefinition foundFunction = null;

            foreach (FunctionDefinition fx in this.availableFunctions)
                if (fx.Definition.ToUpper() == name.ToUpper()) {
                    foundFunction = fx;
                    if (fx.NumParams == numOfArguments)
                        return fx;
                }

            if (foundFunction != null) //foi encontrado mas com um numero diferente de argumentos
                throw new FunctionArgumentsLenghtException(
                    numOfArguments, foundFunction.NumParams, foundFunction.Definition
                );
            else
                throw new FunctionNotFoundException(name);

        }

        public int ValidateFunctionDefinition(string name, int numOfArguments) {

            string strReturn = name;
            FunctionDefinition foundFunction = null;

            foreach (FunctionDefinition fx in this.availableFunctions)
                if (fx.Definition.ToUpper() == name.ToUpper()) {
                    foundFunction = fx;
                    if (fx.NumParams == numOfArguments)
                        return 0;
                }

            if (foundFunction != null) //foi encontrado mas com um numero diferente de argumentos
                return 1;
            else
                return 2; //Não foi encontrado vou deixar que dê erro depois no SQL se for caso disso
        }

        public SymbolTable CreateLocalSymbolTable() {
            return this; //TODO: implement scopes when needed;
        }

        public void AddSymbol(IdentifierSymbol symbol) {
            string prvDef = symbol.PrivateDefinition;
            if (!this.symbols.ContainsKey(prvDef)) this.symbols.Add(prvDef, symbol);
        }

        public IdentifierSymbol GetSymbol(string definitionStart, int rangeStart, bool relativeStart, string definitionEnd, int rangeEnd, bool relativeEnd, int line, int column) {
            string prvDef = IdentifierSymbol.GetPrivateDefinition(definitionStart, rangeStart, relativeStart, definitionEnd, rangeEnd, relativeEnd);
            if (this.symbols.ContainsKey(prvDef)) return this.symbols[prvDef];

            var startPosition = new IdentifierSymbol.CellPosition() { Column = definitionStart, Row = rangeStart, IsRelative = relativeStart };
            var endPosition = new IdentifierSymbol.CellPosition() { Column = definitionEnd, Row = rangeEnd, IsRelative = relativeEnd };

            return IdentifierSymbol.CreateSymbol(prvDef, prvDef, startPosition, endPosition, this, line, column);
        }

        public IdentifierSymbol GetSymbol(string definition, int rangeStart, bool relativeStart, int rangeEnd, bool relativeEnd, int line, int column) {

            string prvDef = IdentifierSymbol.GetPrivateDefinition(definition, rangeStart, relativeStart, definition, rangeEnd, relativeEnd);
            if (this.symbols.ContainsKey(prvDef)) return this.symbols[prvDef];

            var startPosition = new IdentifierSymbol.CellPosition() { Column = definition, Row = rangeStart, IsRelative = relativeStart };
            var endPosition = new IdentifierSymbol.CellPosition() { Column = definition, Row = rangeEnd, IsRelative = relativeEnd };


            return IdentifierSymbol.CreateSymbol(prvDef, definition, startPosition, endPosition, this, line, column);
        }

        public IdentifierSymbol GetSymbol(string definition, IdentifierSymbolType identifierType, int? row, int numOfArguments, bool isRelative, int line, int column) {

            string prvDef = IdentifierSymbol.GetPrivateDefinition(definition, identifierType, row, numOfArguments, isRelative);
            if (this.symbols.ContainsKey(prvDef)) return this.symbols[prvDef];

            return IdentifierSymbol.CreateSymbol(prvDef, definition, identifierType, row, isRelative, this, line, column);

        }

        public string GetExternalPropertyName(string definition) { return definition; }


    }

}
