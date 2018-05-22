using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpeckleCore;
using SpeckleRevitPlugin.Utilities;

namespace SpeckleRevitPlugin.Classes
{
    /// <summary>
    /// Class that holds a rhino receiver client warpped around the SpeckleApiClient.
    /// </summary>
    [Serializable]
    public class RevitReceiver : ISpeckleRevitClient
    {
        public Interop Context { get; set; }
        public SpeckleApiClient Client { get; set; }
        public List<SpeckleObject> Objects { get; set; }
        public string StreamId { get; private set; }
        public bool Paused { get; set; }
        public bool Visible { get; set; } = true;

        public RevitReceiver()
        {
        }

        public RevitReceiver( string payload, Interop parent )
        {
            Context = parent;
            dynamic p = JsonConvert.DeserializeObject(payload);

            StreamId = (string) p.streamId;
            Client = new SpeckleApiClient((string) p.account.restApi, true);

            Client.OnReady += Client_OnReady;
            Client.OnLogData += Client_OnLogData;
            Client.OnWsMessage += Client_OnWsMessage;
            Client.OnError += Client_OnError;

            Client.IntializeReceiver((string) p.streamId,
                Context.GetDocumentName(),
                "Revit",
                Context.GetDocumentGuid(),
                (string) p.account.apiToken);

            Objects = new List<SpeckleObject>();
        }

        public string GetClientId()
        {
            return Client.ClientId;
        }

        public ClientRole GetRole()
        {
            return ClientRole.Receiver;
        }

        #region Events

        private void Client_OnError( object source, SpeckleEventArgs e )
        {
            Context.NotifySpeckleFrame( "client-error", StreamId, JsonConvert.SerializeObject( e.EventData ) );
        }

        public virtual void Client_OnLogData( object source, SpeckleEventArgs e )
        {
            Context.NotifySpeckleFrame( "client-log", StreamId, JsonConvert.SerializeObject( e.EventData ) );
        }

        public virtual void Client_OnReady(object source, SpeckleEventArgs e)
        {
            Context.NotifySpeckleFrame("client-add", StreamId, JsonConvert.SerializeObject(new
            {
                client = Client,
                stream = Client.Stream
            }));

            Context.UserClients.Add(this);
            UpdateGlobal();
        }

        public virtual void Client_OnWsMessage(object source, SpeckleEventArgs e)
        {
            if (Paused)
            {
                Context.NotifySpeckleFrame("client-expired", StreamId, "");
                return;
            }

            switch ((string)e.EventObject.args.eventType)
            {
                case "update-global":
                    UpdateGlobal();
                    break;
                case "update-meta":
                    UpdateMeta();
                    break;
                case "update-name":
                    UpdateName();
                    break;
                case "update-object":
                    break;
                case "update-children":
                    UpdateChildren();
                    break;
                default:
                    Context.NotifySpeckleFrame("client-log", StreamId, JsonConvert.SerializeObject("Unkown event: " + (string)e.EventObject.args.eventType));
                    break;
            }
        }
        #endregion

        #region Updates

        public void UpdateName()
        {
            try
            {
                var response = Client.StreamGetAsync(StreamId, "fields=name");
                Client.Stream.Name = response.Result.Resource.Name;
                Context.NotifySpeckleFrame("client-metadata-update", StreamId, Client.Stream.ToJson());
            }
            catch (Exception err)
            {
                Context.NotifySpeckleFrame("client-error", Client.Stream.StreamId, JsonConvert.SerializeObject(err.Message));
                Context.NotifySpeckleFrame("client-done-loading", StreamId, "");
            }
        }

        public void UpdateMeta()
        {
            Context.NotifySpeckleFrame("client-log", StreamId, JsonConvert.SerializeObject("Metadata update received."));

            try
            {
                var streamGetResponse = Client.StreamGetAsync(StreamId, null).Result;

                if (streamGetResponse.Success == false)
                {
                    Context.NotifySpeckleFrame("client-error", StreamId, streamGetResponse.Message);
                    Context.NotifySpeckleFrame("client-log", StreamId, JsonConvert.SerializeObject("Failed to retrieve global update."));
                }

                Client.Stream = streamGetResponse.Resource;

                Context.NotifySpeckleFrame("client-metadata-update", StreamId, Client.Stream.ToJson());
            }
            catch (Exception err)
            {
                Context.NotifySpeckleFrame("client-error", Client.Stream.StreamId, JsonConvert.SerializeObject(err.Message));
                Context.NotifySpeckleFrame("client-done-loading", StreamId, "");
            }
        }

