using RLang.Calculation.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RLang.Calculation.Excel {
    public static class Predicates {

        public enum CriteriaOperation { EQ, NEQ, GT, ST, GTEQ, STEQ }

        public class Criteria {
            public object Value { get; private set; }
            public CriteriaOperation Operation { get; private set; }

            public Criteria(object criteria) {
                var strCriteria = criteria as string;
                if (strCriteria != null) {

                    if (strCriteria.StartsWith(">=")) {
                        this.Operation = CriteriaOperation.GTEQ;
                        this.Value = strCriteria.Substring(2);
                    } else if (strCriteria.StartsWith("<=")) {
                        this.Operation = CriteriaOperation.STEQ;
                        this.Value = strCriteria.Substring(2);
                    } else if (strCriteria.StartsWith("<>")) {
                        this.Operation = CriteriaOperation.NEQ;
                        this.Value = strCriteria.Substring(2);
                    } else if (strCriteria.StartsWith(">")) {
                        this.Operation = CriteriaOperation.GT;
                        this.Value = strCriteria.Substring(1);
                    } else if (strCriteria.StartsWith("<")) {
                        this.Operation = CriteriaOperation.ST;
                        this.Value = strCriteria.Substring(1);
                    } else if (strCriteria.StartsWith("=")) {
                        this.Operation = CriteriaOperation.EQ;
                        this.Value = strCriteria.Substring(1);
                    } else {
                        this.Operation = CriteriaOperation.EQ;
                        this.Value = strCriteria;
                    }

                } else {
                    this.Operation = CriteriaOperation.EQ;
                    this.Value = criteria;
                }

            }

            public bool Check(object value) {

                if (this.Operation == CriteriaOperation.EQ) {
                    if (value != null)
                        return object.Equals(value, ExecutionContext.ChangeType(this.Value, value.GetType(), null));
                    else
                        return this.Value == null;
                } else if (this.Operation == CriteriaOperation.NEQ) {
                    if (value != null)
                        return !object.Equals(value, ExecutionContext.ChangeType(this.Value, value.GetType(), null));
                    else
                        return this.Value != null;
                } else {
                    double v1 = (double)ExecutionContext.ChangeType(value, typeof(double), Double.NaN);
                    double v2 = (double)ExecutionContext.ChangeType(this.Value, typeof(double), Double.NaN);
                    switch (this.Operation) {
                        case CriteriaOperation.GT: return v1 > v2;
                        case CriteriaOperation.GTEQ: return v1 >= v2;
                        case CriteriaOperation.ST: return v1 < v2;
                        case CriteriaOperation.STEQ: return v1 <= v2;
                    }
                }

                return false;

            }

        }

        public static Func<object, bool> CheckCriteria(object criteria) {
            var objCriteria = new Criteria(criteria);
            return (obj => objCriteria.Check(obj));
        }



    }
}
