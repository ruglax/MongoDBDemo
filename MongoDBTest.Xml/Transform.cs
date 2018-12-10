using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDBDemo.Document
{
    public static class Transform
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawXml"></param>
        /// <returns></returns>
        public static JObject TransformToJson(string rawXml)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(rawXml);

            //Convert XML Documento to JSON
            string json = JsonConvert.SerializeXmlNode(xdoc);

            //Add new propperty to json
            return JObject.Parse(json);
        }
    }
}
