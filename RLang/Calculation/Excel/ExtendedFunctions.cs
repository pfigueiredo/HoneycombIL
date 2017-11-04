using RLang.Calculation.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLang.Calculation.Excel {
    public class ExtendedFunctions {

        private ExtendedFunctions() { }

        #region datetime
        [BuiltinFunction]
        public static double DATEDIFF(DateTime start, DateTime end) {
            var ts = end.Subtract(start);
            return ts.TotalDays;
        }

        [BuiltinFunction]
        public static DateTime ADDSECONDS(double seconds, DateTime date) {
            return date.AddSeconds(seconds);
        }

        [BuiltinFunction]
        public static DateTime ADDMINUTES(double minutes, DateTime date) {
            return date.AddSeconds(minutes);
        }

        [BuiltinFunction]
        public static DateTime ADDHOURS(double hours, DateTime date) {
            return date.AddSeconds(hours);
        }

        [BuiltinFunction]
        public static DateTime ADDDAYS(double days, DateTime date) {
            return date.AddDays(days);
        }

        [BuiltinFunction]
        public static DateTime ADDMONTHS(double months, DateTime date) {
            return date.AddMonths((int)months);
        }

        [BuiltinFunction]
        public static DateTime ADDYEARS(double years, DateTime date) {
            return date.AddYears((int)years);
        }
        #endregion

        #region Ranges

        [BuiltinFunction]
        public static ContextTable.Range DISTINCT(ContextTable.Range range) {
            return new ContextTable.DistinctRange(range);
        }

        [BuiltinFunction]
        public static ContextTable.Range DISTINCTIF(ContextTable.Range range, object criteria) {
            return new ContextTable.DistinctRange(new ContextTable.WhereRange(range, Predicates.CheckCriteria(criteria)));
        }

        [BuiltinFunction]
        public static ContextTable.Range RANGEIF(ContextTable.Range range, object criteria) {
            return new ContextTable.WhereRange(range, Predicates.CheckCriteria(criteria));
        }

        [BuiltinFunction]
        public static ContextTable.Range RANGEIF(ContextTable.Range range, object criteria, ContextTable.Range returnRange) {

            var r1 = range.GetValues().GetEnumerator();
            var r2 = returnRange.GetValues().GetEnumerator();
            List<object> values = new List<object>();
            var objCriteria = new Predicates.Criteria(criteria);

            if (r1 != null && r2 != null) {
                do {

                    var valR1 = r1.Current;
                    var valR2 = r2.Current;

                    if (objCriteria.Check(valR1) && valR2 != null) {
                        values.Add(valR2);
                    }

                    if (!r2.MoveNext()) break;

                } while (r1.MoveNext());
            }

            return new ContextTable.AdHocRange(returnRange, values);

        }

        [BuiltinFunction]
        public static object FIRST(ContextTable.Range range) {
            return range.GetValues().FirstOrDefault();
        }

        [BuiltinFunction]
        public static object LAST(ContextTable.Range range) {
            return range.GetValues().LastOrDefault();
        }

        [BuiltinFunction]
        public static ContextTable.Range TAKE(double count, ContextTable.Range range) {
            return new ContextTable.AdHocRange(range,  range.GetValues().Take((int)count));
        }

        [BuiltinFunction]
        public static ContextTable.Range TOP(double count, ContextTable.Range range) {
            return new ContextTable.AdHocRange(range, range.GetValues().Take((int)count));
        }

        [BuiltinFunction]
        public static ContextTable.Range BOTTOM(double count, ContextTable.Range range) {
            return new ContextTable.AdHocRange(range, range.GetValues().Reverse().Take((int)count));
        }

        [BuiltinFunction]
        public static ContextTable.Range SKIP(double count, ContextTable.Range range) {
            return new ContextTable.AdHocRange(range, range.GetValues().Skip((int)count));
        }

        [BuiltinFunction]
        public static ContextTable.Range REVERSE(ContextTable.Range range) {
            return new ContextTable.AdHocRange(range, range.GetValues().Reverse());
        }

        [BuiltinFunction]
        public static ContextTable.Range ASC(ContextTable.Range range) {
            return new ContextTable.AdHocRange(range, range.GetValues().OrderBy((o) => o));
        }

        [BuiltinFunction]
        public static ContextTable.Range DESC(ContextTable.Range range) {
            return new ContextTable.AdHocRange(range, range.GetValues().OrderByDescending((o) => o));
        }

        [BuiltinFunction]
        public static bool ANY(ContextTable.Range range) {
            return range.GetValues().Any();
        }

        [BuiltinFunction]
        public static double AVERAGE(ContextTable.Range range) {
            double sum = 0; int count = 0;
            foreach (var value in range.GetValues()) {
                double v = ExecutionContext.ChangeType<Double>(value, double.NaN);
                if (!double.IsNaN(v)) {
                    sum += v;
                    count++;
                }
            }
            return sum / count;
        }

        [BuiltinFunction]
        public static object LOOKUP(object value, ContextTable.Range lookupRange, ContextTable.Range resultRange) {

            var r1 = lookupRange.GetValues().GetEnumerator();
            var r2 = resultRange.GetValues().GetEnumerator();

            if (r1 != null && r2 != null) {
                do {

                    var valR1 = r1.Current;
                    var valR2 = r2.Current;

                    if (ExecutionContext.IsEqual(value, valR1)) {
                        return valR2;
                    }

                    if (!r2.MoveNext()) break;

                } while (r1.MoveNext());
            }

            return null;
        }

        [BuiltinFunction]
        public static object VLOOKUP(object value, ContextTable.Range range, double column) {
            var rArr = range.ToArray() as object[,];
            if (column > 0 && rArr != null) {
                if (rArr.GetLength(0) <= column) {
                    for (int r = 0; r < rArr.GetLength(1); r++) {
                        object lObj = rArr[0, r];
                        if (ExecutionContext.IsEqual(value, lObj)) {
                            return rArr[(int)column - 1, r];
                        }
                    }
                }
            }
            return null;
        }

        #endregion


    }
}
