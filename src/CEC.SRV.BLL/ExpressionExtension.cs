using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace CEC.SRV.BLL
{
    internal static class ExpressionExtension
    {

        internal static CallInfo GetCallInfo(this LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            
            MethodCallExpression methodCall = ExpressionExtension.ToMethodCall(expression);
            
            return new CallInfo
            {
                Method = methodCall.Method,
                Arguments = methodCall.Arguments,
                Object = methodCall.Object
            };
        }

        /// <summary>
        /// Casts the body of the lambda expression to a <see cref="MethodCallExpression"/>.
        /// </summary>
        /// <exception cref="ArgumentException">If the body is not a method call.</exception>
        internal static MethodCallExpression ToMethodCall(this LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var methodCall = expression.Body as MethodCallExpression;
            if (methodCall == null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,expression.ToString()));
            }

            return methodCall;
        }

        /// <summary>
        /// Converts the body of the lambda expression into the <see cref="PropertyInfo"/> referenced by it.
        /// </summary>
        internal static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var prop = expression.Body as MemberExpression;
            if (prop != null)
            {
                var info = prop.Member as PropertyInfo;
                if (info != null)
                {
                    return info;
                }
            }

            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture, expression.ToString()));
        }

        /// <summary>
        /// Checks whether the body of the lambda expression is a property access.
        /// </summary>
        internal static bool IsProperty(this LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            return IsProperty(expression.Body);
        }

        /// <summary>
        /// Checks whether the expression is a property access.
        /// </summary>
        internal static bool IsProperty(this Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            var prop = expression as MemberExpression;

            return prop != null && prop.Member is PropertyInfo;
        }

        
    }

    internal class CallInfo
    {
        public Expression Object { get; set; }
        public MethodInfo Method { get; set; }
        public IEnumerable<Expression> Arguments { get; set; }
    }
}