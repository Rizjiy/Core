using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using System.Linq.Expressions;
using LinqToDB;


namespace Core.Utils.Linq2Db
{
    public static class Linq2DbExtensions
    {

        /// <summary>
        /// Метод должен применяться исключительно для типа ITable!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IQueryable<T> LoadWith<T>(this IQueryable<T> table, Expression<Func<T, object>> selector)
        {
            //Это чисто Linq2Db-шный метод.
            var iTable = table as ITable<T>;
            if (iTable != null)
                return LinqExtensions.LoadWith(iTable, selector);

            //throw new NotImplementedException("Метод LoadWith можно вызывать только на типе ITable!");
            return table;

        }

        [Sql.Expression("CAST({0} AS DECIMAL({1},{2}))")]
        public static decimal CastAsDecimal(this string num, int precision, int scale)
        {
            decimal res;
            decimal.TryParse(num, out res);
            return res;
        }

        [Sql.Expression("CAST({0} AS DECIMAL({1},{2}))")]
        public static decimal CastAsDecimal(this decimal num, int precision, int scale)
        {
            return num;
        }


        [Sql.Expression("CAST({0} AS DATE)")]
        public static DateTime CastAsDate(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        [Sql.Expression("REPLACE({0},{1},{2})")]
        public static string SqlReplace(this string sourceStr, string oldStr, string newStr)
        {
            return sourceStr.Replace(oldStr, newStr);
        }

        /// <summary>
        /// Как-то криво вызывается :(
        /// </summary>
        /// <returns></returns>
        [Sql.Function("NEWID", ServerSideOnly = true)]
        public static Guid NewId()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Если IQueryable<T> объект, на котором вызывается, реализует ITable<T>, 
        /// то к сгенерированному SQL для названия таблицы добавляется хинт WITH(<args>). 
        /// В противном случае возвращается без изменений исходный IQueryable<T>.   
        /// </summary>
        /// <typeparam name="T">тип объекта сущности данных</typeparam>
        /// <param name="query">исходный запрос</param>
        /// <param name="args">хинт строкой. Эта строка 1 в 1 вставится внутрь выражения WITH(<args>)</param>
        /// <returns>результирущий запрос</returns>
        public static IQueryable<T> With<T>(this IQueryable<T> query, string args) where T : class
        {
            ITable<T> table = query as ITable<T>;

            return table != null ? LinqExtensions.With(table, args) : query;
        }

    }
}
