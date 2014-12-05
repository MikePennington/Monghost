using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MonGhost
{
    public interface IMongoHelper
    {
        MongoDatabase Database { get; }
        MongoCollection<T> GetCollection<T>() where T : MongoEntity;
        T FindOne<T>(ObjectId id) where T : MongoEntity;
        T FindOne<T>(IMongoQuery query) where T : MongoEntity;
        List<T> Find<T>(IMongoQuery query) where T : MongoEntity;
        void Save<T>(T entity) where T : MongoEntity;
        void Insert<T>(T entity) where T : MongoEntity;
        void Remove<T>(T entity) where T : MongoEntity;
    }

    public class MongoHelper : IMongoHelper
    {
        private static readonly Dictionary<Type, string> CollectionNameMap = new Dictionary<Type, string>();

        private static MongoDatabase _database;

        public MongoHelper(string connectionString, string databaseName)
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
                collectionName = ((T)Activator.CreateInstance(typeof(T))).GetCollectionName();
                CollectionNameMap.Add(typeof(T), collectionName);
            }

            var collection = Database.GetCollection<T>(collectionName);
            return collection;
        }

        public T FindOne<T>(ObjectId id) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            var query = Query<T>.EQ(x => x.Id, id);
            return collection.FindOne(query);
        }

        public T FindOne<T>(IMongoQuery query) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            return collection.FindOne(query);
        }

        public List<T> Find<T>(IMongoQuery query) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            return collection.Find(query).ToList();
        }

        public void Save<T>(T entity) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            collection.Save(entity);
        }

        public void Insert<T>(T entity) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            collection.Insert(entity);
        }

        public void Remove<T>(T entity) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            var query = Query<T>.EQ(x => x.Id, entity.Id);
            collection.Remove(query);
        }
    }
}