using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyProject.Core.ModelExtensions
{
    public static class ViewModelHelperExtensions
    {
        private static readonly Dictionary<(Type, Type), Action<object, object>> _propertyCopyDictionary = new();

        /// <summary>
        /// 타입이 <c>TDestination</c>인 객체를 하나 만들고, <c>source</c>의 프로퍼티에서 값을 가져와 새로 생성된
        /// 객체의 프로퍼티에 복사합니다. JS의 spread operator 대신 사용할 수 있습니다. 상속관계에 있는 record에서,
        /// 부모 타입의 record의 값을 자식 타입의 record로 복사하는 데 사용할 수 있습니다.
        /// </summary>
        /// <typeparam name="TDestination">결과 타입</typeparam>
        /// <param name="source">값을 가져올 객체</param>
        /// <returns>새로 생성된 객체</returns>
        public static TDestination Copy<TDestination>(this object source)
        {
            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);

            Action<object, object> copy;
            lock (_propertyCopyDictionary)
            {
                if (!_propertyCopyDictionary.TryGetValue((sourceType, destinationType), out copy))
                {
                    var sourceParam = Expression.Parameter(typeof(object));
                    var destinationParam = Expression.Parameter(typeof(object));

                    var sourceCasted = Expression.TypeAs(sourceParam, sourceType);
                    var destinationCasted = Expression.TypeAs(destinationParam, destinationType);

                    var assignmentExpressions = sourceType.GetProperties().Select((sourceProperty) =>
                    {
                        var destinationProperty = destinationType.GetProperty(sourceProperty.Name);
                        return Expression.Call(
                            destinationCasted,
                            destinationProperty.GetSetMethod(),
                            Expression.Call(
                                sourceCasted,
                                sourceProperty.GetGetMethod()
                            )
                        );
                    });

                    copy = Expression.Lambda<Action<object, object>>(
                        Expression.Block(assignmentExpressions),
                        new ParameterExpression[] { sourceParam, destinationParam }
                    ).Compile();
                    _propertyCopyDictionary.Add((sourceType, destinationType), copy);
                }
            }

            TDestination destination = (TDestination)Activator.CreateInstance(destinationType);
            copy(source, destination);
            return destination;
        }
    }
}
