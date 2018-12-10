using System;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDBDemo.Document
{
    public class CfdiLoader : ILoaderInfo
    {
        public BsonDocument LoadInfo(JObject cfdiObject)
        {
            LoadComprobanteInfo(cfdiObject);
            LoadEmisorInfo(cfdiObject);
            LoadReceptorInfo(cfdiObject);
            LoadcfdiRelacionadoInfo(cfdiObject);
            LoadConceptosInfo(cfdiObject);
            return  BsonSerializer.Deserialize<BsonDocument>(cfdiObject.ToString());
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
                        .AddAfterSelf(new JProperty("@ClaveProdServDescripcion", "No existe en el catálogo"));
                    conceptoObj.Property("@ClaveUnidad")
                        .AddAfterSelf(new JProperty("@ClaveUnidadDescripcion", "'Metro por kelvin"));

                    var conceptoImpuesto = conceptoObj["cfdi:Impuestos"];
                    var conceptoImpuestoTraslado = conceptoImpuesto["cfdi:Traslados"];
                    if (conceptoImpuestoTraslado != null)
                    {

                    }
                }
            }
        }

        private void LoadReceptorInfo(JObject cfdiJson)
        {
            var receptor = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Receptor"];
            receptor?.Property("@UsoCFDI")
                .AddAfterSelf(new JProperty("@UsoCFDIDescripcion", "Adquisición de mercancias"));
        }

        private void LoadEmisorInfo(JObject cfdiJson)
        {
            var emisor = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Emisor"];
            emisor?.Property("@RegimenFiscal")
                .AddAfterSelf(new JProperty("@RegimenFiscalDescripcion", "Demás ingresos"));
        }

        private void LoadcfdiRelacionadoInfo(JObject cfdiJson)
        {
            var cfdiRelacionados = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:CfdiRelacionados"];
            cfdiRelacionados?.Property("@TipoRelacion")
                .AddAfterSelf(new JProperty("@TipoRelacionDescripcion", "Nota de crédito de los documentos relacionados"));
        }

        private void LoadComprobanteInfo(JObject cfdiJson)
        {
            var comprobante = (JObject)cfdiJson["cfdi:Comprobante"];
            comprobante.Property("@Moneda")
                .AddAfterSelf(new JProperty("@MonedaDescripcion", "Peso Mexicano"));
            comprobante.Property("@TipoDeComprobante")
                .AddAfterSelf(new JProperty("@TipoDeComprobanteDescripcion", "Ingreso"));
            comprobante.Property("@FormaPago")
                .AddAfterSelf(new JProperty("@FormaPagoDescripcion", "Efectivo"));
            comprobante.Property("@MetodoPago")
                .AddAfterSelf(new JProperty("@MetodoPagoDescripcion", "Pago en una sola exhibición"));
        }
    }
}
