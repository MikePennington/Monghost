using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Mongonizer
{
    public interface IMongoMapper
    {
        MongoDatabase Database { get; }
        MongoCollection<T> GetCollection<T>() where T : MongoEntity;
        T FindOne<T>(BsonValue id) where T : MongoEntity;
        void Save<T>(T entity) where T : MongoEntity;
        void Remove<T>(BsonValue id) where T : MongoEntity;
    }

    public class MongoMapper : IMongoMapper
    {
        private static readonly Dictionary<Type, string> CollectionNameMap = new Dictionary<Type, string>();

        private static MongoDatabase _database;

        public MongoMapper(string connectionString, string databaseName)
        {
            if (_database == null)
            {
                var client = new MongoClient(connectionString);
                var server = client.GetServer();
                _database = server.GetDatabase(databaseName);
            }
        }

        public MongoDatabase Database { get { return _database; } }

        public MongoCollection<T> GetCollection<T>() where T : MongoEntity
        {
            // Check dictionary first
            string collectionName;
            if (CollectionNameMap.ContainsKey(typeof(T)))
            {
                collectionName = CollectionNameMap[typeof(T)];
            }
            else
            {
                collectionName = BuildCollectionName(typeof(T));
                CollectionNameMap.Add(typeof(T), collectionName);
            }

            var collection = Database.GetCollection<T>(collectionName);
            return collection;
        }

        public T FindOne<T>(BsonValue id) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            var instance = Activator.CreateInstance<T>();
            var query = Query.EQ(instance.GetIdName(), id);
            return collection.FindOne(query);
        }

        public void Save<T>(T entity) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            collection.Save(entity);
        }

        public void Remove<T>(BsonValue id) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            var instance = Activator.CreateInstance<T>();
            var query = Query.EQ(instance.GetIdName(), id);
            collection.Remove(query);
        }

        public string BuildCollectionName(Type type)
        {
            var name = type.Name.ToLower();
            return Pluralization.Pluralize(name);
        }
    }
}