# Mongonizer
A lightweight wrapper around the .NET MongoDB ORM to automate object to collection name mapping.

## How-To

Mongonizer allows you to easily find, save, and remove entities from MongoDB without having to maintain mappings between classes and collection names.
```
IMongoMapper mongoMapper = new MongoMapper("mongodb://127.0.0.1:27027", "testdb");
var cat = _mongoMapper.FindOne<Cat>(Query<Cat>.EQ(x => x.Name, "Fluffy");
_mongoMapper.Save(cat);
_mongoMapper.Remove<Cat>(cat);
```

Mongonizer users the .NET PluralizationService to ensure the collection names match the MongoDB standard of plural, lowercase collection names.

Mongonizer also has access to the Database object and the Collection object, so you can interact with the official .NET MongoDB driver objects if you want to. This allows you to inject just the IMongoMapper interface, and have access to everything you need.

```
MongoDatabase database = mongoMapper.Database;
MongoCollection<User> collection = mongoMapper.GetCollection<User>;
```

You can extend from one of two base classes. Use MongoEntityWithObjectId when your entity has an ObjectId property named Id. Use MongoEntity for any entity that does not have an ObjectId.

Examples entities:
```
public class Cat : MongoEntityWithObjectId
{
    public override ObjectId Id { get; set; }
    public string Name { get; set; }
}

public class Dog : MongoEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class Owner : MongoEntity
{
    [BsonId]
    public string Email { get; set; }
    public string Name { get; set; }
}
```
