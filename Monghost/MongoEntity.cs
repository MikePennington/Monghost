using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Monghost
{
    public abstract class MongoEntity<T>
    {
        private static readonly Dictionary<Type, string> CollectionNameMap = new Dictionary<Type, string>();

        public abstract T Id { get; set; }

        public string GetCollectionName()
        {
            if (CollectionNameMap.ContainsKey(GetType()))
                return CollectionNameMap[GetType()];

            var name = LowerAndPluralizeType();
            AddToMap(name);
            return name;
        }

        private void AddToMap(string name)
        {
            CollectionNameMap.Add(GetType(), name);
        }

        private string LowerAndPluralizeType()
        {
            var name = GetType().Name.ToLower();
            return Pluralization.Pluralize(name);
        }
    }

    public abstract class MongoEntity : MongoEntity<ObjectId>
    {
        public abstract override ObjectId Id { get; set; }
    }
}
