# Mongonizer
A lightweight wrapper around the .NET MongoDB ORM to automate object to collection name mapping

## How-To

All of your MongoDB models should inherit from MongoEntity.

```
    public class EntityOne : MongoEntity
    {
        public ObjectId Id { get; set; }
        public string String { get; set; }
        public bool Bool { get; set; }
        public List<int> List { get; set; }
    }

    public class EntityTwo : MongoEntity
    {
        public Guid Id { get; set; }
        public string String { get; set; }
        public bool Bool { get; set; }
        public List<int> List { get; set; }
    }

    public class EntityThree : MongoEntity
    {
        [BsonId]
        public string Name { get; set; }
        public int Age { get; set; }
    }
```

Then you get all do this:
```
var mongoMapper = new MongoMapper("mongodb://127.0.0.1:27027", "testdb");
_mongoMapper.FindOne<EntityOne>(entity);
_mongoMapper.Save(entity);
_mongoMapper.Remove<EntityOne>(entity.Id);
```
