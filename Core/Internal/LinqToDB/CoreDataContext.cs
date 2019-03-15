using Core.Services;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Internal.LinqToDB
{
    /// <summary>
    /// Шлюз к датаконтексту
    /// </summary>
    public class CoreDataContext : IDisposable, IDataConnection
    {

        /// <summary>
		/// Соединение БД
		/// </summary>
		public readonly DataConnection _dataConnection;

        #region DataConnection

        public int CommandTimeout {
            get
            {
                return _dataConnection.CommandTimeout;
            }
            set
            {
                _dataConnection.CommandTimeout = value;
            }
        }

        #endregion

        public CoreDataContext(string connectionString)
        {
            _dataConnection = new DataConnection(connectionString);
            _dataConnection.MappingSchema.ColumnNameComparer = StringComparer.OrdinalIgnoreCase;
        }

        public IQueryable<T> GetTable<T>() where T : class
        {
            return _dataConnection.GetTable<T>();
        }


        #region DataConnectionExtensions

        public T Execute<T>(string sql, object parameters)
        {
            return _dataConnection.Execute<T>(sql, parameters);
        }
        public T Execute<T>(string sql, DataParameter parameter)
        {
            return _dataConnection.Execute<T>(sql, parameter);
        }
        public T Execute<T>(string sql, params DataParameter[] parameters)
        {
            return _dataConnection.Execute<T>(sql, parameters);
        }
        public T Execute<T>(string sql)
        {
            return _dataConnection.Execute<T>(sql);
        }
        public int Execute(string sql, object parameters)
        {
            return _dataConnection.Execute(sql, parameters);
        }
        public int Execute(string sql, params DataParameter[] parameters)
        {
            return _dataConnection.Execute(sql, parameters);
        }
        public int Execute(string sql)
        {
            return _dataConnection.Execute(sql);
        }
        public int ExecuteProc(string sql, params DataParameter[] parameters)
        {
            return _dataConnection.ExecuteProc(sql, parameters);
        }
        public T ExecuteProc<T>(string sql, params DataParameter[] parameters)
        {
            return _dataConnection.ExecuteProc<T>(sql, parameters);
        }
        public DataReader ExecuteReader(string sql, CommandType commandType, CommandBehavior commandBehavior, params DataParameter[] parameters)
        {
            return _dataConnection.ExecuteReader(sql, commandType, commandBehavior, parameters);
        }
        public DataReader ExecuteReader(string sql)
        {
            return _dataConnection.ExecuteReader(sql);
        }
        public DataReader ExecuteReader(string sql, DataParameter parameter)
        {
            return _dataConnection.ExecuteReader(sql, parameter);
        }
        public DataReader ExecuteReader(string sql, object parameters)
        {
            return _dataConnection.ExecuteReader(sql, parameters);
        }
        public DataReader ExecuteReader(string sql, params DataParameter[] parameters)
        {
            return _dataConnection.ExecuteReader(sql, parameters);
        }
        public int Merge<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, string tableName = null, string databaseName = null, string schemaName = null) where T : class
        {
            return _dataConnection.Merge<T>(source, predicate, tableName, databaseName, schemaName);
        }
        public int Merge<T>(bool delete, IEnumerable<T> source, string tableName = null, string databaseName = null, string schemaName = null) where T : class
        {
            return _dataConnection.Merge<T>(delete, source, tableName, databaseName, schemaName);
        }
        public int Merge<T>(IEnumerable<T> source, string tableName = null, string databaseName = null, string schemaName = null) where T : class
        {
            return _dataConnection.Merge<T>(source, tableName, databaseName, schemaName);
        }
        public IEnumerable<T> Query<T>(T template, string sql, object parameters)
        {
            return _dataConnection.Query<T>(template, sql, parameters);
        }
        public IEnumerable<T> Query<T>(T template, string sql, params DataParameter[] parameters)
        {
            return _dataConnection.Query<T>(template, sql, parameters);
        }
        public IEnumerable<T> Query<T>(string sql, object parameters)
        {
            return _dataConnection.Query<T>(sql, parameters);
        }
        public IEnumerable<T> Query<T>(string sql, DataParameter parameter)
        {
            return _dataConnection.Query<T>(sql, parameter);
        }
        public IEnumerable<T> Query<T>(string sql, params DataParameter[] parameters)
        {
            return _dataConnection.Query<T>(sql, parameters);
        }
        public IEnumerable<T> Query<T>(string sql)
        {
            return _dataConnection.Query<T>(sql);
        }
        public IEnumerable<T> Query<T>(Func<IDataReader, T> objectReader, string sql, object parameters)
        {
            return _dataConnection.Query<T>(objectReader, sql, parameters);
        }
        public IEnumerable<T> Query<T>(Func<IDataReader, T> objectReader, string sql, params DataParameter[] parameters)
        {
            return _dataConnection.Query<T>(objectReader, sql, parameters);
        }
        public IEnumerable<T> Query<T>(Func<IDataReader, T> objectReader, string sql)
        {
            return _dataConnection.Query<T>(objectReader, sql);
        }
        public IEnumerable<T> QueryProc<T>(string sql, params DataParameter[] parameters)
        {
            return _dataConnection.QueryProc<T>(sql, parameters);
        }
        public IEnumerable<T> QueryProc<T>(Func<IDataReader, T> objectReader, string sql, params DataParameter[] parameters)
        {
            return _dataConnection.QueryProc<T>(objectReader, sql, parameters);
        }
        public CommandInfo SetCommand(string commandText, object parameters)
        {
            return _dataConnection.SetCommand(commandText);
        }

        #endregion

        #region DataExtensions

        public int Delete<T>(T obj)
        {
            return _dataConnection.Delete<T>(obj);
        }
        public int Insert<T>(T obj, string tableName = null, string databaseName = null, string schemaName = null)
        {
            return _dataConnection.Insert<T>(obj, tableName, databaseName, schemaName);
        }
        public int InsertOrReplace<T>(T obj)
        {
            return _dataConnection.InsertOrReplace<T>(obj);
        }
        public object InsertWithIdentity<T>(T obj)
        {
            return _dataConnection.InsertWithIdentity<T>(obj);
        }
        public int Update<T>(T obj)
        {
            return _dataConnection.Update<T>(obj);
        }

        #endregion


        #region TRANSACTION

        public DataConnectionTransaction BeginTransaction()
        {
            return _dataConnection.BeginTransaction();
        }

        public DataConnectionTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return _dataConnection.BeginTransaction(isolationLevel);
        }

        public void CommitTransaction()
        {
            _dataConnection.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _dataConnection.RollbackTransaction();
        }

        #endregion



        public void Dispose()
        {
            _dataConnection.Dispose();
        }

        public BulkCopyRowsCopied BulkCopy<T>(BulkCopyOptions options, IEnumerable<T> source) where T: class
        {
            return _dataConnection.BulkCopy(options, source);
        }

        public BulkCopyRowsCopied BulkCopy<T>(IEnumerable<T> source) where T : class
        {
            return _dataConnection.BulkCopy(source);
        }

        public BulkCopyRowsCopied BulkCopy<T>(int maxBatchSize, IEnumerable<T> source) where T : class
        {
            return _dataConnection.BulkCopy(maxBatchSize, source);
        }
    }
}
