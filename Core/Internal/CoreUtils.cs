using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Core.Utils;

namespace Core.Internal
{
    public static class CoreUtils
    {
        #region Dynamic

        public static DynamicContainer ToDynamic(this object source, bool recursive = false)
        {
            var container = new DynamicContainer(source) { Recursive = recursive };
            return container;
        }

        static DynamicContainer ToDynamic(this object source, bool recursive, IEnumerable<MemberInfo> exclude)
        {
            var container = new DynamicContainer(source) { Recursive = recursive };

            if (exclude != null)
                container.ExcludeProps.AddRange(exclude);

            return container;
        }

        public static DynamicContainer Exclude<T>(this DynamicContainer container, Expression<Func<T>> exclude)
        {
            if (container.Source == null)
                return container;

            if (exclude == null)
                return container;

            var mi = exclude.GetMemberInfo();
            if (mi != null)
                container.ExcludeProps.Add(mi);

            return container;
        }

        public class DynamicContainer
        {
            internal readonly object Source;
            internal IEnumerable<MemberInfo> ObjectProps;
            internal readonly List<MemberInfo> ExcludeProps = new List<MemberInfo>();
            internal bool Recursive;

            protected internal DynamicContainer(object source)
            {
                this.Source = source;
                if (this.Source == null)
                    return;

                this.ObjectProps = source.GetType().GetProperties();
            }

            public dynamic Object(Action<dynamic> objInternals = null)
            {
                if (this.Source == null)
                    return null;

                IDictionary<string, object> expando = new ExpandoObject();

                foreach (var prop in this.ObjectProps)
                {
                    if (this.ExcludeProps.Contains(prop))
                        continue;

                    var value = ((PropertyInfo)prop).GetValue(this.Source);

                    if (value != null)
                    {
                        var valueType = value.GetType();
                        if (valueType.IsValueType || valueType == typeof(string))
                            expando.Add(prop.Name, value);
                        else if (this.Recursive)
                            expando.Add(prop.Name, value.ToDynamic(true, this.ExcludeProps).Object());
                    }
                    else
                        expando.Add(prop.Name, null);
                }

                if (objInternals != null)
                    objInternals(expando);

                return expando;
            }
        }

        #endregion

        #region Linq

        /// <summary>
        /// Удаление элементов из коллекции
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iList">лист откуда удалять</param>
        /// <param name="itemsToRemove">отфильтрованный лист что удалять</param>
        public static void RemoveAll<T>(this IList<T> iList, IEnumerable<T> itemsToRemove)
        {
            var set = new HashSet<T>(itemsToRemove);

            var list = iList as List<T>;
            if (list == null)
            {
                int i = 0;
                while (i < iList.Count)
                {
                    if (set.Contains(iList[i]))
                        iList.RemoveAt(i);
                    else i++;
                }
            }
            else
            {
                list.RemoveAll(set.Contains);
            }
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
        {
            if (list != null)
                return list;
            return new List<T>();
        }
        #endregion

        #region Strings 
        public static string CamelToPascal(string camelCase)
        {
            if (camelCase.IsEmpty())
                return camelCase;
            return Regex.Replace(camelCase, @"^[a-z]|(?<=\.)[a-z]", m => m.ToString().ToUpper());
        }
        public static string PascalToCamel(string pascalCase)
        {
            if (pascalCase.IsEmpty())
                return pascalCase;
            return Regex.Replace(pascalCase, @"^[A-Z]|(?<=\.)[A-Z]", m => m.ToString().ToLower());
        }
        public static string StringFormat(this string source, params object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(source))
                return source;
            return String.Format(source, parameters);
        }
        public static string StringFormatIfExists(this string source, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0
                || parameters.Any(p =>
                {
                    if (p == null)
                        return true;
                    var ps = p as string;
                    if (ps == null)
                        return false;
                    return String.IsNullOrWhiteSpace(ps);
                }))
            {

                return null;
            }
            if (String.IsNullOrWhiteSpace(source))
                return source;
            return String.Format(source, parameters);
        }
        public static string StringJoin(this IEnumerable<string> strings, string separator = ", ")
        {
            return String.Join(separator, strings);
        }
        public static bool IsEmpty(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }
        public static bool IsEmpty(this DateTime value)
        {
            return value == default(DateTime);
        }
        public static bool IsEmpty(this int value)
        {
            return value == default(Int32);
        }
        public static bool IsEmpty(this Decimal value)
        {
            return value == default(Decimal);
        }
        public static bool IsEmpty<T>(this IList<T> value)
        {
            return value == null || value.Count == 0;
        }
        public static string NameWithoutEnd(this string value, string end)
        {
            if (value.IsEmpty() || end.IsEmpty())
                return value;
            if (value.Length == end.Length || !value.EndsWith(end))
                return value;
            return value.Substring(0, value.Length - end.Length);
        }
        public static string NameWithoutEnd(this string value, params string[] ends)
        {

            if (value.IsEmpty() || ends == null || ends.Length == 0)
                return value;
            foreach (var end in ends)
            {
                if (end.IsEmpty() || value.Length == end.Length || !value.EndsWith(end))
                    continue;
                return value.Substring(0, value.Length - end.Length);
            }
            return value;
        }

