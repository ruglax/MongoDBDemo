using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBDemo.DataAccess
{
    public class Repository
    {
        //Connection string mongodb database
        const string ConnectionString = "mongodb://localhost:27017";

        private readonly IMongoDatabase _database;

        public Repository()
        {
            // Create a MongoClient object by using the connection string
            var client = new MongoClient(ConnectionString);

            //Use the MongoClient to access the server
            _database = client.GetDatabase("DBTest");
        }

        public async Task InsertAsync(List<BsonDocument> documents)
        {
            var collection = _database.GetCollection<BsonDocument>("CFDI33");
            await collection.InsertManyAsync(documents);
        }

        public async Task InsertAsync(BsonDocument bsdocument)
        {
            var collection = _database.GetCollection<BsonDocument>("CFDI33");
            await collection.InsertOneAsync(bsdocument); //Insert into mongoDB
        }
    }
}
   
