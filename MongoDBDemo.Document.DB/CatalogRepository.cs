using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using MongoDBDemo.Document.Catalog;

namespace MongoDBDemo.Document.DB
{
    public class CatalogRepository : ICatalogClient
    {
        #region Attibutes

        protected string connectionString;

        #endregion

        #region Constructs

        public CatalogRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            this.connectionString = connectionString;
        }

        #endregion

        #region Public Methods

        public CatalogEntry GetValue(CatalogQuery query)
        {
            return GetValue(query.CatalogName, query.Filter);
        }

        public CatalogEntry GetValue(string catalog, string clave)
        {
            int len = clave?.Length ?? 0;
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT TOP 1 CLAVE, DESCRIPCION ");
            builder.Append($"FROM {catalog} ");
            builder.Append("WHERE VERSION = @VERSION AND CLAVE = @CLAVE ");
            builder.Append($"AND LEN(CLAVE) = {len} ");
            builder.Append("AND ((FECHA_FIN_VIGENCIA IS NULL AND @TODAY >= FECHA_INICIO_VIGENCIA) OR ");
            builder.Append("(FECHA_FIN_VIGENCIA IS NOT NULL AND @TODAY BETWEEN FECHA_INICIO_VIGENCIA AND FECHA_FIN_VIGENCIA))");
            using (var conn = new SqlConnection(connectionString))
            {

                conn.Open();
                return conn.Query<CatalogEntry>(builder.ToString(), new { @CLAVE = clave, @TODAY = DateTime.Now, @VERSION = "1.0" }).FirstOrDefault();
            }
        }

        public CatalogEntry GetValue(CatalogType catalogType, string clave)
        {
            string table = GetTable(catalogType);
            return GetValue(table, clave);
        }

        #endregion

        private string GetTable(CatalogType catalogType)
        {
            switch (catalogType)
            {
                case CatalogType.Aduana:
                    return "TBLADUANA";
                case CatalogType.ClaveUnidad:
                    return "TBLCLAVEUNIDAD";
                case CatalogType.ClaveProdServ:
                    return "TBLPRODUCTOSSERVICIOS";
                case CatalogType.CodigoPostal:
                    return "TBLCODIGOPOSTAL";
                case CatalogType.FormaPago:
                    return "TBLFORMAPAGO";
                case CatalogType.Impuesto:
                    return "TBLIMPUESTO";
                case CatalogType.MetodoPago:
                    return "TBLMETODODEPAGO33";
                case CatalogType.Moneda:
                    return "TBLMONEDAS";
                case CatalogType.NumPedimentoAduana:
                    return "TBLNUMPEDIMENTOADUANA";
                case CatalogType.Pais:
                    return "TBLPAISES";
                case CatalogType.PatenteAduanal:
                    return "TBLPATENTEADUANAL";
                case CatalogType.RegimenFiscal:
                    return "TBLREGIMENFISCAL";
                case CatalogType.TasaOCuota:
                    return "TBLTASAOCUOTA";
                case CatalogType.TipoDeComprobante:
                    return "TBLTIPOCOMPROBANTE";
                case CatalogType.TipoFactor:
                    return "TBLTIPOFACTOR";
                case CatalogType.TipoRelacion:
                    return "TBLTIPORELACION";
                case CatalogType.UsoCFDI:
                    return "TBLUSOCFDI";
                default:
                    throw new ArgumentException($"El catálogo {catalogType} especificado no está definido.");
            }
        }
    }
}