        /// <summary>
        /// Метод склоняет одно слово в зависимости от числа с ним связанного
        /// </summary>
        /// <param name="value">слово для склонения</param>
        /// <param name="number">число с ним связанное</param>
        /// <param name="onePostfix">постфикс для числа один</param>
        /// <param name="twoPostfix">постфикс для числа два</param>
        /// <param name="fivePostfix">постфикс для числа пять</param>
        [Obsolete("Использовать класс Склонятель из библиотеки Morpher.dll. Подключать его через LightInject.")]
        public static string DeclineWordByNumber(string value, int number,
            string onePostfix = "я", string twoPostfix = "и", string fivePostfix = "й")
        {
            string postfix;

            // По сути есть три варианта окончания обычных слов.
            // 1. Одна 'Позици' + 'я' 
            // 2. Две, три, четыре 'Позици' + 'и' 
            // 3. Пять, шесть, семь, восемь, девять, десять, одинандцать, двенадцать, тринадцать, четырнадцать
            //      'Позици' + 'й' 
            //

            // Если остаток от деления больше одного и меньше пяти 
            // при этом не равен 11,12,13,14
            //
            if ((number % 10 >= 1) && (number % 10 < 5) && (number % 100 != 11) && (number % 100 != 12) && (number % 100 != 13) && (number % 100 != 14))
            {
                //Если остаток от деления равен одному и не равен 11
                if ((number % 10 == 1) && (number % 100 != 11))
                    postfix = onePostfix;
                else
                    postfix = twoPostfix;
            }
            else
                postfix = fivePostfix;
            return $"{value}{postfix}";
        }

        public static DateTime? ToDate(this string value)
        {
            if (value.IsEmpty())
                return null;
            DateTime result;
            if (DateTime.TryParse(value,
                       CultureInfo.CreateSpecificCulture("ru-RU"),
                       DateTimeStyles.None,
                       out result))
                return result;
            return null;
        }

        /// <summary>
        /// Аналог стандартного метода TryParse 
        /// </summary>
        public static T To<T>(this object value, T defaultValue = default(T))
        {
            var decimalSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            var stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);
            var converter = TypeDescriptor.GetConverter(typeof(T));

