# Mongonizer
A lightweight wrapper around the .NET MongoDB ORM to automate object to collection name mapping.

## How-To

Mongonizer allows you to easily find, save, and remove entities from MongoDB without having to maintain mappings between classes and collection names.
```
IMongoMapper mongoMapper = new MongoMapper("mongodb://127.0.0.1:27027", "testdb");
var entity = _mongoMapper.FindOne<Cat>("123");
_mongoMapper.Save(entity);
_mongoMapper.Remove<EntityOne>(entity);
```

Mongonizer users the .NET PluralizationService to ensure the collection names match the MongoDB standard of plural, lowercase collection names.

You can extend from one of two base classes. Use MongoEntityWithObjectId when your entity has an ObjectId property named Id. Use MongoEntity for any entity that does not have an ObjectId.

Examples entities:
```
public class Address : MongoEntityWithObjectId
{
    public override ObjectId Id { get; set; }
    public string City { get; set; }
}

public class PhoneNumber : MongoEntity
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; }
}

public class User : MongoEntity
{
    [BsonId]
    public string Email { get; set; }
    public string Name { get; set; }
}
```
