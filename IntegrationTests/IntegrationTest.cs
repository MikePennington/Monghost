using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonGhost;
using MongoDB.Bson;
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
            var person = _fixture.Build<Person>().Without(x => x.Id).Create();
            _mongoHelper.Save(person);

            person.Id.ShouldNotBe(ObjectId.Empty);
        }

        [TestMethod]
        public void ShouldSaveAndFind()
        {
            var person = _fixture.Build<Person>().Without(x => x.Id).Create();
            _mongoHelper.Save(person);

            var personFromDb = _mongoHelper.FindOne<Person>(person.Id);

            personFromDb.Id.ShouldBe(person.Id);
            personFromDb.Name.ShouldBe(person.Name);
            personFromDb.Addresses.Count.ShouldBe(person.Addresses.Count);
        }

        [TestMethod]
        public void ShouldRemove()
        {
            var person = _fixture.Build<Person>().Without(x => x.Id).Create();
            _mongoHelper.Save(person);

            _mongoHelper.Remove(person);

            var personFromDb = _mongoHelper.FindOne<Person>(person.Id);

            personFromDb.ShouldBe(null);
        }
    }

    public class Person : MongoEntity
    {
        public override ObjectId Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public List<Address> Addresses { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool Default { get; set; }
    }
}
