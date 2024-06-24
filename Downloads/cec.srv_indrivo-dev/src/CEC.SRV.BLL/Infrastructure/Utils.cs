using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Infrastructure
{
    public static class Utils
    {
        public static string GetPropName<T>(Expression<Func<T, object>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpr = propertyExpression.Body as MemberExpression;

            if (memberExpr == null)
            {
                var unaryExpr = propertyExpression.Body as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                    memberExpr = unaryExpr.Operand as MemberExpression;
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member.Name;
            }

            throw new ArgumentException("No property reference expression was found.",
                             "propertyRefExpr");
        }
    }
}
