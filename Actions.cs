using System.Collections.Generic;
using System.Linq;
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

        public void Edit(string collectionName, string query) {
            // example
            if (HasMultiplePositionalOperators(query)) {
                query = ResolvePositionalOperators(collectionName, query, new Dictionary<string, string>());
            }
        }

        // Utility

        public bool HasMultiplePositionalOperators(string query)
        {
            return query.Count(x => x == '$') > 1;
        }

        public string ResolvePositionalOperators(string collectionName, string query, Dictionary<string, string> ids)
        {
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq(ids.First().Key, ids.First().Value);
            ids.Remove(ids.First().Key);

            // There's going to be a lot of sketchy stuff that goes down here
            // and it would be prudent to take some time to look at it
            // http://stackoverflow.com/questions/2905187/accessing-object-property-as-string-and-setting-its-value

            return "Help me.";
        }
    }
}