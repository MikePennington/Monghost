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
    
    /// <summary>
    /// Use if you want to be able to call Remove(entity)
    /// </summary>
    public abstract class MongoEntityWithObjectId : MongoEntity
    {
        public abstract ObjectId Id { get; set; }
    }
}
