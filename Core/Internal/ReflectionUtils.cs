using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Core.Internal
{
    public static class ReflectionUtils
    {

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof (double),
            typeof (float),
            typeof (decimal),
            typeof (byte),
            typeof (sbyte),
            typeof (ushort),
            typeof (uint),
            typeof (ulong),
            typeof (short),
            typeof (int),
            typeof (long)
        };
        public static bool IsNumeric(this Type type)
        {
            return NumericTypes.Contains(type) || NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        public static string GetPropertyName<T, TResult>(this Expression<Func<T, TResult>> prop)
        {
            var lambda = (LambdaExpression)prop;
            if (lambda == null)
                return null;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;
            return memberExpression.Member.Name;
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> selector)
            where T : class
        {
            return selector.GetPropertyName();
        }

        /// <summary>
        /// Получение метаинформации свойства по выражению. 
        /// </summary>
        /// <param name="prop"> Выражение по которому определяется свойство. </param>
        /// <typeparam name="T"> Тип выражения. </typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns> Метаинформация по свойству. </returns>
        public static PropertyInfo GetProperty<T, TResult>(this Expression<Func<T, TResult>> prop)
        {
            return typeof(T).GetProperty(prop.GetPropertyName());
        }

        public static List<string> GetPropertyPathParts<T, TResult>(this Expression<Func<T, TResult>> selector)
        {
            if (selector == null)
                return null;

            var path = new List<string>();
            var expression = selector.Body;

            var memberExpression = selector.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression as UnaryExpression;
                if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
                    memberExpression = unaryExpression.Operand as MemberExpression;

            }
            while (memberExpression != null)
            {

                path.Insert(0, memberExpression.Member.Name);
                if (memberExpression.Expression is MemberExpression)
                    memberExpression = (MemberExpression)memberExpression.Expression;
                else if (memberExpression.Expression is MethodCallExpression)
                {
                    var methodCallExpression = (MethodCallExpression)memberExpression.Expression;
                    if (methodCallExpression.Object is MemberExpression)
                        memberExpression = (MemberExpression)methodCallExpression.Object;
                    else if (methodCallExpression.Arguments.Count > 0 && methodCallExpression.Arguments[0] is MemberExpression)
                        memberExpression = (MemberExpression)methodCallExpression.Arguments[0];
                    else
                        break;
                }
                else
                    break;
            }
            return path;
        }

        public static bool IsBasedOnType(this Type type, Type basedOn)
        {

            if (basedOn.IsAssignableFrom(type))
                return true;

            if (basedOn.IsGenericTypeDefinition)
            {
                if (basedOn.IsInterface)
                    return IsBasedOnGenericInterface(type, basedOn);
                else
                    return IsBasedOnGenericClass(type, basedOn);
            }

            return false;
        }

        private static bool IsBasedOnGenericInterface(Type type, Type basedOn)
        {
            foreach (var baseInterface in type.GetInterfaces())
            {
                if (baseInterface.IsGenericType && (baseInterface.GetGenericTypeDefinition() == basedOn))
                {
                    if ((baseInterface.ReflectedType == null) && baseInterface.ContainsGenericParameters)
                        return true;
                }
            }

            return false;
        }

        private static bool IsBasedOnGenericClass(Type type, Type basedOn)
        {
            while (type != null)
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == basedOn))
                    return true;
                type = type.BaseType;
            }

            return false;
        }

        public static IEnumerable<PropertyInfo> AvailableProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .OrderBy(p => p.Name);
        }

        public static string NameWithAssembly(this Type type)
        {
            var name = type.AssemblyQualifiedName;
            return name?.Substring(0, name.IndexOf(", Version=", StringComparison.Ordinal));
        }

        public static string GetPropertyCode(this PropertyInfo propertyInfo)
        {
            return propertyInfo.Name;
        }

        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            var lambdaExpression = (LambdaExpression)expression;
            if (lambdaExpression.Body is ConstantExpression)
                return null;

            MemberExpression memberExpression;
            if (lambdaExpression.Body is UnaryExpression)
                memberExpression = (MemberExpression)((UnaryExpression)lambdaExpression.Body).Operand;
            else
                memberExpression = (MemberExpression)lambdaExpression.Body;

            var memberInfo = memberExpression.Member;

            if (memberExpression.Expression != null && memberExpression.Expression.Type != memberInfo.DeclaringType)
            {
                var objectType = memberExpression.Expression.Type;
                memberInfo = objectType.GetMember(memberInfo.Name).First();
            }

            return memberInfo;
        }

        /// <summary>
        /// Искуственно вызывает событие в HttpApplication
        /// </summary>
        /// <param name="app"> Экземпляр HttpApplication </param>
        /// <param name="invokeEvent"> Событие </param>
        /// <param name="args"> Агрументы события, обычно (this, EventArgs.Empty)</param>
        public static void FireEvent(this HttpApplication app, HttpApplicationEvents invokeEvent, params object[] args)
        {
            var objectType = app.GetType();

            var memberInfo = objectType.GetField(invokeEvent.ToString("G"), BindingFlags.Static | BindingFlags.NonPublic);
            if (memberInfo == null)
                return;
            var eventIndex = memberInfo.GetValue(app);

            var fieldInfo = objectType.GetField("_events", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo == null)
                return;
            var events = (EventHandlerList)fieldInfo.GetValue(app);

            var handler = (EventHandler)events[eventIndex];

            var delegates = handler.GetInvocationList();

            foreach (var dlg in delegates)
            {
                dlg.Method.Invoke(dlg.Target, args);
            }
        }
        public enum HttpApplicationEvents
        {
            EventBeginRequest,
            EventEndRequest,

            EventAuthenticateRequest,
            EventAuthorizeRequest,
        }

        /// <summary>
        /// Заполняет private или protected поле экземпляра
        /// </summary>
        /// <param name="obj"> Экземпляр </param>
        /// <param name="fieldName"> Название поля </param>
        /// <param name="newValue"> Значение </param>
        public static void SetPrivateField(this object obj, string fieldName, object newValue)
        {
            var prop = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            prop?.SetValue(obj, newValue);
        }

        public static bool IsNullable(this Type type)
        {
            if (type == null)
                return false;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        public static bool IsBase(this Type type)
        {
            return type.IsAbstract || type.Name.EndsWith("Base") || type.IsGenericTypeDefinition;
        }
        public static T GetAttribute<T>(this MemberInfo member, bool inherit = true, bool implement = true)
           where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), inherit).OfType<T>().FirstOrDefault();
            if (attribute != null)
                return attribute;

            if (!implement)
                return null;
            IEnumerable<Type> interfaces;
            var type = member as Type;
            if (type != null)
                interfaces = type.GetInterfaces();
            else if (member.DeclaringType != null)
                interfaces = member.DeclaringType.GetInterfaces();
            else
                return null;

            return interfaces.Select(i => i.GetMethod(member.Name))
                    .Where(m => m != null)
                    .Select(m => m.GetCustomAttributes(typeof(T), inherit).OfType<T>().FirstOrDefault())
                    .FirstOrDefault();
        }
        public static bool ExistAttribute<T>(this MemberInfo member, bool inherit = true, bool implement = true)
            where T : Attribute
        {
            return member.GetAttribute<T>(inherit, implement) != null;
        }
        public static Type[] GetGenericArguments(this Type type)
        {
            if (type == null || !type.IsGenericType)
                return null;
            var arguments = type.GenericTypeArguments;
            if (arguments == null || arguments.Length == 0)
                return null;
            return arguments;
        }
        public static Type GetFirstGenericArgument(this Type type)
        {
            if (type == null || !type.IsGenericType)
                return null;
            return type.GetGenericArguments().FirstOrDefault();
        }
        public static Type GetCollectionElementType(this Type collectionType)
        {
            var interfaces = collectionType.GetInterfaces().Where(t => t.IsGenericType).ToList();

            if (collectionType.IsGenericType)
                interfaces.Add(collectionType);

            var enumerableInterface = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return enumerableInterface?.GetGenericArguments()[0];
        }
        public static PropertyInfo GetToOneByToMany(Type declaredType, Type propertyType)
        {
            var manyType = GetCollectionElementType(propertyType);
            return manyType?.AvailableProperties().FirstOrDefault(p => p.PropertyType.IsAssignableFrom(declaredType));
        }
        public static PropertyInfo GetToManyByToOne(Type declaredType, Type propertyType)
        {
            return propertyType.AvailableProperties().FirstOrDefault(p =>
            {
                var elementType = p.PropertyType.GetCollectionElementType();
                if (elementType == null)
                    return false;

                return elementType.IsAssignableFrom(declaredType);

            });
        }
        public static string GetAssemblyShortNameByTypeName(string typeName)
        {
            return string.Join(".", typeName.Split('.').Take(2));
        }
        public static Type GetBaseGeneric(this Type type)
        {
            while (true)
            {
                if (type.IsGenericType)
                    return type;
                if (type.BaseType == null || type.BaseType == typeof (object))
                    return null;
                type = type.BaseType;
            }
        }
        public static bool IsGenericArgumentAllowType(this Type type, Type allowType)
        {
            var checkFirstArgument = type.IsGenericType;

            type = GetBaseGeneric(type);
            if (type == null)
                return false;

            var arguments = type.GetGenericArguments();

            if ((checkFirstArgument && arguments.Length != 1)
                || (checkFirstArgument && arguments.Length == 0))
                return false;
            return allowType.IsAssignableFrom(arguments[0]);
        }
        public static bool IsGenericConstraintAllowType(this Type type, Type allowType)
        {
            var checkFirstArgument = type.IsGenericType;
            var arguments = type.GetGenericArguments();
            if ((checkFirstArgument && arguments.Length != 1)
                || (checkFirstArgument && arguments.Length == 0))
                return false;
            if (!arguments[0].IsGenericParameter)
                return false;
            return arguments[0].GetGenericParameterConstraints().Any(gpc => gpc.IsAssignableFrom(allowType));
        }
        public static IEnumerable<Type> GetComponentServices(Type type, Type root)
        {
            var baseType = type.BaseType;
            var services = new List<Type>();
            Type genericArgument = null;
            while (true)
            {
                if (baseType == null)
                    return services;

                if (genericArgument == null && baseType.IsGenericType && !baseType.IsGenericTypeDefinition && baseType.GetGenericArguments().Length == 1)
                    genericArgument = baseType.GetGenericArguments()[0];

                if (baseType.IsGenericType)
                    baseType = baseType.GetGenericTypeDefinition();

                if (baseType == root)
                    break;

                if (baseType == typeof(object))
                    break;

                if (baseType.IsGenericType && genericArgument != null)
                    services.Add(baseType.MakeGenericType(genericArgument));

                baseType = baseType.BaseType;
            }
            return services;
        }
        public static bool IsMappingCompatibleNotAssociationType(Type sourcePropertyType, Type destPropertyType)
        {
            if (sourcePropertyType == destPropertyType)
                return true;
            if (sourcePropertyType.IsValueType)
            {
                var nullableUnderlyingType = Nullable.GetUnderlyingType(sourcePropertyType);
                if (nullableUnderlyingType != null)
                    sourcePropertyType = nullableUnderlyingType;
            }
            else
            {
                if (typeof(string) != sourcePropertyType)
                {
                    var collectionElementType = GetCollectionElementType(sourcePropertyType);
                    if (collectionElementType != null)
                        sourcePropertyType = collectionElementType;
                }
            }

            if (destPropertyType.IsValueType)
            {
                var nullableUnderlyingType = Nullable.GetUnderlyingType(destPropertyType);
                if (nullableUnderlyingType != null)
                    destPropertyType = nullableUnderlyingType;
            }
            else
            {
                if (typeof(string) != destPropertyType)
                {
                    var collectionElementType = GetCollectionElementType(destPropertyType);
                    if (collectionElementType != null)
                        destPropertyType = collectionElementType;

                }
            }
            if ((destPropertyType == typeof(decimal) || destPropertyType == typeof(double))
                && (sourcePropertyType == typeof(decimal) || sourcePropertyType == typeof(double)))
                return true;
            return destPropertyType == sourcePropertyType;
        }
    }
}
