using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDBDemo.Document.Catalog
{
    public class CatalogQuery
    {
        public string CatalogName { get; set; }

        public string Filter { get; set; }

        public string Version { get; set; } = "1.0";

        public QueryType QueryType { get; set; }
    }

    public enum QueryType
    {
        None, 

        ClaveStartWith,

        ClaveContains,

        DescriptionStartWith,

        DescriptionContains,

        BothContains
    }
}
