using System;

namespace Core.Domain.Attributes
{
    /// <summary>
    /// Атрибут для генерации уникального межтабличного идентификатора(УМИ) из последовательности
    /// Свойстов помечанное этим атрибутом при создании новой сущности получит значение УМИ
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EuidAttribute : Attribute
    {

    }
}
