using RLang.Calculation.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLang.Calculation.Excel {
    public class Functions {

        [BuiltinFunction]
        public static double ROUND(double value, double digits) { return Math.Round(value, (int)digits); }
        [BuiltinFunction(IsConstant = true)]
        public static DateTime DATE() { return DateTime.Now; }
        [BuiltinFunction]
        public static DateTime DATE(object value) { return ExecutionContext.ChangeType<DateTime>(value); }
        [BuiltinFunction]
        public static DateTime DATE(double year, double month, double day) {
            return new DateTime((int)year, (int)month, (int)day);
        }
        [BuiltinFunction]
        public static DateTime DATE(double year, double month, double day, double hour, double minute, double second) {
            return new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second);
        }
        [BuiltinFunction]
        public static double DATEVALUE(DateTime date) { return date.ToOADate(); }
        [BuiltinFunction]
        public static double DAY(DateTime date) { return date.Day; }

        [BuiltinFunction]
        public static double DAYS360(DateTime xStart_dt, DateTime xEnd_dt) {
            return DAYS360(xStart_dt, xEnd_dt, true);
        }

        private static bool IsLastDayOfFebruary(DateTime date) {
            return date.Month == 2 && date.Day == DateTime.DaysInMonth(date.Year, date.Month);
        }

        [BuiltinFunction]
        public static double DAYS360(DateTime startDate, DateTime endDate, Boolean xEuropean) {
            int StartDay = startDate.Day;
            int StartMonth = startDate.Month;
            int StartYear = startDate.Year;
            int EndDay = endDate.Day;
            int EndMonth = endDate.Month;
            int EndYear = endDate.Year;

            if (StartDay == 31 || IsLastDayOfFebruary(startDate)) {
                StartDay = 30;
            }

            if (StartDay == 30 && EndDay == 31) {
                EndDay = 30;
            }

            return ((EndYear - StartYear) * 360) + ((EndMonth - StartMonth) * 30) + (EndDay - StartDay);
        }

        [BuiltinFunction]
        public static DateTime EDATE(DateTime date, double months) { return date.AddMonths((int)months); }
        [BuiltinFunction]
        public static DateTime EOMONTH(DateTime date) {
            var dref = date.AddMonths(1);
            return new DateTime(dref.Year, dref.Month, 1).AddDays(-1);
        }
        [BuiltinFunction]
        public static DateTime EOMONTH(DateTime date, double months) {
            var dref = date.AddMonths((int)months).AddMonths(1);
            return new DateTime(dref.Year, dref.Month, 1).AddDays(-1);
        }
        [BuiltinFunction]
        public static double HOUR(DateTime date) { return date.Hour; }
        [BuiltinFunction]
        public static double MINUTE(DateTime date) { return date.Minute; }
        [BuiltinFunction]
        public static double MONTH(DateTime date) { return date.Month; }
        [BuiltinFunction]
        public static double NETWORKDAYS() { throw new NotImplementedException(); }
        [BuiltinFunction(IsConstant = true)]
        public static DateTime NOW() { return DateTime.Now; }
        [BuiltinFunction]
        public static double SECOND(DateTime date) { return date.Second; }
        [BuiltinFunction(IsConstant = true)]
        public static double TIME() {
            return DateTime.Now.TimeOfDay.TotalDays;
        }
        [BuiltinFunction]
        public static double TIME(double hours, double minutes, double seconds) {
            TimeSpan ts = new TimeSpan(0, (int)hours, (int)minutes, (int)seconds);
            return ts.TotalDays;
        }
        [BuiltinFunction]
        public static double TIME(object value) {
            DateTime dt = ExecutionContext.ChangeType<DateTime>(value);
            TimeSpan tsTime = dt.TimeOfDay;
            return tsTime.TotalDays;
        }
        [BuiltinFunction]
        public static double TIMEVALUE(string time) {
            DateTime dtTime = DateTime.MinValue;
            DateTime.TryParse(time, out dtTime);
            TimeSpan tsTime = dtTime.TimeOfDay;
            return tsTime.TotalDays;
        }
        [BuiltinFunction(IsConstant = true)]
        public static DateTime TODAY() { return DateTime.Today; }
        [BuiltinFunction]
        public static double WEEKDAY(DateTime date) { return WEEKDAY(date, 1); }
        [BuiltinFunction]
        public static double WEEKDAY(DateTime date, double returnType) { return ((double)date.DayOfWeek - returnType + 7) % 7; }
        [BuiltinFunction]
        public static double WEEKNUM(DateTime date, double dayOfWeek) { return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, (DayOfWeek)dayOfWeek); }
        [BuiltinFunction]
        public static double WORKDAY() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double YEAR(DateTime date) { return date.Year; }
        [BuiltinFunction]
        public static double YEARFRAC() { throw new NotImplementedException(); }
        [BuiltinFunction(IsConstant = true)]
        public static bool FALSE() { return false; }
        [BuiltinFunction(IsConstant = true)]
        public static bool TRUE() { return true; }
        [BuiltinFunction]
        public static double ABS(double value) { return Math.Abs(value); }
        [BuiltinFunction]
        public static double ACOS(double value) { return Math.Acos(value); }
        [BuiltinFunction]
        public static double ACOSH() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ASIN(double value) { return Math.Asin(value); }
        [BuiltinFunction]
        public static double ASINH() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ATAN(double value) { return Math.Atan(value); }
        [BuiltinFunction]
        public static double ATAN2(double value, double value2) { return Math.Atan2(value, value2); }
        [BuiltinFunction]
        public static double ATANH() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double CEILING(double value) { return Math.Ceiling(value); }
        [BuiltinFunction]
        public static double COMBIN() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double COS(double value) { return Math.Cos(value); }
        [BuiltinFunction]
        public static double COSH() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double DEGREES(double value) { return Math.PI * value / 180.0; }
        [BuiltinFunction]
        public static double EVEN(double value) { value = Math.Floor(value); return (value % 2 == 0) ? value : value + 1; }
        [BuiltinFunction]
        public static double EXP(double value) { return Math.Exp(value); }
        [BuiltinFunction]
        public static double FACT(double value) { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double FACTDOUBLE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double FLOOR(double value) { return Math.Floor(value); }
        [BuiltinFunction]
        public static double GCD() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double INT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double LCM() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double LN() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double LOG(double value) { return Math.Log(value); }
        [BuiltinFunction]
        public static double LOG10(double value) { return Math.Log10(value); }
        [BuiltinFunction]
        public static double MDETERM() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double MINVERSE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double MMULT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double MOD(double value1, double value2) { return value1 % value2; }
        [BuiltinFunction]
        public static double MROUND() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double MULTINOMIAL() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ODD(double value) { value = Math.Ceiling(value); return (value % 2 != 0) ? value : value - 1; }
        [BuiltinFunction(IsConstant = true)]
        public static double PI() { return Math.PI; }
        [BuiltinFunction]
        public static double POWER(double value, double power) { return Math.Pow(value, power); }
        [BuiltinFunction]
        public static double PRODUCT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double QUOTIENT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double RADIANS(double value) { return (Math.PI / 180) * value; }
        [BuiltinFunction]
        public static double RAND() { return (new Random()).NextDouble(); }
        [BuiltinFunction]
        public static double RANDBETWEEN(double a, double b) { return (new Random()).Next((int)a, (int)b); }
        [BuiltinFunction]
        public static string ROMAN(double number) {

            var romanNumerals = new string[][] {
                new string[]{"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"}, // ones
                new string[]{"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"}, // tens
                new string[]{"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"}, // hundreds
                new string[]{"", "M", "MM", "MMM", "(IV)", "(V)", "(VI)", "(VII)", "(VIII)" , "(XI)" }, // thousands
                new string[]{"", "(X)", "(XX)", "(XXX)", "(XL)", "(L)", "(LX)", "(LXX)", "(LXXX)", "(XC)"},
                new string[]{"", "(C)", "(CC)", "(CCC)", "(CD)", "(D)", "(DC)", "(DCC)", "(DCCC)", "(CM)"},
                new string[]{"", "(M)", "(MM)", "(MMM)", "[IV]", "[V]", "[VI]", "[VII]", "[VIII]" , "[XI]" },
                new string[]{"", "[X]", "[XX]", "[XXX]", "[XL]", "[L]", "[LX]", "[LXX]", "[LXXX]", "[XC]"},
                new string[]{"", "[C]", "[CC]", "[CCC]", "[CD]", "[D]", "[DC]", "[DCC]", "[DCCC]", "[CM]"},
            };

            if (number > 999999999) { return "#ERR"; }

            var intArr = ((int)number).ToString().Reverse().ToArray();
            var len = intArr.Length;
            var romanNumeral = "";
            var i = len;

            while (i-- > 0) {
                romanNumeral += romanNumerals[i][Int32.Parse(intArr[i].ToString())];
            }
            return romanNumeral.Replace(")(", "").Replace("][", "");
        }
        [BuiltinFunction]
        public static double ROUNDDOWN(double value, double precision) { return Math.Floor(value * Math.Pow(10, precision)) / Math.Pow(10, precision); }
        [BuiltinFunction]
        public static double ROUNDUP(double value, double precision) { return Math.Ceiling(value * Math.Pow(10, precision)) / Math.Pow(10, precision); }
        [BuiltinFunction]
        public static double SERIESSUM() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SIGN(double value) { return Math.Sign(value); }
        [BuiltinFunction]
        public static double SIN(double value) { return Math.Sin(value); }
        [BuiltinFunction]
        public static double SINH() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SQRT(double value) { return Math.Sqrt(value); }
        [BuiltinFunction]
        public static double SQRTPI() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SUBTOTAL() { throw new NotImplementedException(); }

        [BuiltinFunction]
        public static object MIN(ContextTable.Range range) {
            return range.GetValues().Min();
        }

        [BuiltinFunction]
        public static object MAX(ContextTable.Range range) {
            return range.GetValues().Max();
        }

        [BuiltinFunction]
        public static double COUNT(ContextTable.Range range) {
            double ret = 0;
            foreach (var obj in range.GetValues()) {
                if (obj != null) {
                    ret++;
                }
            }
            return ret;
        }
        [BuiltinFunction]
        public static double COUNTIF(ContextTable.Range range, object criteria) {
            double ret = 0;
            var objCriteria = new Predicates.Criteria(criteria);

            foreach (var obj in range.GetValues()) {
                try {
                    if (objCriteria.Check(obj) && obj != null) {
                        ret++;
                    }
                } catch { }
            }
            return ret;
        }

        [BuiltinFunction]
        public static double SUM(ContextTable.Range range) {
            double ret = 0;
            foreach (var obj in range.GetValues()) {
                try {
                    ret += (double)Convert.ChangeType(obj, typeof(double));
                } catch { }
            }
            return ret;
        }
        [BuiltinFunction]
        public static double SUMIF(ContextTable.Range range, object criteria) { return SUMIF(range, criteria, range); }
        [BuiltinFunction]
        public static double SUMIF(ContextTable.Range range, object criteria, ContextTable.Range sumRange) {

            var r1 = range.GetValues().GetEnumerator();
            var r2 = sumRange.GetValues().GetEnumerator();
            var objCriteria = new Predicates.Criteria(criteria);

            double result = 0;
            if (r1 != null && r2 != null) {
                do {

                    var valR1 = r1.Current;
                    var valR2 = r2.Current;

                    if (objCriteria.Check(valR1) && valR2 != null) {
                        result += (double)ExecutionContext.ChangeType(valR2, typeof(double), 0D);
                    }

                    if (!r2.MoveNext()) break;

                } while (r1.MoveNext());
            }

            return result;

        }
        [BuiltinFunction]
        public static double SUMPRODUCT(ContextTable.Range range1, ContextTable.Range range2) { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SUMSQ(double a, params double[] p) { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SUMX2MY2() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SUMX2PY2() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SUMXMY2() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double TAN(double value) { return Math.Tan(value); }
        [BuiltinFunction]
        public static double TANH() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double TRUNC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ASC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double BAHTTEXT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double CHAR() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double CLEAN() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double CODE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double CONCATENATE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double DOLLAR() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double EXACT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double FIND() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double FIXED() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double JIS() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double LEFT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double LEN(string value) { return (value != null) ? value.Length : 0; }
        [BuiltinFunction]
        public static string LOWER(string value) { return (value != null) ? value.ToLower() : null; }
        [BuiltinFunction]
        public static double MID() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double PHONETIC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double PROPER() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double REPLACE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double REPT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double RIGHT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SEARCH() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SUBSTITUTE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static string T(object value) { return ((value as string) != null) ? value.ToString() : string.Empty; }
        [BuiltinFunction]
        public static string TEXT(object value) { return (value != null) ? value.ToString() : null; }
        [BuiltinFunction]
        public static string TRIM(string value) { return (value != null) ? value.Trim() : null; }
        [BuiltinFunction]
        public static string UPPER(string value) { return (value != null) ? value.ToUpper() : null; }
        [BuiltinFunction]
        public static double VALUE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ACCRdouble() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ACCRdoubleM() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double AMORDEGRC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double AMORLINC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double COUPDAYBS() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double COUPDAYS() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double COUPDAYSNC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double COUPNCD() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double COUPNUM() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double COUPPCD() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double CUMIPMT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double CUMPRINC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double DB() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double DDB() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double DISC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double DOLLARDE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double DOLLARFR() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double DURATION() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double EFFECT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double FV() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double FVSCHEDULE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double doubleRATE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double IPMT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double IRR() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ISPMT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double MDURATION() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double MIRR() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double NOMINAL() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double NPER() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double NPV() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ODDFPRICE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ODDFYIELD() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ODDLPRICE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ODDLYIELD() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double PMT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double PPMT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double PRICE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double PRICEDISC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double PRICEMAT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double PV() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double RATE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double RECEIVED() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SLN() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double SYD() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double TBILLEQ() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double TBILLPRICE() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double TBILLYIELD() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double VDB() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double XIRR() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double XNPV() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double YIELD() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double YIELDDISC() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double YIELDMAT() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double CELL() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double INFO() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static bool ISBLANK(object value) { return value == null || (value is string && (string)value == string.Empty); }
        [BuiltinFunction]
        public static double ISERR() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static double ISERROR() { throw new NotImplementedException(); }
        [BuiltinFunction]
        public static bool ISEVEN(double value) { return value % 2 == 0; }
        [BuiltinFunction]
        public static bool ISLOGICAL(object value) { return value is bool; }
        [BuiltinFunction]
        public static bool ISNA(string value) { return (value == NA()); }
        [BuiltinFunction]
        public static bool ISNONTEXT(object value) { return ((value as string) == null); }
        [BuiltinFunction]
        public static bool ISNUMBER(object value) { return value != null && ExecutionContext.CanChangeToType<double>(value); }
        [BuiltinFunction]
        public static bool ISODD(double value) { return value % 2 != 0; }
        [BuiltinFunction]
        public static bool ISREF(object value) { return false; }
        [BuiltinFunction]
        public static bool ISTEXT(object value) { return (value as string) != null; }
        [BuiltinFunction]
        public static double N(object value) { return ExecutionContext.ChangeType<double>(value); }
        [BuiltinFunction]
        public static string NA() { return "#NA"; }
        [BuiltinFunction]
        public static double TYPE(object value) {
            if (value == null) return 0;
            else if (value is string) return 2;
            else if (value is bool) return 4;
            else if (value is ContextTable.Range) return 64;
            else if (value.GetType().IsValueType) return 1;
            else return 16;
        }

    }    
}        
         
         
         
         
         
         