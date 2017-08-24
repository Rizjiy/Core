using System;
using LinqToDB;

namespace Core.Internal.LinqToDB
{
    /// <summary>
    /// Расширение функций класса Linq2DB.SQL
    /// </summary>
    public static class MsSql
    {
	   [Sql.Function("IsNumeric", ServerSideOnly = true)]
	   public static bool IsNumeric(string s)
	   {
		  throw new InvalidOperationException();
	   }
    }
}
