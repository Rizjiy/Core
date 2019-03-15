using Core.Internal.Dependency;
using System.Linq;
using LightInject;
using LinqToDB;
using LinqToDB.Data;
using NUnit.Framework;
using Core.NUnitTest.Tests.Services;
using LinqToDB.Linq;
using Core.Utils.Linq2Db;

namespace Core.NUnitTest.Tests.Extensions
{
    /// <summary>
    /// Тесты генерации SQL хинтов методом расширения LinqToDBUtils класса With
    /// </summary>
	[TestFixture]
	public class SqlHintsTests
	{
		private Scope _scope;
		private IServiceContainer _container;

		public PersonEntityService PersonEntityService { get; set; }

		[OneTimeSetUp]
		public void Init()
		{
            new DependencyInitializer()
				.TestMode(true)
				.ForAssembly(GetType().Assembly)
				.Init((dbConfig, container) =>
				{
					dbConfig["Core.NUnitTest.Tests.Extensions"] = "Core";

					_scope = container.BeginScope();
					_container = container;

				});

			_container.InjectProperties(this);

			// Создаем таблицы используемые для генерации Euid-последовательностей
			using (DataConnection context = new DataConnection("Core"))
			{
				var schema = context.DataProvider.GetSchemaProvider().GetSchema(context);

				if (!schema.Tables.Any(table => table.TableName == "Table"))
					context.CreateTable<Sequences.TableEntity>();

				if (!schema.Tables.Any(table => table.TableName == "Person"))
					context.CreateTable<Sequences.PersonEntity>();

                if (!schema.Tables.Any(table => table.TableName == "Test"))
                    context.CreateTable<TestEntity>();
            }
		}

		[OneTimeTearDown]
		public virtual void Dispose()
		{
			using (DataConnection context = new DataConnection("Core"))
			{
				context.DropTable<Sequences.TableEntity>();
				context.DropTable<Sequences.PersonEntity>();
                context.DropTable<TestEntity>();
            }

			_scope.Dispose();
		}

        /// <summary>
        /// простейший пример селекта к одной таблицы
        /// </summary>
		[Test]
        public void With_SelectToSingleTable()
        {
            //=========-----------action
            var query = PersonEntityService.GetQuery().With("NOLOCK");


            //=========-----------assertion

            //проверяем что sql содержит хинты
            var sqlText = ((IExpressionQuery<Sequences.PersonEntity>)query).SqlText;

            StringAssert.Contains("[CoreTest].[dbo].[Person] [t1] WITH (NOLOCK)", sqlText);

            //проверяем что sql отрабатывает без эксепшна
            Assert.DoesNotThrow(() => {
                var res = query.ToList();
            });
        }

        /// <summary>
        /// join по первичным ключам на обоих сторонах
        /// </summary>
		[Test]
        public void With_JoinWithBothPrimaryKeysOnBothSides()
        {
            //=========-----------action
            var query = from p in PersonEntityService.GetQuery().With("NOLOCK")
                        join t in PersonEntityService.DataContext.GetTable<Sequences.TableEntity>().With("NOLOCK") on p.Id equals t.Id
                        select p;


            //=========-----------assertion

            //проверяем что sql содержит хинты
            var sqlText = ((IExpressionQuery<Sequences.PersonEntity>)query).SqlText;

            StringAssert.Contains("[CoreTest].[dbo].[Person] [p] WITH (NOLOCK)", sqlText);
            StringAssert.Contains("[CoreTest].[dbo].[Table] [t] WITH (NOLOCK)", sqlText);

            //проверяем что sql отрабатывает без эксепшна
            Assert.DoesNotThrow(() => {
                var res = query.ToList();
            });
        }

        /// <summary>
        /// join с первичным ключом на одной стороне
        /// </summary>
        [Test]
        public void With_JoinPrimaryKeyOnOneSide()
        {
            //=========-----------action
            var query =
                    from p in PersonEntityService.GetQuery().With("READCOMMITTED")
                    join t in PersonEntityService.DataContext.GetTable<TestEntity>().With("READCOMMITTED") on p.Id equals t.Id
                    select p;


            //=========-----------assertion

            //проверяем что sql содержит хинты
            var sqlText = ((IExpressionQuery<Sequences.PersonEntity>)query).SqlText;

            StringAssert.Contains("[CoreTest].[dbo].[Person] [p] WITH (READCOMMITTED)", sqlText);
            StringAssert.Contains("[CoreTest].[dbo].[Test] [t] WITH (READCOMMITTED)", sqlText);

            //проверяем что sql отрабатывает без эксепшна
            Assert.DoesNotThrow(() => {
                var res = query.ToList();
            });

            
        }
    }
}
