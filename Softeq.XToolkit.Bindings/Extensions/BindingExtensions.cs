using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Softeq.XToolkit.Common.Interfaces;

namespace Softeq.XToolkit.Bindings.Extensions
{
    public static class BindingExtensions
    {
        public static Binding Bind<T1, T2>(this object obj, Expression<Func<T1>> sourcePropertyExpression,
            Expression<Func<T2>> targetPropertyExpression, IConverter<T2, T1> converter)
        {
            return Bind(obj, sourcePropertyExpression, targetPropertyExpression, BindingMode.OneWay, converter);
        }

        public static Binding<T1, T1> Bind<T1>(this object obj, Expression<Func<T1>> sourcePropertyExpression,
            Action whenSourceChanges)
        {
            return obj.SetBinding(sourcePropertyExpression).WhenSourceChanges(whenSourceChanges);
        }

        public static Binding<T1, T1> Bind<T1>(this object obj, Expression<Func<T1>> sourcePropertyExpression,
            Func<T1, Task> whenSourceChanges)
        {
            return obj.SetBinding(sourcePropertyExpression).WhenSourceChanges(whenSourceChanges);
        }

        public static Binding<T1, T1> Bind<T1>(this object obj, Expression<Func<T1>> sourcePropertyExpression,
            Action<T1> action,
            BindingMode bindingMode = BindingMode.OneWay)
        {
            return obj.SetBinding(sourcePropertyExpression, bindingMode).WhenSourceChanges(action);
        }

        public static Binding Bind<T1, T2>(this object obj,
            Expression<Func<T1>> sourcePropertyExpression,
            Expression<Func<T2>> targetPropertyExpression = null,
            BindingMode mode = BindingMode.OneWay,
            IConverter<T2, T1> converter = null)
        {
            return obj.SetBinding(sourcePropertyExpression, targetPropertyExpression, mode)
                .SetConverter(converter);
        }
    }
}
