using System;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace MongoDBDemo.Document
{
    public interface ILoaderInfo
    {
        BsonDocument LoadInfo(JObject cfdiObject);
    }
}