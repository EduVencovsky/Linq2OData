using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Linq2OData.Core
{
    public static class QueryableExtensions
    {
        #region Public
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

        public static IEnumerable<MethodCallExpression> GetMethods<T>(this IQueryable<T> queryable, string method) 
        {
            var methodExpression = (MethodCallExpression)queryable.Expression;
            var methods = new List<MethodCallExpression>();
            while (methodExpression != null) {
                if (methodExpression.Method.Name == method)
                {
                    methods.Add(methodExpression);
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
                    return methods;
                }
            }
            return methods;
        }

        public static ReadOnlyCollection<Expression> GetMethodArguments<T>(this IQueryable<T> queryable, string method) 
        {
            return queryable.GetMethod(method)?.Arguments;
        }

        public static IEnumerable<ReadOnlyCollection<Expression>> GetMethodsArguments<T>(this IQueryable<T> queryable, string method) 
        {
            var methods = queryable.GetMethods(method);
            var methodsArguments = new List<ReadOnlyCollection<Expression>>();

            if(methods != null && methods.Count() > 0)
                return methodsArguments;

            foreach(var m in methods){
                methodsArguments.Add(m.Arguments);
            }

            return methodsArguments;
        }

        public static IEnumerable<object> GetMethodArgumentsValues<T>(this IQueryable<T> queryable, string method)
        {
            var arguments = queryable.GetMethodArguments(method);                
            return GetMethodArgumentsValues(arguments);
        }
        
        public static IEnumerable<IEnumerable<object>> GetMethodsArgumentsValues<T>(this IQueryable<T> queryable, string method)
        {
            var methodsArguments = queryable.GetMethodsArguments(method);                
            var argumentsValues = new List<IEnumerable<object>>();
            
            foreach(var methodArguments in methodsArguments)
            {              
                argumentsValues.Add(GetMethodArgumentsValues(methodArguments));
            }

            return argumentsValues;
        }
        
        public static bool HasMethod<T>(this IQueryable<T> queryable, string method)
        {
            return queryable.GetMethod(method) != null;        
        }
        #endregion
        
        #region Private            
        private static IEnumerable<object> GetMethodArgumentsValues(ReadOnlyCollection<Expression> arguments)
        {
            var resultArguments = new List<object>();

            if(arguments == null)
                return resultArguments;   
            
            foreach (var arg in arguments.Skip(1)) // skip first argument because it's the "this" parameter of the builder pattern
            {
                if (arg is ConstantExpression constantExpression)
                {
                    resultArguments.Add(constantExpression.Value);
                }
                else if (arg is UnaryExpression unaryExpression)
                {
                    resultArguments.Add(unaryExpression.Operand);
                }
                else 
                {
                    resultArguments.Add(arg);
                }
            } 
            return resultArguments;
        }
        #endregion
    }
}