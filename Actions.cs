using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoSimple
{
    public class Actions
    {
        protected static IMongoDatabase database;
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

        /// <summary>
        /// Fetches one document from the given collection matching the given key.
        /// </summary>
        /// <typeparam name="T">Model type to return.</typeparam>
        /// <param name="collectionName">MongoDB collection name.</param>
        /// <param name="key">Field by which to identify document.</param>
        /// <param name="value">Value by which to identify document.</param>
        /// <returns></returns>
        public T FetchOne<T>(string collectionName, string key, string value) {
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq(key, value);
            var result = collection.Find(filter).First();
            return BsonSerializer.Deserialize<T>(result);
        }
    }
}
