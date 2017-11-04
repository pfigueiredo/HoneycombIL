using RLang.Calculation.Engine;
using RTimeSheetCalculator.Models.MyRandstad;
using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RTimeSheetCalculator.Models.Engine;
using System.Reflection;
using System.Globalization;

namespace RTimeSheetCalculator.Controllers.Api.Calculation.Engine
{
    public class CalcController : ApiController {

        public class ReturnResult {
            public bool Ok { get; set; }
            public string Message { get; set; }
            public List<ResultItem> Result { get; set; }
            public List<object> Debug { get; set; }
            public TimeSpan ExecutionTime { get; set; }
        }

        public class ResultItem {
            public string Id { get; set; }
            public List<object> ResultItems { get; set; }
        }


        [Route("api/Engine/Calculate/"), HttpPost]
        public ReturnResult CalculateTimeSheet (ExecutionParameters parameters) {

            var start = DateTime.Now;
            string expression = parameters.BuildExpression();

            ReturnResult result = new ReturnResult();

            rLangParser parser = new rLangParser();
            parser.Setup();

            try {

                ParserBackend backend = ParserBackend.IL;
                var ret = parser.Parse(expression, backend, null, null);

                object lastRet = null;

                result.Result = new List<ResultItem>();
                result.Debug = new List<object>();

                //var domain = AppDomain.CreateDomain("ExecutionDomain", null,
                //    new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase });


                MethodInfo execMethod = ret.Main.GetMethod("Exec", BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < parameters.Context.Count; i++) {
                    ExecutionContext context = parameters.Context[i].BuildExecutionContext();

                    lastRet = execMethod.Invoke(null, new object[] { context });

                    var retObj = new ResultItem() { Id = context.ContextId, ResultItems = new List<object>() };

                    foreach (var pair in context.Values) {
                        dynamic itemObj = new ExpandoObject();
                        itemObj.Id = pair.Key;
                        foreach (var kvp in pair.Value) {
                            string key = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(kvp.Key);
                            ((IDictionary<string, object>)itemObj).Add(key, kvp.Value);
                        }
                        
                        retObj.ResultItems.Add(itemObj);
                    }

                    result.Result.Add(retObj);
                    if (parameters.Context[i].DebugExecutionResult) result.Debug.Add(context);

                }
                

                result.Ok = true;
                result.Message = string.Format("Ok {0}", lastRet != null ? string.Format("- {0}", lastRet.ToString()) : "");

            } catch (RuleException rEx) {
                result.Ok = false;
                result.Message = string.Format("Violação de regra: {0}", rEx.Message);
            } catch (VariableNotFoundException vnEx) {
                result.Ok = false;
                result.Message = string.Format("A variavel/localização '{0}' não foi encontrada", vnEx.Value);
            } catch (VariableOutOfRangeException vrEx) {
                result.Ok = false;
                result.Message = string.Format("A variavel/localização '{0}' compreendida fora do conjunto {1} ", vrEx.Value, vrEx.Range);
            } catch (FunctionArgumentsLenghtException faEx) {
                result.Ok = false;
                result.Message = string.Format("A função '{0}' espera {1} argumentos e não {2}", faEx.FunctionName, faEx.ExpectedNumOfArgs, faEx.NumOfArgs);
            } catch (FunctionNotFoundException fEx) {
                result.Ok = false;
                result.Message = string.Format("A função '{0}' não foi encontrada na lista de funções disponíveis para serem utilizadas na formula", fEx.FunctionName);
            } catch (InvalidGlobalSymbolException gsEx) {
                result.Ok = false;
                result.Message = string.Format("Symbolo global inválido '{0}'", gsEx.Symbol);
            } catch (LexicalErrorException lEx) {
                result.Ok = false;
                result.Message = string.Format("Erro ao compilar a expressão na linha '{0}', character '{1}' junto a '{2}' ",
                    lEx.Error.Column, lEx.Error.Column, lEx.Error.Token);
            } catch (SyntaxErrorException sEx) {
                result.Ok = false;
                result.Message = string.Format("Erro de syntaxe na linha '{0}', character '{1}' junto a '{2}' onde era esperado encontrar '{3}'",
                    sEx.Error.Line, sEx.Error.Column, sEx.Error.Token, sEx.Error.ExpectedTokens);
            } catch (Exception ex) {
                result.Ok = false;
                result.Message = string.Format("Erro não esperado ao compilar a expressão: ({0})", ex.Message);
            } finally {

                result.ExecutionTime = DateTime.Now.Subtract(start);

            }

            return result;

        }


    }
}
