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

		/// <summary>
		/// Connect to a locally running database.
		/// </summary>
		public MongoSimple(string databaseName)
		{
			client = new MongoClient();
			database = client.GetDatabase(databaseName);
		}

		/// <summary>
		/// Connect to a remote database.
		/// </summary>
        public MongoSimple(string connectionString, string databaseName)
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
        }

        // Read

		/// <summary>
		/// Fetches one document of type T, matching a key-value pair of type T2.
		/// </summary>
        public T FetchOne<T, T2>(string collectionName, string key, T2 value) {
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq(key, value);
            var result = collection.Find(filter).First();
            return BsonSerializer.Deserialize<T>(result);
        }

		/// <summary>
		/// Fetches all documents of type T, matching the key-value pair of type T2.
		/// </summary>
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

		/// <summary>
		/// Inserts one document of type T into the given collection.
		/// </summary>
		public void InsertOne<T>(string collectionName, T toInsert)
		{
			var collection = database.GetCollection<BsonDocument>(collectionName);
			collection.InsertOne(toInsert.ToBsonDocument());
		}

		/// <summary>
		/// Inserts many documents of type T into the given collection.
		/// </summary>
		public void InsertMany<T>(string collectionName, T[] toInsert)
		{
			var collection = database.GetCollection<BsonDocument>(collectionName);

			foreach (T item in toInsert)
			{
				collection.InsertOne(item.ToBsonDocument());
			}
		}

		// Remove

		/// <summary>
		/// Removes one document from the given collection matching the given key-value pair.
		/// </summary>
		public bool RemoveOne<ValueType>(string collectionName, string key, ValueType value)
		{
			return RemoveHelper(collectionName, Builders<BsonDocument>.Filter.Eq(key, value), false);
		}

		/// <summary>
		/// Removes many documents from the given collection matching the given key-value pair.
		/// </summary>
		public bool RemoveMany<ValueType>(string collectionName, string key, ValueType value)
		{
			return RemoveHelper(collectionName, Builders<BsonDocument>.Filter.Eq(key, value), true);
		}

		/// <summary>
		/// Removes all documents from the collection.
		/// </summary>
		public bool RemoveAll(string collectionName)
		{
			return RemoveHelper(collectionName, new BsonDocument(), true);
		}

		private bool RemoveHelper(string collectionName, FilterDefinition<BsonDocument> filter, bool many)
		{
			var collection = database.GetCollection<BsonDocument>(collectionName);
			DeleteResult result = many ? collection.DeleteMany(filter) : collection.DeleteOne(filter);
			return result.DeletedCount != 0;
		}

		// Edit

		/// <summary>
		/// Unsets the given field that matches the given key-value pair.
		/// </summary>
		public bool Unset<ValueType>(string collectionName, string key, ValueType value, string field)
		{
			return EditHelper(collectionName, Builders<BsonDocument>.Filter.Eq(key, value), Builders<BsonDocument>.Update.Unset(field));
		}

		/// <summary>
		/// Sets the given field to the provided value in the document matching the given key-value pair.
		/// </summary>
		public bool Set<ValueType, ValueType2>(string collectionName, string key, ValueType value, string field, ValueType2 toSet)
		{
			return EditHelper(collectionName, Builders<BsonDocument>.Filter.Eq(key, value), Builders<BsonDocument>.Update.Set(field, toSet));
		}

		private bool EditHelper(string collectionName, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
		{
			var collection = database.GetCollection<BsonDocument>(collectionName);
			var result = collection.UpdateOne(filter, update);
			return result.ModifiedCount != 0;
		}
    }
}