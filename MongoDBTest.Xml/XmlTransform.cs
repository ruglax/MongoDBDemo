using System;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDBDemo.Document
{
    public class XmlTransform
    {
        public BsonDocument Transform(string rawXml, out JObject cfdiJson)
        {
            if (string.IsNullOrWhiteSpace(rawXml))
            {
                cfdiJson = new JObject();
                return null;
            }

            // Create XML Document
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(rawXml);

            //Convert XML Documento to JSON
            string json = JsonConvert.SerializeXmlNode(xdoc);

            //Add new propperty to json
            cfdiJson = FillXmlInfoAsync(json);

            //Deserialize JSON String to BSon Document
            return BsonSerializer.Deserialize<BsonDocument>(json);

        }

        private JObject FillXmlInfoAsync(string json)
        {
            JObject cfdiJson = JObject.Parse(json);
            var comprobante = (JObject)cfdiJson["cfdi:Comprobante"];
            comprobante.Property("@Moneda")
                .AddAfterSelf(new JProperty("@MonedaDescripcion", "Peso Mexicano"));
            comprobante.Property("@TipoDeComprobante")
                .AddAfterSelf(new JProperty("@TipoDeComprobanteDescripcion", "Ingreso"));
            comprobante.Property("@FormaPago")
                .AddAfterSelf(new JProperty("@FormaPagoDescripcion", "Efectivo"));
            comprobante.Property("@MetodoPago")
                .AddAfterSelf(new JProperty("@MetodoPagoDescripcion", "Pago en una sola exhibición"));

            var cfdiRelacionados = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:CfdiRelacionados"];
            cfdiRelacionados?.Property("@TipoRelacion")
                .AddAfterSelf(new JProperty("@TipoRelacionDescripcion", "Nota de crédito de los documentos relacionados"));

            var emisor = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Emisor"];
            emisor?.Property("@RegimenFiscal")
                .AddAfterSelf(new JProperty("@RegimenFiscalDescripcion", "Demás ingresos"));

            var receptor = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Receptor"];
            receptor?.Property("@UsoCFDI")
                .AddAfterSelf(new JProperty("@UsoCFDIDescripcion", "Adquisición de mercancias"));

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

            var tfd = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Complemento"]["tfd:TimbreFiscalDigital"];
            tfd.Property("@UUID").Value = Guid.NewGuid().ToString();

            return cfdiJson;
        }
    }
}
