using Core.Internal.LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
//using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LightInject;
using LinqToDB.Mapping;
using Core.Utils;
using Core.Domain;
using Core.Services;
using Core.Utils.CustomeException;

namespace Core.Fakes
{
    /// <summary>
    /// Фейковый контекст. Нужен для тестов LimqToDb запросов без БД
    /// </summary>
    public class DataContextFake : IDisposable, IDataConnection
    {
        private IServiceContainer _container;

        /// <summary>
		///убрать
		/// </summary>
		protected dynamic _dataConnection;

        public int CommandTimeout { get; set; }
        

        public DataContextFake(IServiceContainer container)
        {
            _container = container;

            _dataConnection = new System.Dynamic.ExpandoObject();
        }


        public IQueryable<T> GetTable<T>() where T : class
        {

            return _container.GetInstance<IList<T>>().AsQueryable();

        }


        #region DataConnectionExtensions

        public BulkCopyRowsCopied BulkCopy<T>(BulkCopyOptions options, IEnumerable<T> source) where T: class
        {
            return _dataConnection.BulkCopy<T>(options, source);
        }
        public BulkCopyRowsCopied BulkCopy<T>(IEnumerable<T> source) where T : class
        {
            return _dataConnection.BulkCopy<T>(source);
        }
        public BulkCopyRowsCopied BulkCopy<T>(int maxBatchSize, IEnumerable<T> source) where T : class
        {
            return _dataConnection.BulkCopy<T>(maxBatchSize, source);
        }
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
            //Делаем только для EntityBase
            var entity = obj as EntityBase;
            if (entity == null)
                throw new InvalidCastException($"Метод работает только с типом EntityBase. Type:{obj.GetType()}");

            var list = GetList<T>();

            int index = list.Cast<EntityBase>().FindIndex((e) => e.Id == entity.Id);
            if (index >= 0)
            {
                list.RemoveAt(index);
                return 1;
            }
            else
                return 0;

        }

        public int Insert<T>(T obj, string tableName = null, string databaseName = null, string schemaName = null)
        {

            var list = GetList<T>();
            list.Add(obj);

            return 1;
        }

        public int InsertOrReplace<T>(T obj)
        {
            //Делаем только для EntityBase
            var entity = obj as EntityBase;
            if (entity == null)
                throw new InvalidCastException($"Метод работает только с типом EntityBase. Type:{obj.GetType()}");

            var list = GetList<T>();

            int index = list.Cast<EntityBase>().FindIndex((e) => e.Id == entity.Id);
            if (index >= 0)
                list[index] = obj;
            else
                list.Add(obj);

            return 1;
        }

        public object InsertWithIdentity<T>(T obj)
        {
            //Делаем только для EntityBase
            var entity = obj as EntityBase;
            if (entity == null)
                throw new InvalidCastException($"Метод работает только с типом EntityBase. Type:{obj.GetType()}");

            int maxIndex = 0;

            //Ищем последнюю запись
            var list = GetList<T>();
            if (list.Any())
                maxIndex = list.Cast<EntityBase>().Select(e => e.Id).Max();

            entity.Id = ++maxIndex;

            list.Add(obj);

            return maxIndex;
        }

        public int Update<T>(T obj)
        {
            //Делаем только для EntityBase
            var entity = obj as EntityBase;
            if (entity == null)
                throw new InvalidCastException($"Метод работает только с типом EntityBase. Type:{obj.GetType()}");

            var list = GetList<T>();
            int index = list.Cast<EntityBase>().FindIndex((e)=> e.Id == entity.Id);

            if (index == -1)
                throw new NotFoundException("Не найден элемент в коллекции!");
                
            list[index] = obj;
            return 1;
        }

        #endregion


        #region TRANSACTION

        public DataConnectionTransaction BeginTransaction()
        {
            return new DataConnectionTransaction(new DataConnection(new FakeDataProvider(), String.Empty));
        }

        public DataConnectionTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return new DataConnectionTransaction(new DataConnection(new FakeDataProvider(),String.Empty));
        }

        public void CommitTransaction()
        {
            
        }

        public void RollbackTransaction()
        {
            
        }

        #endregion



        public void Dispose()
        {
            
        }

        /// <summary>
        /// Вспомагательный метод. Приводит ОБРАТНО к типу IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IList<T> GetList<T>()
        {
            var table = _container.GetInstance<IList<T>>();

            return table;
        }

        public void QueryHint(string hint)
        {
           
        }
    }
}
