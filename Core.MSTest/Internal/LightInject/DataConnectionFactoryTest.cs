using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Internal;
using Core.Internal.LinqToDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Utils;

namespace Core.MSTest.Internal.LightInject
{
    [TestClass]
    public class DataConnectionFactoryTest
    {
        private const string C1 = "C1";
        private const string C2 = "C2";
        private DataConnectionFactory _factory;
        private List<string> _confs;

        [TestInitialize]
        public void Init()
        {
            _factory = new DataConnectionFactory();
            _confs = new List<string>();
            var rnd = new Random();
            for (var i = 0; i < 100; i++)
            {
                _confs.Add(rnd.Next(0, 2) == 0 ? C1 : C2);
            }
        }

        [TestMethod]
        public void TestSerialEquals()
        {
            var con1 = _factory.GetDataConnection(C1);
            var con2 = _factory.GetDataConnection(C1);
            Assert.AreSame(con1, con2);
        }

        [TestMethod]
        public void TestSerialNotEquals()
        {
            var con1 = _factory.GetDataConnection(C1);
            var con2 = _factory.GetDataConnection(C2);
            Assert.AreNotSame(con1, con2);
        }

        [TestMethod]
        public void TestParallelEquals()
        {
            var bag = new ConcurrentBag<object>();

            Parallel.For(0, 100, new ParallelOptions
            {
                MaxDegreeOfParallelism = 100,
            }, i =>
            {
                bag.Add(_factory.GetDataConnection(C1));
                Console.WriteLine("Thread.CurrentThread.ManagedThreadId == {0}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(10); // Без задержки слишком много задач попадает в один поток
            });

            Assert.AreEqual(bag.Count, 100);

            object first;
            Assert.IsTrue(bag.TryTake(out first));

            bag.ForEach(item =>
            {
                Assert.AreSame(first, item);
            });
        }

        [TestMethod]
        public void TestParallelNotEquals()
        {
            var bag = new ConcurrentBag<object>();

            Parallel.ForEach(_confs, new ParallelOptions
            {
                MaxDegreeOfParallelism = 100,
            }, conf =>
            {
                bag.Add(_factory.GetDataConnection(conf));
                Console.WriteLine("Thread.CurrentThread.ManagedThreadId == {0}, conf == {1}", Thread.CurrentThread.ManagedThreadId, conf);
                Thread.Sleep(10); // Без задержки слишком много задач попадает в один поток
            });

            Assert.AreEqual(bag.Count, 100);

            object first;
            Assert.IsTrue(bag.TryTake(out first));

            var correct = 0;
            bag.ForEach(item =>
            {
                if (!object.ReferenceEquals(first, item))
                    correct++;
            });

            Console.WriteLine("correct == {0}", correct);
            Assert.IsTrue(correct >= 33);
        }
    }
}
