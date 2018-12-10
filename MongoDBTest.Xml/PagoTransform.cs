using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;

namespace MongoDBDemo.Document
{
    public class PagoLoader : ILoaderInfo
    {
        private readonly ILoaderInfo _cfdiLoader;

        public PagoLoader(ILoaderInfo cfdiLoader)
        {
            _cfdiLoader = cfdiLoader;
        }


        public BsonDocument LoadInfo(JObject cfdiObject)
        {
            _cfdiLoader.LoadInfo(cfdiObject);

            return BsonSerializer.Deserialize<BsonDocument>(cfdiObject.ToString());
        }
    }
}
