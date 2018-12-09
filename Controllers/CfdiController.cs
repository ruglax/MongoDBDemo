using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDBDemo.Controllers
{
    /// <summary>
    /// Controlados para la aceptación o rechazo de peticiones pendientes.
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CfdiController : Controller
    {
        private readonly IHostingEnvironment _env;

        public CfdiController(IHostingEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// Realiza la afectación para aceptar rechazar las peticiones pendientes.
        /// </summary>
        /// <param name="solicitudRespuesta"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(string uuid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var directorySeparator = Path.DirectorySeparatorChar.ToString();
            var pathToFile = _env.WebRootPath
                             + directorySeparator
                             + "resources"
                             + directorySeparator
                             + "CFDI.xml";

            List<BsonDocument> documents = new List<BsonDocument>();
            for (var i = 0; i < 10000; i++)
            {
                using (StreamReader sourceReader = System.IO.File.OpenText(pathToFile))
                {
                    string rawXml = await sourceReader.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(rawXml))
                    {
                        // Create XML Document
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(rawXml);

                        //Convert XML Documento to JSON
                        string json = JsonConvert.SerializeXmlNode(xdoc);

                        //Add new propperty to json
                        JObject cfdiJson = FillXmlInfoAsync(json);

                        //Deserialize JSON String to BSon Document
                        //var bsdocument = cfdiJson.ToBsonDocument();
                        var bsdocument = BsonSerializer.Deserialize<BsonDocument>(cfdiJson.ToString());
                        documents.Add(bsdocument);
                        //await InsertAsync(bsdocument);
                        //return Ok(cfdiJson);
                    }
                }
            }

            await InsertAsync(documents);

            return Ok();
            //return NotFound("No se encontró el archivo especificado");
        }

        private async Task InsertAsync(List<BsonDocument> documents)
        {
            //Connection string mongodb database
            const string connectionString = "mongodb://localhost:27017";

            // Create a MongoClient object by using the connection string
            var client = new MongoClient(connectionString);

            //Use the MongoClient to access the server
            var database = client.GetDatabase("DBTest");

            var collection = database.GetCollection<BsonDocument>("CFDI33");

            //await collection.InsertOneAsync(bsdocument); //Insert into mongoDB

            await collection.InsertManyAsync(documents);
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
            if (cfdiRelacionados != null)
            {
                cfdiRelacionados.Property("@TipoRelacion")
                    .AddAfterSelf(new JProperty("@TipoRelacionDescripcion", "Nota de crédito de los documentos relacionados"));
            }

            var emisor = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Emisor"];
            if (emisor != null)
            {
                emisor.Property("@RegimenFiscal")
                    .AddAfterSelf(new JProperty("@RegimenFiscalDescripcion", "Demás ingresos"));
            }

            var receptor = (JObject)cfdiJson["cfdi:Comprobante"]["cfdi:Receptor"];
            if (receptor != null)
            {
                receptor.Property("@UsoCFDI")
                    .AddAfterSelf(new JProperty("@UsoCFDIDescripcion", "Adquisición de mercancias"));
            }

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

            return cfdiJson;
        }
    }
}
