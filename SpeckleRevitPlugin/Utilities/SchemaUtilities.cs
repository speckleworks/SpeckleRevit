using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace SpeckleRevitPlugin.Utilities
{
    public static class SchemaUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static Schema GetSchema(string schemaName)
        {
            var schemas = Schema.ListSchemas();
            if (schemas == null || !schemas.Any()) return null;
            return schemas.FirstOrDefault(s => s.SchemaName == schemaName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static bool SchemaExist(string schemaName)
        {
            return GetSchema(schemaName) != null;
        }

        public static Schema CreateSchema()
        {
            var schemaGuid = new Guid(Properties.Resources.SchemaId);
            var schemaBuilder = new SchemaBuilder(schemaGuid);
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Application);

            // (Konrad) These values are set in addin manifest.
            schemaBuilder.SetApplicationGUID(new Guid("0a03f52e-0ee0-4c19-82c4-8181626ec816"));
            schemaBuilder.SetVendorId("Speckle");

            schemaBuilder.SetSchemaName(Properties.Resources.SchemaName);
            schemaBuilder.SetDocumentation("Speckle schema.");
            schemaBuilder.AddMapField("senders", typeof(string), typeof(string));
            schemaBuilder.AddMapField("receivers", typeof(string), typeof(string));
            var schema = schemaBuilder.Finish();

            return schema;
        }

        public static void AddSchemaEntity(Schema schema, Element e, string fieldName, Dictionary<string, string> fieldValue)
        {
            if (schema == null)
            {
                throw new NullReferenceException("schema");
            }
            if (e == null)
            {
                throw new NullReferenceException("element");
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new NullReferenceException("fieldName");
            }

            try
            {
                var entity = new Entity(schema);
                var settingsField = schema.GetField(fieldName);
                entity.Set<IDictionary<string, string>>(settingsField, fieldValue);

                e.SetEntity(entity);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Element GetProjectInfo(Document doc)
        {
            var pInfo = new FilteredElementCollector(doc)
                .OfClass(typeof(ProjectInfo))
                .FirstElement();
            return pInfo;
        }

        public static void UpdateSchemaEntity(Schema schema, Element e, string fieldName, Dictionary<string, string> fieldValue)
        {
            if (schema == null)
            {
                throw new NullReferenceException("schema");
            }
            if (e == null)
            {
                throw new NullReferenceException("element");
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new NullReferenceException("fieldName");
            }

            try
            {
                var entity = e.GetEntity(schema);
                var field = schema.GetField(fieldName);
                entity.Set<IDictionary<string, string>>(field, fieldValue);

                e.SetEntity(entity);
            }
            catch(Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }
    }
}
