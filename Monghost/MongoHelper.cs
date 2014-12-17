using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Monghost
{
    public interface IMongoHelper
    {
        MongoDatabase Database { get; }
        MongoCollection<T> GetCollection<T>() where T : MongoEntity;
        MongoCollection<T> GetCollection<T, K>() where T : MongoEntity<K>;

        T FindOne<T>(ObjectId id) where T : MongoEntity;
        T FindOne<T, K>(K id) where T : MongoEntity<K>;
        
        void Save<T>(T entity) where T : MongoEntity;
        void Save<T, K>(T entity) where T : MongoEntity<K>;
        
        void Remove<T>(T entity) where T : MongoEntity;
        void Remove<T, K>(T entity) where T : MongoEntity<K>;
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
            return GetCollection<T, ObjectId>();
        }

        public MongoCollection<T> GetCollection<T, K>() where T : MongoEntity<K>
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

        public T FindOne<T>(ObjectId id) where T : MongoEntity
        {
            return FindOne<T, ObjectId>(id);
        }

        public T FindOne<T, K>(K id) where T : MongoEntity<K>
        {
            var collection = GetCollection<T, K>();
            var query = Query<T>.EQ(x => x.Id, id);
            return collection.FindOne(query);
        }

        public void Save<T>(T entity) where T : MongoEntity
        {
            Save<T, ObjectId>(entity);
        }

        public void Save<T, K>(T entity) where T : MongoEntity<K>
        {
            var collection = GetCollection<T, K>();
            collection.Save(entity);
        }

        public void Remove<T>(T entity) where T : MongoEntity
        {
            Remove<T, ObjectId>(entity);
        }

        public void Remove<T, K>(T entity) where T : MongoEntity<K>
        {
            var collection = GetCollection<T, K>();
            var query = Query<T>.EQ(x => x.Id, entity.Id);
            collection.Remove(query);
        }

        public string BuildCollectionName(Type type)
        {
            var name = type.Name.ToLower();
            return Pluralization.Pluralize(name);
        }
    }
}