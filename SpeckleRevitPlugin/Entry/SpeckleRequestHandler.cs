#region Namespaces
using System;
using System.Collections.Generic;
using System.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SpeckleRevitPlugin.Utilities;
#endregion

namespace SpeckleRevitPlugin.Entry
{
    public class SpeckleRequestHandler : IExternalEventHandler
    {
        public SpeckleRequest Request { get; set; } = new SpeckleRequest();
        public object Arg1 { get; set; }
        public object Arg2 { get; set; }

        public delegate void ClientsRetrieved(IDictionary<string, string> receivers, IDictionary<string, string> senders);
        public static event ClientsRetrieved OnClientsRetrieved;

        public void Execute(UIApplication uiapp)
        {
            try
            {
                switch (Request.Take())
                {
                    case SpeckleCommandType.None:
                        return;
                    case SpeckleCommandType.GetClients:
                        GetClients(uiapp);
                        break;
                    case SpeckleCommandType.SaveClients:
                        SaveClients(uiapp);
                        break;
                    case SpeckleCommandType.Command3:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        private static void GetClients(UIApplication app)
        {
            if (!SchemaUtilities.SchemaExist(Properties.Resources.SchemaName)) return;

            var doc = app.ActiveUIDocument.Document;
            var schema = SchemaUtilities.GetSchema(Properties.Resources.SchemaName);
            var pInfo = SchemaUtilities.GetProjectInfo(doc);
            var receivers = pInfo.GetEntity(schema).Get<IDictionary<string, string>>(schema.GetField("receivers"));
            var senders = pInfo.GetEntity(schema).Get<IDictionary<string, string>>(schema.GetField("senders"));

            OnClientsRetrieved?.Invoke(receivers, senders);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        private void SaveClients(UIApplication app)
        {
            // (Konrad) Extract the variables used by this method
            var senders = Arg1 as Dictionary<string, string>;
            var receivers = Arg2 as Dictionary<string, string>;

            var doc = app.ActiveUIDocument.Document;
            var pInfo = SchemaUtilities.GetProjectInfo(doc);
            var schemaExists = SchemaUtilities.SchemaExist(Properties.Resources.SchemaName);
            var schema = schemaExists
                ? SchemaUtilities.GetSchema(Properties.Resources.SchemaName)
                : SchemaUtilities.CreateSchema();

            using (var trans = new Transaction(doc, "Store Clients"))
            {
                trans.Start();

                if (schemaExists)
                {
                    SchemaUtilities.UpdateSchemaEntity(schema, pInfo, "senders", senders);
                    SchemaUtilities.UpdateSchemaEntity(schema, pInfo, "receivers", receivers);
                }
                else
                {
                    SchemaUtilities.AddSchemaEntity(schema, pInfo, "senders", senders);
                    SchemaUtilities.AddSchemaEntity(schema, pInfo, "receivers", receivers);
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// External event name
        /// </summary>
        /// <returns></returns>
        public string GetName() { return "SpeckleRequestHandler"; }
    }

    public class SpeckleRequest
    {
        private int _request = (int)SpeckleCommandType.None;

        public SpeckleCommandType Take()
        {
            return (SpeckleCommandType)Interlocked.Exchange(ref _request, (int)SpeckleCommandType.None);
        }

        public void Make(SpeckleCommandType request)
        {
            Interlocked.Exchange(ref _request, (int)request);
        }
    }

    public enum SpeckleCommandType
    {
        None,
        GetClients,
        SaveClients,
        Command3
    }
}
