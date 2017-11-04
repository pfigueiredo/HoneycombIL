using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RLang.Calculation.Engine {

    public class ExecutionContext : MarshalByRefObject, IDisposable  {

        #region ChangeType

        public static bool IsEqual (object a, object b) {
            if (a == null || b == null) return object.Equals(a, b);

            Type ta = a.GetType();
            Type tb = b.GetType();

            if (ta == tb) return object.Equals(a, b);

            double va = (double)ChangeType(a, typeof(double), double.NaN);
            double vb = (double)ChangeType(b, typeof(double), double.NaN);

            return va.Equals(vb);

        }

        public static bool CanChangeToType<T>(object value) {

            Type destType = typeof(T);

            if (value == null || value == System.DBNull.Value) {
                return (destType.IsClass || destType.IsInterface
                    || (destType.IsGenericType && destType.GetGenericTypeDefinition() == typeof(Nullable<>)));
            }

            Type objType = value.GetType();
            
            if (destType.IsAssignableFrom(objType)) return true;

            if (destType.IsGenericType && destType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                destType = destType.GetGenericArguments()[0];
            }

            //special cases;
            //DateTime <-> double
            if (destType == typeof(DateTime) && objType == typeof(double)) return true;
            if (destType == typeof(double) && objType == typeof(DateTime)) return true;

            //bool <-> double
            if (destType == typeof(bool) && objType == typeof(double)) return true;
            if (destType == typeof(double) && objType == typeof(bool)) return true;

            try {
                return Convert.ChangeType(value, destType, System.Globalization.CultureInfo.InvariantCulture) != null;
            } catch {
                return false;
            }

        }

        public static T ChangeType<T>(object value) {
            var defValue = (typeof(T) == typeof(double)) ? (T)(object)Double.NaN : default(T);
            return ChangeType<T>(value, defValue);
        }

        public static T ChangeType<T>(object value, T defaultValue) {
            return (T)(object)ChangeType(value, typeof(T), defaultValue);
        }

        public static object ChangeType(object value, Type destType, object defaultValue) {
            if (value == null || value == System.DBNull.Value) return defaultValue;

            Type objType = value.GetType();

            if (destType.IsAssignableFrom(objType)) return value;

            if (destType.IsGenericType && destType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                destType = destType.GetGenericArguments()[0];
            }

            //special cases;
            //DateTime <-> double
            if (destType == typeof(DateTime) && objType == typeof(double))
                return (object)DateTime.FromOADate((double)value);
            if (destType == typeof(double) && objType == typeof(DateTime))
                return (object)((DateTime)value).ToOADate();

            //bool <-> double
            if (destType == typeof(bool) && objType == typeof(double))
                return (object)((double)value != 0);
            if (destType == typeof(double) && objType == typeof(bool))
                return (object)(((bool)value) ? 1.0 : 0.0);

            try {
                return Convert.ChangeType(value, destType, System.Globalization.CultureInfo.InvariantCulture);
            } catch {
                return defaultValue;
            }
        }
        #endregion  

        #region static
        public static ExecutionContext LinkContexts(List<ExecutionContext> list) {

            if (list != null && list.Count > 0) {

                for (int i = 0; i < list.Count; i++) {
                    ExecutionContext previous = null, current = list[i];
                    if (i > 0) {
                        previous = list[i - 1];
                        previous.Next = current;
                    }
                    current.Previous = previous;
                }

                return list[0];

            }

            return null;
        }

        private static object _locker = new object();
        private static PropertyInfo _valueProperty = null;
        public static PropertyInfo ValueProperty() {

            if (_valueProperty == null) {
                lock (_locker) {
                    if (_valueProperty == null) {
                        Type t = typeof(ExecutionContext);
                        _valueProperty = t.GetProperty("Item", new Type[] { typeof(string), typeof(string) });
                    }
                }
            }

            return _valueProperty;

        }

        private static PropertyInfo _dataProperty = null;
        public static PropertyInfo DataProperty() {

            if (_dataProperty == null) {
                lock (_locker) {
                    if (_dataProperty == null) {
                        Type t = typeof(ExecutionContext);
                        _dataProperty = t.GetProperty("Data");
                    }
                }
            }

            return _dataProperty;

        }

        private static PropertyInfo _tableValueProperty = null;
        public static PropertyInfo TableValueProperty() {

            if (_tableValueProperty == null) {
                lock (_locker) {
                    if (_tableValueProperty == null) {
                        Type t = typeof(ContextTable);
                        _tableValueProperty = t.GetProperty("Item", typeof(object), new Type[] { typeof(int), typeof(string) });
                    }
                }
            }

            return _tableValueProperty;

        }

        private static PropertyInfo _rowNumberProperty = null;
        public static PropertyInfo RowNumberProperty() {

            if (_rowNumberProperty == null) {
                lock (_locker) {
                    if (_rowNumberProperty == null) {
                        Type t = typeof(ExecutionContext);
                        _rowNumberProperty = t.GetProperty("Row", typeof(int));
                    }
                }
            }

            return _rowNumberProperty;
        }

        private static PropertyInfo _EODNumberProperty = null;
        public static PropertyInfo EODNumberProperty() {

            if (_EODNumberProperty == null) {
                lock (_locker) {
                    if (_EODNumberProperty == null) {
                        Type t = typeof(ExecutionContext);
                        _EODNumberProperty = t.GetProperty("EOD", typeof(bool));
                    }
                }
            }

            return _EODNumberProperty;
        }

        private static MethodInfo _rangeMethod = null;
        public static MethodInfo RangeMethod() {

            if (_rangeMethod == null) {
                lock (_locker) {
                    if (_rangeMethod == null) {
                        Type t = typeof(ContextTable);
                        _rangeMethod = t.GetMethod("GetRange");
                    }
                }
            }

            return _rangeMethod;
        }

        #endregion

        private Dictionary<string, ExpandoObject> values = new Dictionary<string, ExpandoObject>(StringComparer.InvariantCultureIgnoreCase);
        private ContextTable data = new ContextTable();

        public ContextTable Data { get { return data; } }
        public ExecutionContext Previous { get; set; }
        public ExecutionContext Next { get; set; }
        public string ContextId { get; set; }

        public IEnumerable<string> Variables {
            get {
                foreach (var obj in values.Keys) {
                    yield return obj;
                }
            }
        }
        
        public IEnumerable<KeyValuePair<string, ExpandoObject>> Values {
            get {
                foreach (var obj in values)
                    yield return obj;
            }
        }

        public int Row { get; set; }
        public bool EOD { get { return this.Row >= this.Data.Rows.Count; } }

        private object _contextLocker = new object();
        public object ContextLocker { get { return _contextLocker; } }

        public void ImportObject(string key, object obj) {

            throw new NotImplementedException();
            //var newObject = new ExpandoObject() as IDictionary<string, object>;
            //foreach (prop in ExpandoObject)

        }

        public object this[string key] {
            get { return this[key, "value"]; }
            set { this[key, "value"] = value; }
        }

        public object this[string key, string property] {
            get {

                if (key == null || key == string.Empty)
                    throw new ArgumentException("Key cannot be empty");

                if (property == null || property == string.Empty)
                    throw new ArgumentException("Property cannot be empty");

                property = property.ToLower();

                if (values.ContainsKey(key)) {
                    var obj = values[key] as IDictionary<string, object>;

                    if (obj.ContainsKey(property))
                        return obj[property];
                }

                return 0;
            }
            set {

                if (key == null || key == string.Empty)
                    throw new ArgumentException("Key cannot be empty");

                if (property == null || property == string.Empty)
                    throw new ArgumentException("Property cannot be empty");

                property = property.ToLower();

                if (values.ContainsKey(key)) {
                    var obj = values[key] as IDictionary<string, object>;
                    if (obj.ContainsKey(property))
                        obj[property] = value;
                    else
                        obj.Add(property, value);
                } else {
                    var obj = new ExpandoObject() as IDictionary<string, object>;
                    if (obj.ContainsKey(property))
                        obj[property] = value;
                    else
                        obj.Add(property, value);
                    values.Add(key, (ExpandoObject)obj);
                }
            }
        }

        public List<string> GetContextSymbols() {
            List<string> ret = new List<string>();

            var context = this;
            while (context != null) {

                foreach (var key in context.values.Keys) {
                    if (!ret.Contains(key)) ret.Add(key);
                }

                context = context.Next;
            }

            return ret;

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    this.data.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
        }
        #endregion

    }

    [Serializable]
    public class ContextTable : DataTable {

        public void AddColumn(string column) {
            this.Columns.Add(column, typeof(object));
        }

        public void AddRow() {
            this.Rows.Add(this.NewRow());
        }

        private void AddRowNumColumn() {
            this.Columns.Add("Row", typeof(int));
            this.Columns["Row"].AutoIncrement = true;
            this.Columns["Row"].AutoIncrementSeed = 0;
        }

        public ContextTable() : base () {
            AddRowNumColumn();
        }

        public ContextTable(string name) : base(name) {
            AddRowNumColumn();
        }

        protected ContextTable(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public int Count {  get { return this.Rows.Count; } }

        private void InsureCell(int row, string column) {
            if (!this.Columns.Contains(column)) this.Columns.Add(column, typeof(object));

            while (this.Rows.Count <= row) {
                this.Rows.Add(this.NewRow());
            }
        }

        private void InsureCell(int row, int column) {

            while (this.Rows.Count <= column) {
                int index = this.Rows.Count;
                this.Columns.Add(string.Format("Col{0}", index), typeof(object));
            }

            while (this.Rows.Count <= row) {
                this.Rows.Add(this.NewRow());
            }

        }

        public object this[int row, int column] {
            get {

                if (column < 0) return 0.0;
                if (row < 0) return 0.0;

                InsureCell(row, column);

                object val = this.Rows[row][column];
                return ((val as System.DBNull) != null) ? 0.0 : val;
            }
            set {
                InsureCell(row, column);
                this.Rows[row][column] = value;
            }
        }

        public object this[int row, string column] {
            get {
                if (row < 0) return 0.0;

                this.InsureCell(row, column);

                object val = this.Rows[row][column];
                return ((val as System.DBNull) != null) ? 0.0 : val;
            }
            set {
                this.InsureCell(row, column);
                this.Rows[row][column] = value;
            }
        }

        public Range GetRange(string startColumn, int startRow, string endColumn, int endRow) {

            if (!this.Columns.Contains(startColumn))
                throw new VariableNotFoundException(string.Format("column:{0}", startColumn));

            if (!this.Columns.Contains(endColumn))
                throw new VariableNotFoundException(string.Format("column:{0}", endColumn));

            if (startRow < 0) startRow = 0;
            if (startRow >= this.Count) startRow = this.Count - 1;
            if (endRow < 0) endRow = 0;
            if (endRow >= this.Count) endRow = this.Count - 1;

            return new Range(this, startColumn, startRow, endColumn, endRow);
        }

        public class Range {

            private Range ParentRange { get; set; }
            private ContextTable Table { get; set; }
            public string StartColumn { get; set; }
            public string EndColumn { get; set; }
            public int StartRow { get; set; }
            public int EndRow { get; set; }

            public ContextTable GetTable() { return this.Table; }

            protected Range(Range range) {
                this.ParentRange = range;
                this.Table = range.Table;
                this.StartColumn = range.StartColumn;
                this.EndColumn = range.EndColumn;
                this.StartRow = range.StartRow;
                this.EndRow = range.EndRow;
            }

            public Range(ContextTable table, string startColumn, int startRow, string endColumn, int endRow) {
                this.ParentRange = null;
                this.Table = table;
                this.StartColumn = startColumn;
                this.EndColumn = endColumn;
                this.StartRow = startRow;
                this.EndRow = endRow;
            }

            private IEnumerable<object> GetCurrentRangeValues() {
                int startColumn = Table.Columns.Contains(StartColumn) ? Table.Columns.IndexOf(StartColumn) : 0;
                int endColumn = Table.Columns.Contains(EndColumn) ? Table.Columns.IndexOf(EndColumn) : Table.Columns.Count - 1;

                if (startColumn > endColumn) {
                    var t = startColumn;
                    startColumn = endColumn;
                    endColumn = t;
                }

                for (int c = Table.Columns.IndexOf(StartColumn); c <= Table.Columns.IndexOf(EndColumn); c++) {
                    for (int r = StartRow; (r <= EndRow && r < Table.Rows.Count); r++) {
                        var value = Table.Rows[r][c];
                        yield return ((value as System.DBNull) != null) ? null : value;
                    }
                }
            }

            public virtual IEnumerable<object> GetValues() {

                if (ParentRange != null)
                    return ParentRange.GetValues();
                else
                    return this.GetCurrentRangeValues();

            }

            public override string ToString() {
                var values = GetValues().Take(11);
                return string.Format("[{0}{1}]", string.Join(",", values.Take(10)), (values.Count() > 10) ? "..." : "");
            }

            public virtual Array ToArray() {
                int startColumn = Table.Columns.Contains(StartColumn) ? Table.Columns.IndexOf(StartColumn) : 0;
                int endColumn = Table.Columns.Contains(EndColumn) ? Table.Columns.IndexOf(EndColumn) : Table.Columns.Count - 1;

                if (startColumn > endColumn) {
                    var t = startColumn;
                    startColumn = endColumn;
                    endColumn = t;
                }

                object[,] retArray = new object[endColumn - startColumn + 1, EndRow - StartRow + 1];

                for (int c = Table.Columns.IndexOf(StartColumn); c <= Table.Columns.IndexOf(EndColumn); c++) {
                    for (int r = StartRow; (r <= EndRow && r < Table.Rows.Count); r++) {
                        var value = Table.Rows[r][c];
                        retArray[c, r] = ((value as System.DBNull) != null) ? null : value;
                    }
                }

                return retArray;
            }

        } 

        public class WhereRange : Range {

            public Func<object, bool> Predicate { get; set; }

            public WhereRange(Range range, Func<object, bool> predicate) : base(range) {
                this.Predicate = predicate;
            }

            public override IEnumerable<object> GetValues() {
                return base.GetValues().Where(this.Predicate);
            }

            public override Array ToArray() {
                return this.GetValues().ToArray();
            }

        }

        public class DistinctRange : Range {

            public DistinctRange(Range range) : base(range) { }

            public override IEnumerable<object> GetValues() {
                return base.GetValues().Distinct();
            }

            public override Array ToArray() {
                return this.GetValues().ToArray();
            }

        }

        public class AdHocRange : Range {

            public IEnumerable<object> Values { get; set; }

            public AdHocRange(Range range, IEnumerable<object> values) : base(range) {
                this.Values = values;
            }

            public override IEnumerable<object> GetValues() {
                return Values;
            }

            public override Array ToArray() {
                return this.GetValues().ToArray();
            }

        }

    }

}
