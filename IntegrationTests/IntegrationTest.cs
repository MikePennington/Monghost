using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Mongonizer;
using Ploeh.AutoFixture;
using Shouldly;

namespace IntegrationTests
{
    /// <summary>
    /// MongoDB is now started and stopped from inside the tests! Now need to have it running beforehand.
    /// </summary>
    [TestClass]
    public class IntegrationTest
    {
        private static Process _mongodProcess;

        private Fixture _fixture;
        private IMongoMapper _mongoMapper;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            _mongodProcess = new Process { StartInfo = { FileName = "mongod", Arguments = "--smallfiles --port 27027" } }; // Use non-standard port
            _mongodProcess.Start(); 
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _mongodProcess.CloseMainWindow();
        }

        [TestInitialize]
        public void Setup()
        {
            _fixture = new Fixture();

            // Create a new database with each test to ensure test isolation
            _mongoMapper = new MongoMapper("mongodb://127.0.0.1:27027", "monghost-integration-tests-" + Guid.NewGuid());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mongoMapper.Database.Drop();
        }

        [TestMethod]
        public void SaveShouldPopulateId()
        {
            var entityFromFixture = _fixture.Build<EntityWithObjectId>().Without(x => x.Id).Create();
            _mongoMapper.Save(entityFromFixture);

            entityFromFixture.Id.ShouldNotBe(ObjectId.Empty);
        }

        [TestMethod]
        public void ShouldPluralizeAndLowercaseCollectionName()
        {
            var entityFromFixture = _fixture.Build<EntityWithObjectId>().Without(x => x.Id).Create();
            _mongoMapper.Save(entityFromFixture);

            var entitytFromDb = _mongoMapper.Database.GetCollection<EntityWithObjectId>("entitywithobjectids").FindOne();
            entitytFromDb.ShouldNotBe(null);
        }

        [TestMethod]
        public void ShouldSaveAndFind()
        {
            var entityFromFixture = _fixture.Build<EntityWithObjectId>().Without(x => x.Id).Create();
            _mongoMapper.Save(entityFromFixture);

            var entitytFromDb = _mongoMapper.FindOne<EntityWithObjectId>(entityFromFixture.Id);

            entitytFromDb.Id.ShouldBe(entityFromFixture.Id);
            entitytFromDb.String.ShouldBe(entityFromFixture.String);
            entitytFromDb.Bool.ShouldBe(entityFromFixture.Bool);
            entitytFromDb.List.Count.ShouldBe(entityFromFixture.List.Count);
        }

        [TestMethod]
        public void ShouldRemove()
        {
            var entityFromFixture = _fixture.Build<EntityWithObjectId>().Without(x => x.Id).Create();
            _mongoMapper.Save(entityFromFixture);

            _mongoMapper.Remove<EntityWithObjectId>(entityFromFixture.Id);

            var entitytFromDb = _mongoMapper.FindOne<EntityWithObjectId>(entityFromFixture.Id);

            entitytFromDb.ShouldBe(null);
        }

        [TestMethod]
        public void ShouldRemoveWithEntityMethod()
        {
            var entityFromFixture = _fixture.Build<EntityWithObjectId>().Without(x => x.Id).Create();
            _mongoMapper.Save(entityFromFixture);

            _mongoMapper.Remove(entityFromFixture);

            var entitytFromDb = _mongoMapper.FindOne<EntityWithObjectId>(entityFromFixture.Id);

            entitytFromDb.ShouldBe(null);
        }

        [TestMethod]
        public void ShouldRemoveWithNonObjectIdId()
        {
            var entityFromFixture = _fixture.Build<EntityWithNonObjectIdId>().Create();
            _mongoMapper.Save(entityFromFixture);

            _mongoMapper.Remove<EntityWithNonObjectIdId>(entityFromFixture.Id);

            var entitytFromDb = _mongoMapper.FindOne<EntityWithNonObjectIdId>(entityFromFixture.Id);

            entitytFromDb.ShouldBe(null);
        }

        [TestMethod]
        public void ShouldRemoveWithNoId()
        {
            var entityFromFixture = _fixture.Build<EntityWithNonId>().Create();
            _mongoMapper.Save(entityFromFixture);

            _mongoMapper.Remove<EntityWithNonId>(entityFromFixture.Name);

            var entitytFromDb = _mongoMapper.FindOne<EntityWithNonId>(entityFromFixture.Name);

            entitytFromDb.ShouldBe(null);
        }

        [TestMethod]
        public void ShouldSaveAndFindObjectWithNonObjectIdId()
        {
            var entityFromFixture = _fixture.Build<EntityWithNonObjectIdId>().Create();
            _mongoMapper.Save(entityFromFixture);

            var entitytFromDb = _mongoMapper.FindOne<EntityWithNonObjectIdId>(entityFromFixture.Id);

            entitytFromDb.Id.ShouldBe(entityFromFixture.Id);
            entitytFromDb.String.ShouldBe(entityFromFixture.String);
            entitytFromDb.Bool.ShouldBe(entityFromFixture.Bool);
            entitytFromDb.List.Count.ShouldBe(entityFromFixture.List.Count);
        }

        [TestMethod]
        public void ShouldSaveAndFindEntityWithNoId()
        {
            var entityFromFixture = _fixture.Build<EntityWithNonId>().Create();
            _mongoMapper.Save(entityFromFixture);

            var entitytFromDb = _mongoMapper.FindOne<EntityWithNonId>(entityFromFixture.Name);

            entitytFromDb.Name.ShouldBe(entityFromFixture.Name);
            entitytFromDb.Age.ShouldBe(entityFromFixture.Age);
        }
    }

    public class EntityWithObjectId : MongoEntityWithObjectId
    {
        public override ObjectId Id { get; set; }
        public string String { get; set; }
        public bool Bool { get; set; }
        public List<int> List { get; set; }
    }

    public class EntityWithNonObjectIdId : MongoEntity
    {
        public Guid Id { get; set; }
        public string String { get; set; }
        public bool Bool { get; set; }
        public List<int> List { get; set; }
    }

    public class EntityWithNonId : MongoEntity
    {
        [BsonId]
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
