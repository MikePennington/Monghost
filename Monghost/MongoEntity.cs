using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Monghost
{
    public abstract class MongoEntity<T>
    {
        public abstract T Id { get; set; }
    }

    public abstract class MongoEntity : MongoEntity<ObjectId>
    {
        public abstract override ObjectId Id { get; set; }
    }
}
