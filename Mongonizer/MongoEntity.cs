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
}
