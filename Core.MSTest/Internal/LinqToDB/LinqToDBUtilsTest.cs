using Core.Domain;
using Core.Internal.LinqToDB;
using LinqToDB.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.MSTest.Internal.LinqToDB
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class LinqToDBUtilsTest
    {
        #region ChildEntity & ParentEntity
        [Table]
        public class ChildEntity : EntityBase
        {
            [PrimaryKey]
            [Column]
            public override int Id { get; set; }

            [Column]
            public string Name { get; set; }

        }
        [Table]
        public class ParentEntity : EntityBase
        {
            [PrimaryKey]
            [Column]
            public override int Id { get; set; }


            [Column]
            public int? ChildId { get; set; }

            [Association(OtherKey = "Id", ThisKey = "ChildId", CanBeNull = false), NotNull]
            public ChildEntity Child { get; set; }

            [Column]
            public int? ClientId { get; set; }

            [Association(OtherKey = "Id", ThisKey = "ClientId", CanBeNull = false), NotNull]
            public ChildEntity Client { get; set; }

        }
        #endregion

        private ChildEntity _childEntity;
        private ChildEntity _clientEntity;
        private ParentEntity _parentEntity;

        [TestInitialize]
        public void Init()
        {
            _childEntity = new ChildEntity
            {
                Id = 1,
                Name = "child1"
            };
            _clientEntity = new ChildEntity
            {
                Id = 2,
                Name = "client1"
            };
            _parentEntity = new ParentEntity
            {
                Id = 100
            };

        }

        [TestMethod]
        public void FillKeysTest()
        {
            _parentEntity.Child = _childEntity;
            _parentEntity.Client = _clientEntity;

            var result = _parentEntity.FillKeys();

            Assert.AreEqual(result.ChildId, 1);
            Assert.AreEqual(result.ClientId, 2);
            Assert.AreEqual(result, _parentEntity);
        }


        [TestMethod]
        public void FillKeysOnNullEntity()
        {
            // Подготовка.
            ParentEntity entity = null;

            // Процесс.
            var nullEntity = entity.FillKeys();

            // Проверка.
            Assert.IsNull(nullEntity);
        }


        [TestMethod]
        public void FillKeysOnNullAssociationProperties()
        {
            // Подготовка.
            var entity = new ParentEntity();
            entity.ClientId = -1;
            entity.Client = null;

            // Действие.
            var result = entity.FillKeys();

            // Проверка.
            Assert.AreEqual(-1, result.ClientId);

        }



    }
}
