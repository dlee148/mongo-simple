using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

// http://twistedoakstudios.com/blog/Post1295_publish-your-net-library-as-a-nuget-package

namespace Mongo_Simple
{
	public class MongoSimple
    {
        protected static IMongoDatabase database;
        protected static IMongoClient client;

		// C'tors

		public MongoSimple(string databaseName)
		{
			client = new MongoClient();
			database = client.GetDatabase(databaseName);
		}

        public MongoSimple(string connectionString, string databaseName)
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
        }

        // Read

        public T FetchOne<T>(string collectionName, string key, string value) {
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq(key, value);
            var result = collection.Find(filter).First();
            return BsonSerializer.Deserialize<T>(result);
        }

        public T[] FetchAll<T>(string collectionName) where T : new()
        {
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var resultList = collection.Find(new BsonDocument()).ToList();

            T[] deserializedResult = new T[resultList.Count];
            for (int i = 0; i < resultList.Count; i++)
            {
                deserializedResult[i] = new T();
                deserializedResult[i] = BsonSerializer.Deserialize<T>(resultList[i]);
            }

            return deserializedResult; 
        }

		// Insert

		public bool InsertOne<T>(string collectionName, T toInsert)
		{
			var collection = database.GetCollection<BsonDocument>(collectionName);
			collection.InsertOne(toInsert.ToBsonDocument());
			return true;
		}

		public bool InsertMany<T>(string collectionName, T[] toInsert)
		{
			var collection = database.GetCollection<BsonDocument>(collectionName);

			foreach (T item in toInsert)
			{
				collection.InsertOne(item.ToBsonDocument());
			}

			return true;
		}

		// Remove

		public bool RemoveOne<ValueType>(string collectionName, string key, ValueType value)
		{
			return RemoveHelper(collectionName, Builders<BsonDocument>.Filter.Eq(key, value), false);
		}

		public bool RemoveMany<ValueType>(string collectionName, string key, ValueType value)
		{
			return RemoveHelper(collectionName, Builders<BsonDocument>.Filter.Eq(key, value), true);
		}

		public bool RemoveAll(string collectionName)
		{
			return RemoveHelper(collectionName, new BsonDocument(), true);
		}

		private bool RemoveHelper(string collectionName, FilterDefinition<BsonDocument> filter, bool many)
		{
			var collection = database.GetCollection<BsonDocument>(collectionName);

			if (many)
			{
				collection.DeleteMany(filter);
			}
			else
			{
				collection.DeleteOne(filter);
			}

			return true;
		}

		// Edit

		public bool Unset<ValueType>(string collectionName, string key, ValueType value, string field)
		{
			var collection = database.GetCollection<BsonDocument>(collectionName);
			var filter = Builders<BsonDocument>.Filter.Eq(key, value);
			var update = Builders<BsonDocument>.Update.Unset(field);
			collection.UpdateOne(filter, update);
			return true;
		}

		public bool Set<ValueType, ValueType2>(string collectionName, string key, ValueType value, string field, ValueType2 toSet)
		{
			var collection = database.GetCollection<BsonDocument>(collectionName);
			var filter = Builders<BsonDocument>.Filter.Eq(key, value);
			var update = Builders<BsonDocument>.Update.Set(field, toSet);
			collection.UpdateOne(filter, update);
			return true;
		}
    }
}