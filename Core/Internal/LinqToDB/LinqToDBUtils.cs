using System;
using System.Linq;
using Core.Domain;
using LinqToDB.Mapping;
using Core.Utils;

namespace Core.Internal.LinqToDB
{
    // ReSharper disable once InconsistentNaming
    public static class LinqToDBUtils
    {
        /// <summary>
        /// Возвращает название таблицы для типа
        /// </summary>
        /// <param name="type">Тип</param>
        /// <returns></returns>
        public static string GetTableName(this Type type)
        {
            var attribute =
                type.GetCustomAttributes(typeof(TableAttribute), false)
                .FirstOrDefault() as TableAttribute;

            // Получить название таблицы.
            var tableName = attribute == null ||
                string.IsNullOrWhiteSpace(attribute.Name)
                ? type.Name
                : attribute.Name;

            return tableName;
        }

        /// <summary>
        /// Заполнение полей текщего экземпляра, маркированных в ассоцияациях аргументом ThisKey
        /// на основе связанных (ассоциированных) сущностей.
        /// </summary>
        public static T FillKeys<T>(this T entity)
            where T:EntityBase
        {
            if (entity == null)
                return null;

            var entityTypeProps = entity.GetType().GetProperties();
            entityTypeProps.ForEach(assProp =>
            {
                var associationArgs = assProp.GetCustomAttributesData().FirstOrDefault(cad => cad.AttributeType == typeof(AssociationAttribute))?.NamedArguments;
                if (associationArgs == null)
                    return; // Не задан атрибут или аргументы ассоциации

                var linkEntity = assProp.GetValue(entity);
                if (linkEntity == null)
                    return; // Нет связанной сущности

                var otherKeyAttrib = associationArgs.FirstOrDefault(na => na.MemberName.Equals("OtherKey"));
                var otherKeyPropName = otherKeyAttrib.TypedValue.Value.ToString();
                var otherKeyProp = linkEntity.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals(otherKeyPropName));
                if (otherKeyProp == null)
                    return;  // Не задан аргумент "OtherKey" //todo Возможно надо использовать Id
                var otherKeyValue = otherKeyProp.GetValue(linkEntity);

                var thisKeyAttrib = associationArgs.FirstOrDefault(na => na.MemberName.Equals("ThisKey"));
                var thisKeyPropName = thisKeyAttrib.TypedValue.Value.ToString();
                var thisKeyProp = entityTypeProps.FirstOrDefault(p => p.Name.Equals(thisKeyPropName));
                if (thisKeyProp == null)
                    return; // Не задан аргумент "ThisKey"

                thisKeyProp.SetValue(entity, otherKeyValue);
            });

            return entity;
        }
    }
}
