using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace Core.Services
{
    public interface IDataConnection
    {
        int CommandTimeout { get; set; }

        DataConnectionTransaction BeginTransaction();
        DataConnectionTransaction BeginTransaction(IsolationLevel isolationLevel);
        BulkCopyRowsCopied BulkCopy<T>(BulkCopyOptions options, IEnumerable<T> source) where T: class;
        BulkCopyRowsCopied BulkCopy<T>(IEnumerable<T> source) where T: class;
        BulkCopyRowsCopied BulkCopy<T>(int maxBatchSize, IEnumerable<T> source) where T : class;
        void CommitTransaction();
        int Delete<T>(T obj);
        void Dispose();
        int Execute(string sql);
        int Execute(string sql, object parameters);
        int Execute(string sql, params DataParameter[] parameters);
        T Execute<T>(string sql);
        T Execute<T>(string sql, DataParameter parameter);
        T Execute<T>(string sql, object parameters);
        T Execute<T>(string sql, params DataParameter[] parameters);
        int ExecuteProc(string sql, params DataParameter[] parameters);
        T ExecuteProc<T>(string sql, params DataParameter[] parameters);
        DataReader ExecuteReader(string sql);
        DataReader ExecuteReader(string sql, CommandType commandType, CommandBehavior commandBehavior, params DataParameter[] parameters);
        DataReader ExecuteReader(string sql, DataParameter parameter);
        DataReader ExecuteReader(string sql, object parameters);
        DataReader ExecuteReader(string sql, params DataParameter[] parameters);
        IQueryable<T> GetTable<T>() where T : class;
        int Insert<T>(T obj, string tableName = null, string databaseName = null, string schemaName = null);
        int InsertOrReplace<T>(T obj);
        object InsertWithIdentity<T>(T obj);
        int Merge<T>(bool delete, IEnumerable<T> source, string tableName = null, string databaseName = null, string schemaName = null) where T : class;
        int Merge<T>(IEnumerable<T> source, string tableName = null, string databaseName = null, string schemaName = null) where T : class;
        int Merge<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, string tableName = null, string databaseName = null, string schemaName = null) where T : class;
        IEnumerable<T> Query<T>(Func<IDataReader, T> objectReader, string sql);
        IEnumerable<T> Query<T>(Func<IDataReader, T> objectReader, string sql, object parameters);
        IEnumerable<T> Query<T>(Func<IDataReader, T> objectReader, string sql, params DataParameter[] parameters);
        IEnumerable<T> Query<T>(string sql);
        IEnumerable<T> Query<T>(string sql, DataParameter parameter);
        IEnumerable<T> Query<T>(string sql, object parameters);
        IEnumerable<T> Query<T>(string sql, params DataParameter[] parameters);
        IEnumerable<T> Query<T>(T template, string sql, object parameters);
        IEnumerable<T> Query<T>(T template, string sql, params DataParameter[] parameters);
        IEnumerable<T> QueryProc<T>(Func<IDataReader, T> objectReader, string sql, params DataParameter[] parameters);
        IEnumerable<T> QueryProc<T>(string sql, params DataParameter[] parameters);
        void RollbackTransaction();
        CommandInfo SetCommand(string commandText, object parameters);
        int Update<T>(T obj);
    }
}