using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDBDemo.Document.Catalog;
using Newtonsoft.Json.Linq;
using MongoDBDemo.Document.Extensions;

namespace MongoDBDemo.Document
{
    public class CfdiLoader : ILoaderInfo
    {
        private readonly ICatalogClient _catalogClient;

        public CfdiLoader(ICatalogClient catalogClient)
        {
            _catalogClient = catalogClient;
        }

        public BsonDocument LoadInfo(JObject cfdiObject)
        {
            LoadComprobanteInfo(cfdiObject);
            LoadEmisorInfo(cfdiObject);
            LoadReceptorInfo(cfdiObject);
            LoadcfdiRelacionadoInfo(cfdiObject);
            LoadConceptosInfo(cfdiObject);
            return BsonSerializer.Deserialize<BsonDocument>(cfdiObject.ToString());
        }

        private void LoadConceptosInfo(JObject cfdiJson)
        {
            var conceptos = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Conceptos"];
            if (conceptos != null)
            {
                JArray conceptosArray = (JArray)conceptos["cfdi:Concepto"];
                foreach (var concepto in conceptosArray)
                {
                    var conceptoObj = (JObject)concepto;
                    conceptoObj.Property("@ClaveProdServ")
                        .AddAfterSelf(_catalogClient, CatalogType.ClaveProdServ, "@ClaveProdServDescripcion");
                    conceptoObj.Property("@ClaveUnidad")
                        .AddAfterSelf(_catalogClient, CatalogType.ClaveUnidad, "@ClaveUnidadDescripcion");

                    var conceptoImpuesto = conceptoObj["cfdi:Impuestos"];
                    var conceptoImpuestoTraslado = conceptoImpuesto["cfdi:Traslados"];
                    if (conceptoImpuestoTraslado != null)
                    {
                        //JArray trasladoArray = (JArray)conceptoImpuestoTraslado["cfdi:Traslado"];
                    }
                }
            }
        }

        private void LoadReceptorInfo(JObject cfdiJson)
        {
            var receptor = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Receptor"];
            receptor?.Property("@UsoCFDI")
                .AddAfterSelf(_catalogClient, CatalogType.UsoCFDI, "@UsoCFDIDescripcion");
        }

        private void LoadEmisorInfo(JObject cfdiJson)
        {
            var emisor = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Emisor"];
            emisor?.Property("@RegimenFiscal")
                .AddAfterSelf(_catalogClient, CatalogType.RegimenFiscal, "@RegimenFiscalDescripcion");
        }

        private void LoadcfdiRelacionadoInfo(JObject cfdiJson)
        {
            var cfdiRelacionados = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:CfdiRelacionados"];
            cfdiRelacionados?.Property("@TipoRelacion")
                .AddAfterSelf(_catalogClient, CatalogType.TipoRelacion, "@TipoRelacionDescripcion");
        }

        private void LoadComprobanteInfo(JObject cfdiJson)
        {
            var comprobante = (JObject)cfdiJson["cfdi:Comprobante"];
            comprobante.Property("@Moneda")
                .AddAfterSelf(_catalogClient, CatalogType.Moneda, "@MonedaDescripcion");
            comprobante.Property("@TipoDeComprobante")
                .AddAfterSelf(_catalogClient, CatalogType.TipoDeComprobante, "@TipoDeComprobanteDescripcion");
            comprobante.Property("@FormaPago")
                .AddAfterSelf(_catalogClient, CatalogType.FormaPago, "@FormaPagoDescripcion");
            comprobante.Property("@MetodoPago")
                .AddAfterSelf(_catalogClient, CatalogType.MetodoPago, "@MetodoPagoDescripcion");
        }
    }
}
