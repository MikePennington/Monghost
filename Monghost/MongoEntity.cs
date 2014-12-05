using System;
using System.Collections.Generic;
using Monghost;
using MongoDB.Bson;

namespace MonGhost
{
    public abstract class MongoEntity
    {
        private static readonly Dictionary<Type, string> CollectionNameMap = new Dictionary<Type, string>();

        public abstract ObjectId Id { get; set; }

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
}
