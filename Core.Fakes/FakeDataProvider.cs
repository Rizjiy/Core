using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using LinqToDB.SchemaProvider;
using LinqToDB.SqlProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Common;

namespace Core.Fakes
{
    /// <summary>
    /// https://github.com/linq2db/linq2db/blob/cf160d92b7062d1af7581abeb7185b20a0fdde94/Tests/Linq/Update/MergeTests.CommandValidation.cs
    /// </summary>
    public class FakeDataProvider : IDataProvider
    {
        string IDataProvider.ConnectionNamespace
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Type IDataProvider.DataReaderType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        MappingSchema IDataProvider.MappingSchema
        {
            get
            {
                return null;
            }
        }

        string IDataProvider.Name
        {
            get
            {
                return "TestProvider";
            }
        }

        SqlProviderFlags IDataProvider.SqlProviderFlags
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BulkCopyRowsCopied BulkCopy<T>(ITable<T> table, BulkCopyOptions options, IEnumerable<T> source)
        {
            throw new NotImplementedException();
        }

        public Type ConvertParameterType(Type type, DbDataType dataType)
        {
            throw new NotImplementedException();
        }

        public void SetParameter(IDbDataParameter parameter, string name, DbDataType dataType, object value)
        {
            throw new NotImplementedException();
        }

        //BulkCopyRowsCopied IDataProvider.BulkCopy<T>(DataConnection dataConnection, BulkCopyOptions options, IEnumerable<T> source)
        //    where T: class
        //{
        //    throw new NotImplementedException();
        //}

        //Type IDataProvider.ConvertParameterType(Type type, DataType dataType)
        //{
        //    throw new NotImplementedException();
        //}

        IDbConnection IDataProvider.CreateConnection(string connectionString)
        {
            throw new NotImplementedException();
        }

        ISqlBuilder IDataProvider.CreateSqlBuilder()
        {
            throw new NotImplementedException();
        }

        void IDataProvider.DisposeCommand(DataConnection dataConnection)
        {
            throw new NotImplementedException();
        }

        IDisposable IDataProvider.ExecuteScope()
        {
            throw new NotImplementedException();
        }

        CommandBehavior IDataProvider.GetCommandBehavior(CommandBehavior commandBehavior)
        {
            throw new NotImplementedException();
        }

        object IDataProvider.GetConnectionInfo(DataConnection dataConnection, string parameterName)
        {
            throw new NotImplementedException();
        }

        Expression IDataProvider.GetReaderExpression(MappingSchema mappingSchema, IDataReader reader, int idx, Expression readerExpression, Type toType)
        {
            throw new NotImplementedException();
        }

#if !NETSTANDARD1_6
        ISchemaProvider IDataProvider.GetSchemaProvider()
        {
            throw new NotImplementedException();
        }
#endif

        ISqlOptimizer IDataProvider.GetSqlOptimizer()
        {
            throw new NotImplementedException();
        }

        void IDataProvider.InitCommand(DataConnection dataConnection, CommandType commandType, string commandText, DataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        bool IDataProvider.IsCompatibleConnection(IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        bool? IDataProvider.IsDBNullAllowed(IDataReader reader, int idx)
        {
            throw new NotImplementedException();
        }

        int IDataProvider.Merge<T>(DataConnection dataConnection, Expression<Func<T, bool>> predicate, bool delete, IEnumerable<T> source, string tableName, string databaseName, string schemaName)
        {
            throw new NotImplementedException();
        }

        int IDataProvider.Merge<TTarget, TSource>(DataConnection dataConnection, IMergeable<TTarget, TSource> merge)
        {
            throw new NotImplementedException();
        }

        Task<int> IDataProvider.MergeAsync<T>(DataConnection dataConnection, Expression<Func<T, bool>> predicate, bool delete, IEnumerable<T> source, string tableName, string databaseName, string schemaName, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        Task<int> IDataProvider.MergeAsync<TTarget, TSource>(DataConnection dataConnection, IMergeable<TTarget, TSource> merge, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        //void IDataProvider.SetParameter(IDbDataParameter parameter, string name, DataType dataType, object value)
        //{
        //    throw new NotImplementedException();
        //}
    }

}
