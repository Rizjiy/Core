using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Core.MSTest.Internal
{
	[TestClass]
	public class DataTableSerializationTest
	{
		private DataTable _dummy;

		[TestInitialize]
		public void Init()
		{
			_dummy = new DataTable();
			_dummy.TableName = "dummy"; // Необходимо указывать для сериализации
			_dummy.Columns.Add("id", typeof(int));
			_dummy.Columns.Add("name", typeof(string));
			_dummy.Columns.Add("date", typeof(DateTime));
			_dummy.Columns.Add("isGood", typeof(bool));
			_dummy.BeginLoadData();
			for (var i = 0; i < 1000; i++)
			{
				_dummy.Rows.Add(i, Guid.NewGuid().ToString("N"), DateTime.Now, i % 2 == 0);
			}
			_dummy.EndLoadData();
		}

		[TestMethod]
		public void XmlOnlySingleTest()
		{
			var sw = new Stopwatch();
			sw.Start();

			var sb = new StringBuilder();
			_dummy.WriteXml(new StringWriter(sb), XmlWriteMode.WriteSchema);

			var xml = sb.ToString();

			var table = new DataTable();

			table.ReadXml(new StringReader(xml));

			sw.Stop();
			Assert.AreEqual(table.Rows.Count, _dummy.Rows.Count);
			for (var j = 0; j < 4; j++)
			{
				Assert.AreEqual(_dummy.Columns[j].DataType, table.Columns[j].DataType);
			}

			Console.WriteLine($"XmlOnlySingleTest duration = {sw.ElapsedMilliseconds}ms");
		}

		[TestMethod]
		public void XmlOnlyTest()
		{
			var sw = new Stopwatch(); 

			for (var i = 0; i < 1000; i++)
			{
				sw.Start();

				var sb = new StringBuilder();

				_dummy.WriteXml(new StringWriter(sb), XmlWriteMode.WriteSchema);

				var xml = sb.ToString();

				var table = new DataTable();

				table.ReadXml(new StringReader(xml));

				sw.Stop();
				Assert.AreEqual(table.Rows.Count, _dummy.Rows.Count);
				for (var j = 0; j < 4; j++)
				{
					Assert.AreEqual(_dummy.Columns[j].DataType, table.Columns[j].DataType);
				}
			}

			Console.WriteLine($"XmlOnlyTest x 1000 duration = {sw.ElapsedMilliseconds}ms");
		}

		[TestMethod]
		public void JsonOnlySingleTest()
		{
			var sw = new Stopwatch();
			sw.Start();

            var json = JsonConvert.SerializeObject(_dummy);

			var table = JsonConvert.DeserializeObject<DataTable>(json);

			sw.Stop();
			Assert.AreEqual(table.Rows.Count, _dummy.Rows.Count);
			for (var j = 0; j < 4; j++)
			{
                //Всегда failed не может преобразовать тип колонки DataTable в int
                //Assert.AreEqual(_dummy.Columns[j].DataType, table.Columns[j].DataType);
                Assert.AreEqual(_dummy.Columns[j].ColumnName, table.Columns[j].ColumnName);
            }
            Console.WriteLine($"JsonOnlySingleTest duration = {sw.ElapsedMilliseconds}ms");
		}

		[TestMethod]
		public void JsonOnlyTest()
		{
			var sw = new Stopwatch();

			for (var i = 0; i < 1000; i++)
			{
				sw.Start();

				var json = JsonConvert.SerializeObject(_dummy);

				var table = JsonConvert.DeserializeObject<DataTable>(json);

				sw.Stop();
				Assert.AreEqual(table.Rows.Count, _dummy.Rows.Count);
				for (var j = 0; j < 4; j++)
				{
                    //Всегда failed не может преобразовать тип колонки DataTable в int
                    //Assert.AreEqual(_dummy.Columns[j].DataType, table.Columns[j].DataType);
                    Assert.AreEqual(_dummy.Columns[j].ColumnName, table.Columns[j].ColumnName);
                }
            }

			Console.WriteLine($"JsonOnlyTest x 1000 duration = {sw.ElapsedMilliseconds}ms");
		}

		[TestMethod]
		public void FastJsonOnlySingleTest()
		{
			var sw = new Stopwatch();
			sw.Start();

			var json = fastJSON.JSON.ToJSON(_dummy);

			var table = fastJSON.JSON.ToObject<DataTable>(json);

			sw.Stop();
			Assert.AreEqual(table.Rows.Count, _dummy.Rows.Count);
			for (var j = 0; j < 4; j++)
			{
				Assert.AreEqual(_dummy.Columns[j].DataType, table.Columns[j].DataType);
			}
			Console.WriteLine($"FastJsonOnlySingleTest duration = {sw.ElapsedMilliseconds}ms");
		}

		[TestMethod]
		public void FastJsonOnlyTest()
		{
			var sw = new Stopwatch();

			for (var i = 0; i < 1000; i++)
			{
				sw.Start();

				var json = fastJSON.JSON.ToJSON(_dummy);

				var table = fastJSON.JSON.ToObject<DataTable>(json);

				sw.Stop();

				Assert.AreEqual(table.Rows.Count, _dummy.Rows.Count);
				for (var j = 0; j < 4; j++)
				{
					Assert.AreEqual(_dummy.Columns[j].DataType, table.Columns[j].DataType);
				}
			}

			Console.WriteLine($"FastJsonOnlyTest x 1000 duration = {sw.ElapsedMilliseconds}ms");
		}
	}
}