        public void UpdateGlobal()
        {
            Context.NotifySpeckleFrame("client-log", StreamId, JsonConvert.SerializeObject("Global update received."));

            try
            {
                var streamGetResponse = Client.StreamGetAsync(StreamId, null).Result;
                if (streamGetResponse.Success == false)
                {
                    Context.NotifySpeckleFrame("client-error", StreamId, streamGetResponse.Message);
                    Context.NotifySpeckleFrame("client-log", StreamId, JsonConvert.SerializeObject("Failed to retrieve global update."));
                }

                Client.Stream = streamGetResponse.Resource;
                Context.NotifySpeckleFrame("client-metadata-update", StreamId, Client.Stream.ToJson());
                Context.NotifySpeckleFrame("client-is-loading", StreamId, "");

                // prepare payload
                var payload = Client.Stream.Objects.Where(o => !Context.SpeckleObjectCache.ContainsKey(o._id)).Select(obj => obj._id).ToArray();
                var getObjectsResult = Client.ObjectGetBulkAsync(payload, "omit=displayValue").Result;

                if (getObjectsResult.Success == false)
                    Context.NotifySpeckleFrame("client-error", StreamId, streamGetResponse.Message);

                // add to cache
                foreach (var obj in getObjectsResult.Resources)
                {
                    Context.SpeckleObjectCache[obj._id] = obj;
                }

                // populate real objects
                Objects.Clear();
                foreach (var obj in Client.Stream.Objects)
                {
                    Objects.Add(Context.SpeckleObjectCache[obj._id]);
                }

                //DisplayContents();
                Context.NotifySpeckleFrame("client-done-loading", StreamId, "");
            }
            catch (Exception err)
            {
                Context.NotifySpeckleFrame("client-error", Client.Stream.StreamId, JsonConvert.SerializeObject(err.Message));
                Context.NotifySpeckleFrame("client-done-loading", StreamId, "");
            }
        }

        public void UpdateChildren()
        {
            try
            {
                var getStream = Client.StreamGetAsync(StreamId, null).Result;
                Client.Stream = getStream.Resource;

                Context.NotifySpeckleFrame("client-children", StreamId, Client.Stream.ToJson());
            }
            catch (Exception err)
            {
                Context.NotifySpeckleFrame("client-error", Client.Stream.StreamId, JsonConvert.SerializeObject(err.Message));
                Context.NotifySpeckleFrame("client-done-loading", StreamId, "");
            }
        }

        #endregion

        #region Toggles

        public void TogglePaused(bool status)
        {
            Paused = status;
        }

        public void ToggleVisibility(bool status)
        {
            //TODO: Implement
        }

        public void ToggleLayerVisibility(string layerId, bool status)
        {
            //TODO: Implement
        }

        public void ToggleLayerHover(string layerId, bool status)
        {
            //TODO: Implement
        }

        #endregion

        #region Serialisation & Dispose

        public void Dispose(bool delete = false)
        {
            Client.Dispose(delete);
        }

        public void Dispose()
        {
            Client.Dispose();
        }

        public void CompleteDeserialisation(Interop _Context)
        {
            Context = _Context;

            Context.NotifySpeckleFrame("client-add", StreamId, JsonConvert.SerializeObject(new { stream = Client.Stream, client = Client }));
            Context.UserClients.Add(this);
        }

        protected RevitReceiver(SerializationInfo info, StreamingContext context)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            Objects = new List<SpeckleObject>();

            var serialisedClient = Convert.FromBase64String(info.GetString("client"));

            using (var ms = new MemoryStream())
            {
                ms.Write(serialisedClient, 0, serialisedClient.Length);
                ms.Seek(0, SeekOrigin.Begin);
                var bf = new BinaryFormatter
                {
                    Binder = new SearchAssembliesBinder(Assembly.GetExecutingAssembly(), true)
                };
                Client = (SpeckleApiClient)bf.Deserialize(ms);
                StreamId = Client.StreamId;
            }

            Client.OnReady += Client_OnReady;
            Client.OnLogData += Client_OnLogData;
            Client.OnWsMessage += Client_OnWsMessage;
            Client.OnError += Client_OnError;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, Client);
                info.AddValue("client", Convert.ToBase64String(ms.ToArray()));
                info.AddValue("paused", Paused);
                info.AddValue("visible", Visible);
            }
        }

        #endregion
    }
}
