using System;
using System.Linq.Expressions;
using Linq2OData.Core;

namespace Linq2OData.Core
{
    public static class ODataHelpers
    {        
        /// <summary>
        /// Converts a expression into a odata filter.
        /// </summary>
        /// <typeparam name="T">Type of the entity to create the filter from.</typeparam>
        /// <param name="expression">Expression to be converted to OData.</param>
        /// <returns>Converted OData.</returns>
        public static string ToOdataFilter<T>(this Expression<Func<T, bool>> expression){
            var translator = new ODataFilter();
            var odata = translator.ToFilter(expression);
            return odata;
        }
    }
}
