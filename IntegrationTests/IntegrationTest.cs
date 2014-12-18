using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monghost;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Ploeh.AutoFixture;
using Shouldly;

namespace IntegrationTests
{
    /// <summary>
    /// MongoDB must be running locally at 127.0.0.1:27017 for these tests to run properly
    /// </summary>
    [TestClass]
    public class IntegrationTest
    {
        private static Fixture _fixture;
        private static IMongoHelper _mongoHelper;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new Fixture();

            // Create a new database with each test to ensure test isolation
            _mongoHelper = new MongoHelper("mongodb://127.0.0.1:27017", "monghost-integration-tests-" + Guid.NewGuid());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mongoHelper.Database.Drop();
        }

        [TestMethod]
        public void SaveShouldPopulateId()
        {
            var objectFromFixture = _fixture.Build<ObjectWithObjectId>().Without(x => x.Id).Create();
            _mongoHelper.Save(objectFromFixture);

            objectFromFixture.Id.ShouldNotBe(ObjectId.Empty);
        }

        [TestMethod]
        public void ShouldPluralizeAndLowercaseCollectionName()
        {
            var objectFromFixture = _fixture.Build<ObjectWithObjectId>().Without(x => x.Id).Create();
            _mongoHelper.Save(objectFromFixture);

            var objectFromDb = _mongoHelper.Database.GetCollection<ObjectWithObjectId>("objectwithobjectids").FindOne();
            objectFromDb.ShouldNotBe(null);
        }

        [TestMethod]
        public void ShouldSaveAndFind()
        {
            var objectFromFixture = _fixture.Build<ObjectWithObjectId>().Without(x => x.Id).Create();
            _mongoHelper.Save(objectFromFixture);

            var objectFromDb = _mongoHelper.FindOne<ObjectWithObjectId>(objectFromFixture.Id);

            objectFromDb.Id.ShouldBe(objectFromFixture.Id);
            objectFromDb.String.ShouldBe(objectFromFixture.String);
            objectFromDb.Bool.ShouldBe(objectFromFixture.Bool);
            objectFromDb.List.Count.ShouldBe(objectFromFixture.List.Count);
        }

        [TestMethod]
        public void ShouldRemove()
        {
            var objectFromFixture = _fixture.Build<ObjectWithObjectId>().Without(x => x.Id).Create();
            _mongoHelper.Save(objectFromFixture);

            _mongoHelper.Remove(objectFromFixture);

            var objectFromDb = _mongoHelper.FindOne<ObjectWithObjectId>(objectFromFixture.Id);

            objectFromDb.ShouldBe(null);
        }

        [TestMethod]
        public void ShouldSaveAndFindObjectWithStringId()
        {
            var objectFromFixture = _fixture.Build<ObjectWithStringId>().Create();
            _mongoHelper.Save<ObjectWithStringId, Guid>(objectFromFixture);

            var objectFromDb = _mongoHelper.FindOne<ObjectWithStringId, Guid>(objectFromFixture.Id);

            objectFromDb.Id.ShouldBe(objectFromFixture.Id);
            objectFromDb.String.ShouldBe(objectFromFixture.String);
            objectFromDb.Bool.ShouldBe(objectFromFixture.Bool);
            objectFromDb.List.Count.ShouldBe(objectFromFixture.List.Count);
        }

        [TestMethod]
        public void ShouldSaveAndFindEntityWithFunc()
        {
            var objectFromFixture = _fixture.Build<EntityWithNonId>().Create();
            _mongoHelper.SaveTest(objectFromFixture);

            var objectFromDb = _mongoHelper.FindOneTest<EntityWithNonId>(objectFromFixture.Name);

            objectFromDb.Name.ShouldBe(objectFromFixture.Name);
            objectFromDb.Age.ShouldBe(objectFromFixture.Age);
        }
    }

    public class ObjectWithObjectId : MongoEntity
    {
        public override ObjectId Id { get; set; }
        public string String { get; set; }
        public bool Bool { get; set; }
        public List<int> List { get; set; }
    }

    public class ObjectWithStringId : MongoEntity<Guid>
    {
        public override Guid Id { get; set; }
        public string String { get; set; }
        public bool Bool { get; set; }
        public List<int> List { get; set; }
    }

    public class EntityWithNonId : MongoTest
    {
        [BsonId]
        public string Name { get; set; }
        public int Age { get; set; }
        public override string GetIdName()
        {
            return "_id";
        }
    }
}
