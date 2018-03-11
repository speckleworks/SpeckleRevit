using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpeckleCore;
using SpeckleRevit;
using SpeckleRevitPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SpeckleRevit
{
    /// <summary>
    /// Rhino Sender Client
    /// </summary>
    [Serializable]
    public class RevitSender : ISpeckleClient
    {
        public Interop Context { get; set; }

        public SpeckleApiClient Client { get; private set; }

        public List<SpeckleObject> Objects { get; set; }

        public string StreamId { get; set; }

        public bool Paused { get; set; } = false;

        public bool Visible { get; set; } = true;

        System.Timers.Timer DataSender, MetadataSender;

        public string StreamName;

        public bool IsSendingUpdate = false, Expired = false;

        public RevitSender(string _payload, Interop _Context)
        {
            Context = _Context;

            dynamic InitPayload = JsonConvert.DeserializeObject<ExpandoObject>(_payload);

            Client = new SpeckleApiClient((string)InitPayload.account.restApi, true);

            StreamName = (string)InitPayload.streamName;
        }

        public ClientRole GetRole()
        {
            throw new NotImplementedException();
        }

        public string GetClientId()
        {
            throw new NotImplementedException();
        }

        public void TogglePaused(bool status)
        {
            throw new NotImplementedException();
        }

        public void ToggleVisibility(bool status)
        {
            throw new NotImplementedException();
        }

        public void ToggleLayerVisibility(string layerId, bool status)
        {
            throw new NotImplementedException();
        }

        public void ToggleLayerHover(string layerId, bool status)
        {
            throw new NotImplementedException();
        }

        public void Dispose(bool delete = false)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        internal void AddTrackedObjects(string[] guids)
        {
            throw new NotImplementedException();
        }

        internal void RemoveTrackedObjects(string[] guids)
        {
            throw new NotImplementedException();
        }

        internal void ForceUpdate()
        {
            throw new NotImplementedException();
        }

        internal void CompleteDeserialisation(Interop interop)
        {
            throw new NotImplementedException();
        }
    }
}
