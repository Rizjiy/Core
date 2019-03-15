using Core.Utils;
using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.NUnitTest.Tests.Extensions
{
    [TestFixture]
    public class EnumDisplayNameTests
    {
        /// <summary>
        /// Тестирования получения единственного имени для перечисления
        /// </summary>
        [Test]
        public void GetSingleNameTest()
        {
            // Подготовка
            var tEnum = SingleTestEnum.Test1;

            // Действие
            var name = tEnum.DisplayName();

            // Проверка
            Assert.AreEqual("ТЕСТ_1111", name);
        }


        /// <summary>
        /// Тестирования получения нескольких имен для перечисления
        /// </summary>
        [Test]
        public void GetSomeNamesTest()
        {
            // Подготовка
            TestEnum tEnum = TestEnum.Test1 | TestEnum.Test2 | TestEnum.Test3 | TestEnum.Test4;

            // Действие
            var name = tEnum.DisplayName();

            // Сравнение
            Assert.AreEqual("Test11, Test22, Test3, Test4", name);
        }


        /// <summary>
        /// Задание разделителя при получении нескольких наименований из перечисления
        /// </summary>
        [Test]
        public void GetSomeNamesWithSeparatorTest()
        {
            // Подготовка
            TestEnum tEnum = TestEnum.Test1 | TestEnum.Test2;

            // Действие
            var name = tEnum.DisplayName("; ");

            // Сравнение
            Assert.AreEqual("Test11; Test22", name);
        }
    }


    public enum SingleTestEnum
    {
        [Display(Name = "ТЕСТ_1111")]
        Test1,
        [Display(Name = "ТЕСТ_2222")]
        Test2
    }

    /// <summary>
    /// Для задания нескольких значений у перечисления,
    /// перечисление должно быть помеченно атрибутом FlagsAttribute,
    /// а значения перечисления соответствовать двойке в степене n.
    /// </summary>
    [Flags]
    public enum TestEnum
    {
        [Display(Name = "Test11")]
        Test1 = 1,

        [Display(Name = "Test22")]
        Test2 = 2,
        /// <summary>
        /// Свойство Name не задано - выводим значение наименования
        /// </summary>
        [Display]
        Test3 = 4,

        /// <summary>
        /// Атрибут DisplayAttribute не задан - выводим значение наименования
        /// </summary>
        Test4 = 8
    }
}
