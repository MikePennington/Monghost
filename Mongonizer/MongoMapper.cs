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
        T FindOne<T>(IMongoQuery query) where T : MongoEntity;
        void Save<T>(T entity) where T : MongoEntity;
        void Remove<T>(T entity) where T : MongoEntityWithObjectId;
        void Remove<T>(BsonValue id) where T : MongoEntity;
    }

    public class MongoMapper : IMongoMapper
    {
        private static readonly Dictionary<Type, string> CollectionNameMap = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, string> IdNameMap = new Dictionary<Type, string>();

        private readonly MongoDatabase _database;

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
            var collectionName = GetCollectionName<T>();
            var collection = Database.GetCollection<T>(collectionName);
            return collection;
        }

        public T FindOne<T>(BsonValue id) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            var query = Query.EQ(GetIdName<T>(), id);
            return collection.FindOne(query);
        }

        public T FindOne<T>(IMongoQuery query) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            return collection.FindOne(query);
        }

        public void Save<T>(T entity) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            collection.Save(entity);
        }

        public void Remove<T>(T entity) where T : MongoEntityWithObjectId
        {
            var collection = GetCollection<T>();
            var query = Query.EQ(GetIdName<T>(), entity.Id);
            collection.Remove(query);
        }

        public void Remove<T>(BsonValue id) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            var query = Query.EQ(GetIdName<T>(), id);
            collection.Remove(query);
        }

        private string GetCollectionName<T>() where T : MongoEntity
        {
            Type type = typeof(T);
            string collectionName;
            if (CollectionNameMap.ContainsKey(type))
            {
                collectionName = CollectionNameMap[type];
            }
            else
            {
                collectionName = Pluralization.Pluralize(type.Name.ToLower());
                CollectionNameMap.Add(type, collectionName);
            }
            return collectionName;
        }
        
        private string GetIdName<T>() where T : MongoEntity
        {
            Type type = typeof (T);
            string idName;
            if (IdNameMap.ContainsKey(type))
            {
                idName = IdNameMap[type];
            }
            else
            {
                idName = Activator.CreateInstance<T>().GetIdName();
                IdNameMap.Add(type, idName);
            }
            return idName;
        }
    }
}