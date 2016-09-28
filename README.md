# MongoSimple v0.0.1

MongoSimple is an additional layer of abstraction on top of [the official MongoDB C# Driver](https://github.com/mongodb/mongo-csharp-driver).
It was created with the target audience of users looking to ease into the NoSQL domain,
without the complexities of asynchronicity. It performs simple queries with minimal fuss.

### Example Usage

```csharp
var x = new MongoSimple();
var result = x.fetchOne<Type>("exampleCollection");
```

#### Future Functionality

- Resolve queries with multiple positional operators
- Add more robust functionality for less common queries (increments, bitwise, etc.)