            try
            {
                return (T)converter.ConvertFrom(stringValue);
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;

                try
                {
                    if (typeof(T).IsNumeric() && e is FormatException)
                        return (T)converter.ConvertFrom(stringValue.TrimEnd('0').TrimEnd(decimalSeparator));

                    var obj = (T)value;
                    return obj;
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        #endregion

        #region Object Graph
        public static IList<T> GetThisWithAncestors<T>(this T source, Func<T, T> parentAction)
        {
            var ancestors = new List<T>();
            if (source == null)
                return ancestors;
            ancestors.Add(source);
            GetAncestorsCore(source, parentAction, ancestors);
            return ancestors;
        }

        public static IList<T> GetAncestors<T>(this T source, Func<T, T> parentAction)
        {
            var ancestors = new List<T>();
            if (source == null)
                return ancestors;
            GetAncestorsCore(source, parentAction, ancestors);
            return ancestors;
        }

        private static void GetAncestorsCore<T>(this T source, Func<T, T> parentAction, List<T> ancestors)
        {
            var parent = parentAction(source);
            if (parent == null || ancestors.Contains(parent))
                return;
            ancestors.Add(parent);
            GetAncestorsCore(parent, parentAction, ancestors);
        }

        public static IList<T> GetThisWithDescendants<T>(this T entity, Func<T, IEnumerable<T>> childsAction)
        {
            var ancestors = new List<T>();
            if (entity == null)
                return ancestors;
            ancestors.Add(entity);
            GetDescendantsCore(entity, childsAction, ancestors);
            return ancestors;
        }

        public static IList<T> GetDescendants<T>(this T source, Func<T, IEnumerable<T>> childsAction)
        {
            var descendants = new List<T>();
            if (source == null)
                return descendants;
            GetDescendantsCore(source, childsAction, descendants);
            return descendants;
        }

        private static void GetDescendantsCore<T>(this T source, Func<T, IEnumerable<T>> childsAction, List<T> descendants)
        {
            var childs = childsAction(source);
            childs?.ForEach(ch =>
            {
                descendants.Add(ch);
                GetDescendantsCore(ch, childsAction, descendants);
            });
        }

        #endregion

        #region Try Catch

        /// <summary>
        /// Выполняет указанную функцию в блоке try catch, в случае ошибки возращает default результирующего типа функции
        /// </summary>
        /// <typeparam name="TSource"> Тип на объекте которого надо выполнить функцию </typeparam>
        /// <typeparam name="TResult"> Тип - результат работы функции </typeparam>
        /// <param name="source"> Экземпляр объекта на котором надо выполнить функцию </param>
        /// <param name="func"> Выполняемая функция </param>
        /// <returns> Результат работы функции или default </returns>
        public static TResult TryCall<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
            where TSource : class
        {
            if (source == null)
                return default(TResult);
            try
            {
                return func(source);
            }
            catch
            {
                return default(TResult);
            }
        }

        /// <summary>
        /// Выполняет указанную функцию в блоке try catch, в случае ошибки возращает default результирующего типа функции
        /// </summary>
        /// <typeparam name="TSource"> Тип на объекте которого надо выполнить функцию </typeparam>
        /// <typeparam name="TResult"> Тип - результат работы функции </typeparam>
        /// <param name="source"> Экземпляр объекта на котором надо выполнить функцию </param>
        /// <param name="func"> Выполняемая функция </param>
        /// <param name="default"> Параметр по умолчанию </param>
        /// <returns> Результат работы функции или default </returns>
        public static TResult TryCall<TSource, TResult>(this TSource source, Func<TSource, TResult> func, TResult @default)
            where TSource : class
        {
            if (source == null)
                return default(TResult);
            try
            {
                return func(source);
            }
            catch
            {
                return @default;
            }
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue result;
            dictionary.TryGetValue(key, out result);
            return result;
        }

        #endregion

        #region Cryptography

        /// <summary>
        /// Хеш от массива байт алгоритмом MD5 
        /// </summary>
        /// <param name="buffer"> Массив байт для хеширования, если null - результат тоже null </param>
        /// <returns> Upercase hex строка с хешем 16 байт </returns>
        public static string Md5Hash(this byte[] buffer)
        {
            if (buffer == null)
                return null;

            var hash = MD5.Create().ComputeHash(buffer);

            var s = new StringBuilder();
            foreach (var b in hash)
                s.Append(b.ToString("X2"));

            return s.ToString();
        }


        /// <summary>
        /// Хеш от строки алгоритмом MD5 
        /// </summary>
        /// <param name="strValue"> Строка для хеширования, если null - результат тоже null </param>
        /// <returns> Upercase hex строка с хешем 16 байт </returns>
        public static string Md5Hash(this string strValue)
        {
            return strValue == null ? null : Encoding.UTF8.GetBytes(strValue).Md5Hash();
        }

        /// <summary>
        /// Шифрование AES 256bit
        /// </summary>
        public static string Encrypt(this string plainText, string key)
        {
            // Check arguments.
            if (plainText.IsEmpty())
                throw new ArgumentNullException(nameof(plainText));

            byte[] encrypted = {};
            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                    return Convert.ToBase64String(encrypted);

                // Create a encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(
                    new PasswordDeriveBytes(key, null).GetBytes(32), 
                    new PasswordDeriveBytes(key, null).GetBytes(16)
                    );

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Дешифрование AES 256bit
        /// </summary>
        public static string Decrypt(this string cipherText, string key)
        {
            // Check arguments.
            if (cipherText.IsEmpty())
                throw new ArgumentNullException("cipherText");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(new PasswordDeriveBytes(key, null).GetBytes(32), new PasswordDeriveBytes(key, null).GetBytes(16));

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        #endregion

        #region JSON






        public class CamelCaseToPascalCaseExpandoObjectConverter : JsonConverter
        {
            //CHANGED
            //the ExpandoObjectConverter needs this internal method so we have to copy it
            //from JsonReader.cs
            internal static bool IsPrimitiveToken(JsonToken token)
            {
                switch (token)
                {
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        return true;
                    default:
                        return false;
                }
            }

            /// <summary>
            /// Writes the JSON representation of the object.
            /// </summary>
            /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The calling serializer.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // can write is set to false
            }

            /// <summary>
            /// Reads the JSON representation of the object.
            /// </summary>
            /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The object value.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return ReadValue(reader);
            }

            private object ReadValue(JsonReader reader)
            {
                while (reader.TokenType == JsonToken.Comment)
                {
                    if (!reader.Read())
                        throw new Exception("Unexpected end.");
                }

                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        return ReadObject(reader);
                    case JsonToken.StartArray:
                        return ReadList(reader);
                    default:
                        //CHANGED
                        //call to static method declared inside this class
                        if (IsPrimitiveToken(reader.TokenType))
                            return reader.Value;

                        //CHANGED
                        //Use string.format instead of some util function declared inside JSON.NET
                        throw new Exception(String.Format(CultureInfo.InvariantCulture, "Unexpected token when converting ExpandoObject: {0}", reader.TokenType));
                }
            }

            private object ReadList(JsonReader reader)
            {
                IList<object> list = new List<object>();

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.Comment:
                            break;
                        default:
                            object v = ReadValue(reader);

                            list.Add(v);
                            break;
                        case JsonToken.EndArray:
                            return list;
                    }
                }

