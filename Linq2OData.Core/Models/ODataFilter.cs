using System;
using System.Linq;
using System.Linq.Expressions;
using Linq2OData.Core.Enums;

namespace Linq2OData.Core.Models
{
    public class ODataFilter<T>
    {
        private string _value;
     
        #region Constructor
        public ODataFilter(string filter)
        {
            _value = filter;
        }            
        #endregion

        #region Operators
        public static implicit operator string(ODataFilter<T> oDataFilter) {
            return oDataFilter._value;
        }

        public static implicit operator ODataFilter<T>(string oDataFilter) {
            return new ODataFilter<T>(oDataFilter);
        }        
        #endregion

        #region Builder Methods

        public ODataFilter<T> AddFilter(string filter, LogicalOperators logicalOperator)
        {
            var logicalOperatorName = Enum.GetName(typeof(LogicalOperators), logicalOperator).ToLower();

            var prefix = !string.IsNullOrWhiteSpace(_value) ?
                $" {logicalOperatorName} " :
                $"";

            _value += prefix + $"({filter})";

            return this;
        }
        
        public ODataFilter<T> And(string odata) => AddFilter(odata, LogicalOperators.And);        
        
        public ODataFilter<T> Or(string odata) =>  AddFilter(odata, LogicalOperators.Or);

        public ODataFilter<T> AddFilter(Expression<Func<T, bool>> expression, LogicalOperators logicalOperators) 
        {
            var odata = expression.ToOdataFilter();
            return AddFilter(odata, logicalOperators);
        }
        
        public ODataFilter<T> And(Expression<Func<T, bool>> expression) 
        {
            var odata = expression.ToOdataFilter();
            return And(odata);
        }
        
        public ODataFilter<T> Or(Expression<Func<T, bool>> expression) 
        {
            var odata = expression.ToOdataFilter();
            return Or(odata);
        }
        #endregion
    }
}