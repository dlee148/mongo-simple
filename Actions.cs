using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

// http://twistedoakstudios.com/blog/Post1295_publish-your-net-library-as-a-nuget-package

namespace MongoSimple
{
    /// <summary>
    /// Constitutes common CRUD actions.
    /// </summary>
    public class Actions
    {
        /// <summary>Database instance - object specific.</summary>
        protected static IMongoDatabase database;
        /// <summary>Client instance - object specific.</summary>
        protected static IMongoClient client;

        /// <summary>
        /// Establishes a connection to a Mongo database.
        /// </summary>
        /// <param name="connectionString">Default port for MongoDB is 27017.</param>
        /// <param name="databaseName">Name of Mongo database.</param>
        public Actions(string connectionString, string databaseName)
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
        }

        // Read

        /// <summary>
        /// Fetches one document from the given collection matching the given key.
        /// </summary>
        /// <typeparam name="T">Model type to return.</typeparam>
        /// <param name="collectionName">MongoDB collection name.</param>
        /// <param name="key">Field by which to identify document.</param>
        /// <param name="value">Value by which to identify document.</param>
        /// <returns>One object of type T.</returns>
        public T FetchOne<T>(string collectionName, string key, string value) {
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq(key, value);
            var result = collection.Find(filter).First();
            return BsonSerializer.Deserialize<T>(result);
        }

        /// <summary>
        /// Fetches all documents in an array.
        /// </summary>
        /// <typeparam name="T">Data model for one document in the collection.</typeparam>
        /// <param name="collectionName">MongoDB collection name.</param>
        /// <returns>An array of objects of type T.</returns>
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

        /// <summary>
        /// Determines if the query string has >1 positional operators.
        /// </summary>
        /// <param name="query">Query string in question.<param>
        /// <returns>Whether or not string has multiple pos. ops.</returns>
        public bool HasMultiplePositionalOperators(string query)
        {
            return query.Count(x => x == '$') > 1;
        }

        public string ResolvePositionalOperators<T>(string collectionName, string query, Dictionary<string, string> ids)
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