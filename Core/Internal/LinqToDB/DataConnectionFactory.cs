using System;
using System.Collections.Concurrent;
using System.Linq;
using Core.Internal.Dependency;
using LinqToDB.Data;
using Core.LinqToDB.Interfaces;
#if DEBUG
using System.Threading;
using System.Diagnostics;
#endif

namespace Core.Internal.LinqToDB
{
    public class DataConnectionFactory : IDisposable, IDataConnectionFactory
    {
        // Каждой конфигурации положено только одно соединение в рамках HttpRequest-а
        private readonly ConcurrentDictionary<string, CoreDataContext> _connections = new ConcurrentDictionary<string, CoreDataContext>();

        public IDataConnection GetDataConnection(string configurationString)
        {
#if DEBUG
            Debug.WriteLine($@"DataConnectionFactory.GetDataConnection(""{configurationString}"");");
#endif
            return _connections.GetOrAdd(configurationString, s =>
            {
#if DEBUG
                var ts = new Stopwatch();
                ts.Start();
                try
                {
#endif
                    return new CoreDataContext(s);
#if DEBUG
                }
                finally
                {
                    ts.Stop();
                    Debug.WriteLine($@"DataConnectionFactory.Add(new DataConnection(""{s}"")); Connection added with {ts.ElapsedMilliseconds} ms");
                }
#endif
            });
        }

#if DEBUG
        public DataConnectionFactory()
        {
            Debug.WriteLine($"DataConnectionFactory.ctor(); Thread.CurrentThread.ManagedThreadId == {Thread.CurrentThread.ManagedThreadId}");
        }
#endif
        public void Dispose()
        {
            _connections.Keys.ToList().ForEach(k =>
            {
                CoreDataContext connection;
                if (_connections.TryRemove(k, out connection))
                    connection.Dispose();
            });
#if DEBUG
            Debug.WriteLine($"DataConnectionFactory.Dispose(); Thread.CurrentThread.ManagedThreadId == {Thread.CurrentThread.ManagedThreadId}");
#endif
        }
    }
}
