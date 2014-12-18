using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Monghost
{
    public class MongoEntity
    {
        public virtual string GetIdName()
        {
            return "_id";
        }
    }
    
    public abstract class MongoEntityWithObjectId : MongoEntity
    {
        public abstract ObjectId Id { get; set; }
    }
}
