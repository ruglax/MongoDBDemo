using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDBDemo.DataAccess;
using MongoDBDemo.Document;
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

        private readonly ILoaderInfo _loaderInfo;

        public CfdiController(IHostingEnvironment env, ILoaderInfo loaderInfo)
        {
            _env = env;
            _loaderInfo = loaderInfo;
        }

        /// <summary>
        /// Realiza la afectación para aceptar rechazar las peticiones pendientes.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet("{uuid}")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery]string uuid)
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

            Repository repository = new Repository();
            using (StreamReader sourceReader = System.IO.File.OpenText(pathToFile))
            {
                string rawXml = await sourceReader.ReadToEndAsync();
                if (!string.IsNullOrWhiteSpace(rawXml))
                {
                    var cfdiJson = Transform.TransformToJson(rawXml);
                    BsonDocument document = _loaderInfo.LoadInfo(cfdiJson);
                    await repository.InsertAsync(document);
                    return Ok(cfdiJson);
                }

            }

            return NotFound("No se encontró el archivo especificado");
        }
    }
}
