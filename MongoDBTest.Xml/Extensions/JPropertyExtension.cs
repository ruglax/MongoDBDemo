using System;
using System.Collections.Generic;
using System.Text;
using MongoDBDemo.Document.Catalog;
using Newtonsoft.Json.Linq;

namespace MongoDBDemo.Document.Extensions
{
    public static class JPropertyExtension
    {
        public static void AddAfterSelf(this JProperty property, ICatalogClient catalogClient, CatalogType catalogType, string propertyName)
        {
            var resultQuery = catalogClient.GetValue(catalogType, property.Value.ToString());
            property.AddAfterSelf(new JProperty(propertyName, resultQuery.DESCRIPCION));
        }

        public static void AddAfterSelf(this JProperty property, CatalogType catalogType, ICatalogClient catalogClient)
        {
            var resultQuery = catalogClient.GetValue(catalogType, property.Value.ToString());
            property.AddAfterSelf(new JProperty($"{property.Name}Descripcion", resultQuery.DESCRIPCION));
        }
    }
}
