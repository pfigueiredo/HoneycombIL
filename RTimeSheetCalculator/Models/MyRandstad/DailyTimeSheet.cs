using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RLang.Calculation.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTimeSheetCalculator.Models.MyRandstad {
    public class DailyTimeSheet {

        public string Company { get; set; }
        public string Employee { get; set; }
        public DateTime Date { get; set; }

        [JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;

        public void AddToExecutionContext (ExecutionContext context) {

            lock (context.ContextLocker) {

                int currRow = context.Data.Count;
                context.Data.AddRow();

                context.Data[currRow, "Empresa"] = this.Company;
                context.Data[currRow, "Colaborador"] = this.Employee;
                context.Data[currRow, "Data"] = this.Date;

                foreach (var key in _additionalData.Keys) {
                    var jtoken = _additionalData[key].ToObject(typeof(object));
                    context.Data[currRow, key] = jtoken;
                }

            }


        }


    }
}
