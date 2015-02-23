using MongoDB.Bson;

namespace Mongonizer
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
