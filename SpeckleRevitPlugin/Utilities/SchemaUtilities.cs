#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
#endregion

namespace SpeckleRevitPlugin.Utilities
{
    public static class SchemaUtilities
    {
        /// <summary>
        /// Retrieves Schema by its name.
        /// </summary>
        /// <param name="schemaName">Schema name.</param>
        /// <returns></returns>
        public static Schema GetSchema(string schemaName)
        {
            var schemas = Schema.ListSchemas();
            if (schemas == null || !schemas.Any()) return null;
            return schemas.FirstOrDefault(s => s.SchemaName == schemaName);
        }

        /// <summary>
        /// Checks if Schema exists in Document.
        /// </summary>
        /// <param name="schemaName">Schema name.</param>
        /// <returns></returns>
        public static bool SchemaExist(string schemaName)
        {
            return GetSchema(schemaName) != null;
        }

        /// <summary>
        /// Creates Speckle Schema.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Adds new Speckle Schema Entity to Element.
        /// </summary>
        /// <param name="s">Schema</param>
        /// <param name="e">Element</param>
        /// <param name="fName">Filed name</param>
        /// <param name="fValue">Field value</param>
        public static void AddSchemaEntity(Schema s, Element e, string fName, Dictionary<string, string> fValue)
        {
            if (s == null)
            {
                throw new NullReferenceException("schema");
            }
            if (e == null)
            {
                throw new NullReferenceException("element");
            }
            if (string.IsNullOrEmpty(fName))
            {
                throw new NullReferenceException("fieldName");
            }

            try
            {
                var entity = new Entity(s);
                var settingsField = s.GetField(fName);
                entity.Set<IDictionary<string, string>>(settingsField, fValue);

                e.SetEntity(entity);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }

        /// <summary>
        /// Speckle Schema will always be stored on this Element,
        /// so now we have a utility to get to it faster.
        /// </summary>
        /// <param name="doc">Revit Document</param>
        /// <returns></returns>
        public static Element GetProjectInfo(Document doc)
        {
            var pInfo = new FilteredElementCollector(doc)
                .OfClass(typeof(ProjectInfo))
                .FirstElement();
            return pInfo;
        }

        /// <summary>
        /// Updates existing Speckle Schema Entity on given Element.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="fName"></param>
        /// <param name="fValue"></param>
        public static void UpdateSchemaEntity(Schema s, Element e, string fName, Dictionary<string, string> fValue)
        {
            if (s == null)
            {
                throw new NullReferenceException("schema");
            }
            if (e == null)
            {
                throw new NullReferenceException("element");
            }
            if (string.IsNullOrEmpty(fName))
            {
                throw new NullReferenceException("fieldName");
            }

            try
            {
                var entity = e.GetEntity(s);
                var field = s.GetField(fName);
                entity.Set<IDictionary<string, string>>(field, fValue);

                e.SetEntity(entity);
            }
            catch(Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }
    }
}
