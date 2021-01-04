using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Linq2OData.Core
{
    public static class HasMethodClass 
    {
        public static bool HasMethod<T>(this IQueryable<T> queryable, string method)
        {
            var methodExpression = (MethodCallExpression)queryable.Expression;
            while (methodExpression != null) {
                if (methodExpression.Method.Name == method)
                {
                    return true;
                } 
                else if (
                    methodExpression.Arguments != null && 
                    methodExpression.Arguments.Count > 1 && 
                    methodExpression.Arguments?[0] is MethodCallExpression subMethodExpression
                ){
                    methodExpression = subMethodExpression;
                }
                else 
                {
                    return false;
                }
            }
            return false;
        }
    }
}