using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Linq2OData.Core
{
    public static class QueryableHelpers
    {
        public static MethodCallExpression GetMethod<T>(this IQueryable<T> queryable, string method) 
        {
            var methodExpression = (MethodCallExpression)queryable.Expression;
            while (methodExpression != null) {
                if (methodExpression.Method.Name == method)
                {
                    return methodExpression;
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
                    return null;
                }
            }
            return null;
        }

        public static ReadOnlyCollection<Expression> GetMethodArguments<T>(this IQueryable<T> queryable, string method) 
        {
            return queryable.GetMethod(method)?.Arguments;
        }

        public static IEnumerable<object> GetMethodArgumentsValues<T>(this IQueryable<T> queryable, string method)
        {
            var arguments = queryable.GetMethodArguments(method)
                .Skip(1); // skip first argument because it's the "this" parameter of the builder pattern
            
            var resultArguments = new List<object>();   
            
            foreach(var arg in arguments)
            {
                if (arg is ConstantExpression constantExpression)
                {
                    resultArguments.Add(constantExpression.Value);
                }
                else 
                {

                }
            } 
            return resultArguments;
        }
        
        public static bool HasMethod<T>(this IQueryable<T> queryable, string method)
        {
            return queryable.GetMethod(method) != null;        
        }
    }
}