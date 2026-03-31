using System;
using System.Linq;
using DevExpress.Data.Filtering;

namespace GAVELISv2.Module.BusinessObjects
{
    public class TimeAddFunction : ICustomFunctionOperatorFormattable
    {
        #region ICustomFunctionOperatorFormattable Members
        // The function's expression to be evaluated on the server. 
        string ICustomFunctionOperatorFormattable.Format(Type providerType, params string[] operands)
        {
            return string.Join(" + ", operands);
        }

        #endregion

        #region ICustomFunctionOperator Members
        // Evaluates the function on the client. 
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            var sum = TimeSpan.Zero;
            foreach (var o in operands)
            {
                if (o == null) return null;
                sum += (TimeSpan)o;
            }
            return sum;
        }

        string ICustomFunctionOperator.Name
        {
            get { return "TimeAdd"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(TimeSpan);
        }
        #endregion
    }
}
