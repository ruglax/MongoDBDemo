using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDBDemo.Document.Catalog
{
    public interface ICatalogClient
    {
        CatalogEntry GetValue(CatalogQuery query);

        CatalogEntry GetValue(string catalog, string clave);

        CatalogEntry GetValue(CatalogType catalogType, string clave);
    }
}