                throw new Exception("Unexpected end.");
            }

            private object ReadObject(JsonReader reader)
            {
                IDictionary<string, object> expandoObject = new ExpandoObject();

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            //CHANGED
                            //added call to CamelToPascal method       
                            string propertyName = CamelToPascal(reader.Value.ToString());

                            if (!reader.Read())
                                throw new Exception("Unexpected end.");

                            object v = ReadValue(reader);

                            expandoObject[propertyName] = v;
                            break;
                        case JsonToken.Comment:
                            break;
                        case JsonToken.EndObject:
                            return expandoObject;
                    }
                }

                throw new Exception("Unexpected end.");
            }

            /// <summary>
            /// Determines whether this instance can convert the specified object type.
            /// </summary>
            /// <param name="objectType">Type of the object.</param>
            /// <returns>
            ///     <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
            /// </returns>
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(ExpandoObject));
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.
            /// </summary>
            /// <value>
            ///     <c>true</c> if this <see cref="JsonConverter"/> can write JSON; otherwise, <c>false</c>.
            /// </value>
            public override bool CanWrite
            {
                get { return false; }
            }
        }

        /// <summary>
        /// Сериализация любого объекта в JSON
        /// </summary>
        /// <param name="obj"> Объект </param>
        /// <returns> JSON </returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        /// <summary>
        /// Десериализация JSON в объект указанного типа
        /// </summary>
        /// <typeparam name="T"> Тип объекта</typeparam>
        /// <param name="json"> JSON </param>
        /// <returns> Объект </returns>
        public static T ParseJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Десериализация JSON в динамический объект
        /// </summary>
        /// <param name="json"> JSON </param>
        /// <returns> Динамический объект </returns>
        public static dynamic ParseJson(this string json)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(json);
        }

        #endregion

        #region Async
        private static readonly TaskFactory MyTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static void RunSync(Func<Task> func)
        {
            MyTaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return MyTaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }


        #endregion

        /// <summary>
        /// Сериализует реквест в формат Fiddler
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ToFiddlerString(this System.Web.HttpRequest request)
        {
            //http://stackoverflow.com/questions/19154860/log-http-requests-with-nlog

            var sb = new StringBuilder();

            sb.AppendLine($"{request.RequestType} {request.Url} {request.Params["SERVER_PROTOCOL"]}");

            var parsed = System.Web.HttpUtility.ParseQueryString(request.Headers.ToString());

            foreach (string key in parsed)
            {
                sb.Append(key + ": " + parsed[key] + "\n");
            }

            var inputStream = request.InputStream;
            inputStream.Position = 0;

            using (var reader = new StreamReader(inputStream))
            {
                var body = reader.ReadToEnd();
                sb.Append("\n" + body);
            }

            return sb.ToString();
        }

    }
}
