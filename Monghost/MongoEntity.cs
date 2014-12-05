using System;
using System.Collections.Generic;
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

            // For some words we need to look at more than just the last letter
            if (name.EndsWith("tch"))
            {
                name = name + "es";
                AddToMap(name);
                return name;
            }

            // Look at the last letter to determine how to pluralize
            var lastLetter = name.Substring(name.Length - 1);
            switch (lastLetter)
            {
                case "y":
                    int index = name.LastIndexOf("y");
                    name = name.Remove(index, 1).Insert(index, "ies");
                    return name;
                case "s":
                case "o":
                case "x":
                    name = name + "es";
                    return name;
                default:
                    name = name + "s";
                    return name;
            }
        }
    }
}
