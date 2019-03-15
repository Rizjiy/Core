using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Core.Utils
{
    /// <summary>
    /// Класс с методами для перечислений
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Отображает имя свойства Name атрибута DisplayAttribute.
        /// </summary>
        /// <param name="value">Значение перечисления</param>
        /// <param name="separator">Разделитель между именами. По умолчанию ", "</param>
        /// <returns></returns>
        public static string DisplayName(this Enum value, string separator = null)
        {
            Type type = value.GetType();

            // Массив реальные названий значения перечисления
            string[] enumNames = value.ToString().Split(new string[] { ", " }, StringSplitOptions.None);

            // Инициализация массива с displayName
            string[] displayNames = new string[enumNames.Length];

            for (int i = 0; i < displayNames.Length; i++)
            {
                MemberInfo memberInfo = type.GetMember(enumNames[i])[0];
                DisplayAttribute attribute = memberInfo.GetCustomAttribute<DisplayAttribute>();

                // Если атрибут не указан или его свойство Name не заполнено, то просто предоставать его реальное название из значения.
                displayNames[i] = string.IsNullOrEmpty(attribute?.Name)
                    ? enumNames[i]
                    : attribute.Name;
            }

            // Соединить результат
            return string.Join(separator ?? ", ", displayNames);
        }
    }
}
