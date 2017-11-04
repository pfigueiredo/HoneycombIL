using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RTimeSheetCalculator.Controllers.Api.Test
{
    public class DailyTimesheetController : ApiController
    {

        #region static test Data

        private static string[] _excludedColumns = new string[] {
            "GrupoTrabalho", "EXTRANETUSERID", "BWSEMPLOYEENAME",
            "BWSSOCIETYNAME", "SCHNAME", "SCHID", "TENANT_ID",
            "UPDATEDBYCOORDINATOR", "UPDATEDBYCOORDINATORWHEN",
            "OLDTIMESHEETID"
        };

        private static object _locker = new object();
        private static List<ExpandoObject> _data = null;

        public static List<ExpandoObject> ReadTestFile() {

            List<ExpandoObject> ret = new List<ExpandoObject>();
            List<string> columns = null;

            string filepath = System.Web.HttpContext.Current.Server.MapPath(
                    "~/App_Data/TimesheetsDailyMyRandstad.csv"
                );

            StreamReader reader = new StreamReader(File.OpenRead(filepath));

            string line = null;

            while ((line = reader.ReadLine()) != null) {

                var tokens = line.Split(';');

                //read headers
                if (columns == null) {
                    columns = new List<string>();
                    columns.AddRange(tokens);
                } //read data
                else {

                    var obj = new ExpandoObject();
                    var data = obj as IDictionary<string, object>;

                    for (int c = 0; c < columns.Count; c++) {

                        if (_excludedColumns.Contains(columns[c])) continue;

                        if (c < tokens.Length) {
                            data[columns[c]] = tokens[c];
                        } else {
                            data[columns[c]] = null;
                        }
                    }

                    ret.Add(obj);
                }

            }

            return ret;

        }

        public static List<ExpandoObject> GetData() {

            if (_data == null) {
                lock (_locker) {
                    if (_data == null) {
                        _data = ReadTestFile();
                    }
                }
            }

            return _data;

        }
        #endregion

        [Route("api/TestData/{bwsEmployeeCode}/{bwsSocId}")]
        public List<ExpandoObject> GetTimeSheetData(string bwsEmployeeCode, string bwsSocId) {

            var data = GetData();


            var q = (from dynamic obj in data
                    where obj.BWSEMPLOYEECODE == bwsEmployeeCode
                        && obj.BWSSOCIETYID == bwsSocId
                     select obj).Cast<ExpandoObject>().ToList();


            return q;

        }



    }
}
