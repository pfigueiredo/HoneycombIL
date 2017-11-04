using RLang.Calculation.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RTimeSheetCalculator.Controllers.Api.Calculation.Engine
{
    public class SyntaxController : ApiController {

        public class ReturnResult {
            public bool Ok { get; set; }
            public string Message { get; set; }
            public object Result { get; set; }
        }

        public class ExpressionBag {
            public string Expression { get; set; }
            public bool Execute { get; set; }
        }

        [Route("api/Syntax/Check/"), HttpPost]
        public ReturnResult CheckSyntax ([FromBody]ExpressionBag eBag)
        {
            return ParseOrExecExpression(eBag, ParserBackend.PARSE);
        }

        [Route("api/Syntax/Exec/"), HttpPost]
        public ReturnResult ExecSyntax([FromBody]ExpressionBag eBag)
        {
            return ParseOrExecExpression(eBag, ParserBackend.EXEC);
        }

        [Route("api/Syntax/Assembly/"), HttpPost]
        public HttpResponseMessage GetAssembly (ExpressionBag bag) {

            

            HttpResponseMessage result = new HttpResponseMessage();

            rLangParser parser = new rLangParser();
            parser.Setup();

            ParserBackend backend = ParserBackend.IL;
            var ret = parser.Parse(bag.Expression, backend);

            if (ret.Assembly != null) {

                var filename = ret.Filename;
                ret.Assembly.Save(filename);

                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(Path.Combine(Path.GetTempPath(), filename), FileMode.Open, FileAccess.Read));
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = filename;

            } else
                result = Request.CreateResponse(HttpStatusCode.Gone);


            return result;

        }

        private ReturnResult ParseOrExecExpression(ExpressionBag eBag,ParserBackend mode)
        {
            ReturnResult result = new ReturnResult();

            rLangParser parser = new rLangParser();
            parser.Setup();

            try
            {
                var ret = parser.Parse(eBag.Expression, mode);
                result.Result = ret;
                result.Ok = true;
                result.Message = string.Format("Ok {0}", ret.Value != null ? string.Format("- {0}", ret.Value) : "");

            }
            catch (RuleException rEx)
            {
                result.Ok = false;
                result.Message = string.Format("Violação de regra: {0}", rEx.Message);
            }
            catch (VariableNotFoundException vnEx)
            {
                result.Ok = false;
                result.Message = string.Format("A variavel/localização '{0}' não foi encontrada", vnEx.Value);
            }
            catch (VariableOutOfRangeException vrEx)
            {
                result.Ok = false;
                result.Message = string.Format("A variavel/localização '{0}' compreendida fora do conjunto {1} ", vrEx.Value, vrEx.Range);
            }
            catch (FunctionArgumentsLenghtException faEx)
            {
                result.Ok = false;
                result.Message = string.Format("A função '{0}' espera {1} argumentos e não {2}", faEx.FunctionName, faEx.ExpectedNumOfArgs, faEx.NumOfArgs);
            }
            catch (FunctionNotFoundException fEx)
            {
                result.Ok = false;
                result.Message = string.Format("A função '{0}' não foi encontrada na lista de funções disponíveis para serem utilizadas na formula", fEx.FunctionName);
            }
            catch (InvalidGlobalSymbolException gsEx)
            {
                result.Ok = false;
                result.Message = string.Format("Symbolo global inválido '{0}'", gsEx.Symbol);
            }
            catch (LexicalErrorException lEx)
            {
                result.Ok = false;
                result.Message = string.Format("Erro ao compilar a expressão na linha '{0}', character '{1}' junto a '{2}' ",
                    lEx.Error.Column, lEx.Error.Column, lEx.Error.Token);
            }
            catch (SyntaxErrorException sEx)
            {
                result.Ok = false;
                result.Message = string.Format("Erro de syntaxe na linha '{0}', character '{1}' junto a '{2}' onde era esperado encontrar '{3}'",
                    sEx.Error.Line, sEx.Error.Column, sEx.Error.Token, sEx.Error.ExpectedTokens);
            }
            catch (Exception ex)
            {
                result.Ok = false;
                result.Message = string.Format("Erro não esperado ao compilar a expressão: ({0})", ex.Message);
            }

            return result;
        }


    }
}
