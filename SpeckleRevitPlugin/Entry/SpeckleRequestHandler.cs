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
        public delegate void ClientsRetrieved(IDictionary<string, string> recivers, IDictionary<string, string> senders);
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
                    case SpeckleCommandType.Command2:
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
        private void GetClients(UIApplication app)
        {
            if (!SchemaUtilities.SchemaExist(Properties.Resources.SchemaName)) return;

            var doc = app.ActiveUIDocument.Document;
            var schema = SchemaUtilities.GetSchema(Properties.Resources.SchemaName);
            var pInfo = SchemaUtilities.GetProjectInfo(doc);
            var recivers = pInfo.GetEntity(schema).Get<IDictionary<string, string>>(schema.GetField("receivers"));
            var senders = pInfo.GetEntity(schema).Get<IDictionary<string, string>>(schema.GetField("senders"));

            OnClientsRetrieved?.Invoke(recivers, senders);
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
        Command2,
        Command3
    }
}
