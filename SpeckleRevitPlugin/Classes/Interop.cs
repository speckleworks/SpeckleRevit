#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpeckleCore;
#endregion

namespace SpeckleRevitPlugin.Classes
{
    /// <summary>
    /// CEF Bound object.
    /// If CEF will be removed, porting to url hacks will be necessary,
    /// so let's keep the methods as simple as possible.
    /// </summary>
    public class Interop : IDisposable
    {
        private List<SpeckleAccount> _userAccounts;
        public ChromiumWebBrowser Browser;
        public List<ISpeckleRevitClient> UserClients;
        public Dictionary<string, SpeckleObject> SpeckleObjectCache;
        public bool SpeckleIsReady;
        public bool SelectionInfoNeedsToBeSentYeMighty = false; // should be false

        public Interop(ChromiumWebBrowser originalBrowser)
        {
            // Makes sure we always get some camelCaseLove
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            Browser = originalBrowser;
            UserClients = new List<ISpeckleRevitClient>();
            _userAccounts = new List<SpeckleAccount>();
            SpeckleObjectCache = new Dictionary<string, SpeckleObject>();
            ReadUserAccounts();
        }

        public void SetBrowser( ChromiumWebBrowser browser )
        {
            Browser = browser;
        }

        public void Dispose( )
        {
            RemoveAllClients();
        }

        public void ShowDev( )
        {
            Browser.ShowDevTools();
        }

        public string GetDocumentName()
        {
            //TODO: Fix this!
            return "Revit doc name.";
            //return Rhino.RhinoDoc.ActiveDoc.Name;
        }

        public string GetDocumentGuid()
        {
            //TODO: Fix this!
            return "Revit GUID";
            //return Rhino.RhinoDoc.ActiveDoc.DocumentId.ToString();
        }

        /// <summary>
        /// Do not call this from the constructor as you'll get confilcts with 
        /// browser load, etc.
        /// </summary>
        public void AppReady( )
        {
            SpeckleIsReady = true;
            InstantiateFileClients();
        }

        //TODO: This is called when document is saved. In Revit that would be either document save or synch events.
        //public void SaveFileClients( )
        //{
        //  RhinoDoc myDoc = RhinoDoc.ActiveDoc;
        //  foreach ( ISpeckleRhinoClient rhinoClient in UserClients )
        //  {
        //    using ( var ms = new MemoryStream() )
        //    {
        //      var formatter = new BinaryFormatter();
        //      formatter.Serialize( ms, rhinoClient );
        //      string section = rhinoClient.GetRole() == ClientRole.Receiver ? "speckle-client-receivers" : "speckle-client-senders";
        //      var client = Convert.ToBase64String( ms.ToArray() );
        //      var clientId = rhinoClient.GetClientId();
        //      RhinoDoc.ActiveDoc.Strings.SetString( section, clientId, client );
        //    }
        //  }
        //}

        public void InstantiateFileClients()
        {
            //TODO: these can be stored in a extensible storage schema and then de-serialized here!
            //if (!SpeckleIsReady) return;

            //Debug.WriteLine("Instantiate file clients.");

            //string[] receiverKeys = RhinoDoc.ActiveDoc.Strings.GetEntryNames("speckle-client-receivers");

            //foreach (string rec in receiverKeys)
            //{
            //    //if ( UserClients.Any( cl => cl.GetClientId() == rec ) )
            //    //  continue;

            //    byte[] serialisedClient = Convert.FromBase64String(RhinoDoc.ActiveDoc.Strings.GetValue("speckle-client-receivers", rec));
            //    using (var ms = new MemoryStream())
            //    {
            //        ms.Write(serialisedClient, 0, serialisedClient.Length);
            //        ms.Seek(0, SeekOrigin.Begin);
            //        RhinoReceiver client = (RhinoReceiver)new BinaryFormatter().Deserialize(ms);
            //        client.Context = this;
            //    }
            //}

            //string[] senderKeys = RhinoDoc.ActiveDoc.Strings.GetEntryNames("speckle-client-senders");

            //foreach (string sen in senderKeys)
            //{
            //    byte[] serialisedClient = Convert.FromBase64String(RhinoDoc.ActiveDoc.Strings.GetValue("speckle-client-senders", sen));

            //    using (var ms = new MemoryStream())
            //    {
            //        ms.Write(serialisedClient, 0, serialisedClient.Length);
            //        ms.Seek(0, SeekOrigin.Begin);
            //        RhinoSender client = (RhinoSender)new BinaryFormatter().Deserialize(ms);
            //        client.CompleteDeserialisation(this);
            //    }
            //}
        }

        #region Account Management

        public string GetUserAccounts( )
        {
            ReadUserAccounts();
            return JsonConvert.SerializeObject(_userAccounts, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        private void ReadUserAccounts( )
        {
            _userAccounts = new List<SpeckleAccount>();
            var strPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            strPath = strPath + @"\SpeckleSettings";

            if (!Directory.Exists(strPath) || !Directory.EnumerateFiles(strPath, "*.txt").Any()) return;

            foreach (var file in Directory.EnumerateFiles(strPath, "*.txt"))
            {
                var content = File.ReadAllText(file);
                var pieces = content.TrimEnd('\r', '\n').Split(',');
                _userAccounts.Add(new SpeckleAccount
                {
                    email = pieces[0],
                    apiToken = pieces[1],
                    serverName = pieces[2],
                    restApi = pieces[3],
                    rootUrl = pieces[4],
                    fileName = file
                });
            }
        }

        public void AddAccount( string payload )
        {
            var pieces = payload.Split(',');
            var strPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Directory.CreateDirectory(strPath + @"\SpeckleSettings");
            strPath = strPath + @"\SpeckleSettings\";

            var fileName = pieces[0] + "." + pieces[2] + ".txt";
            var file = new StreamWriter(strPath + fileName);
            file.WriteLine(payload);
            file.Close();
        }

        public void RemoveAccount( string payload )
        {
            var unused = _userAccounts.RemoveAll(account => account.fileName == payload);
            if (File.Exists(payload)) File.Delete(payload);
        }

        #endregion

        #region Client Management

        public bool AddReceiverClient( string payload )
        {
            var unused = new RevitReceiver(payload, this);
            return true;
        }

        //public bool AddSenderClientFromSelection( string _payload )
        //{
        //  var mySender = new RhinoSender( _payload, this );
        //  return true;
        //}

        //public bool RemoveClient( string _payload )
        //{
        //  var myClient = UserClients.FirstOrDefault( client => client.GetClientId() == _payload );
        //  if ( myClient == null ) return false;

        //  RhinoDoc.ActiveDoc.Strings.Delete( myClient.GetRole() == ClientRole.Receiver ? "speckle-client-receivers" : "speckle-client-senders", myClient.GetClientId() );

        //  myClient.Dispose( true );

        //  return UserClients.Remove( myClient );
        //}

        public bool RemoveAllClients( )
        {
            foreach (var uc in UserClients)
            {
                uc.Dispose();
            }
            UserClients.RemoveAll(c => true);
            return true;
        }

        public string GetAllClients( )
        {
            foreach (var client in UserClients)
            {
                switch (client)
                {
                    case RevitSender rvtSender:
                        NotifySpeckleFrame("client-add", rvtSender.StreamId, JsonConvert.SerializeObject(new
                        {
                            stream = rvtSender.Client.Stream,
                            client = rvtSender.Client
                        }));
                        break;
                    case RevitReceiver rvtReceiver:
                        NotifySpeckleFrame("client-add", rvtReceiver.StreamId, JsonConvert.SerializeObject(new
                        {
                            stream = rvtReceiver.Client.Stream,
                            client = rvtReceiver.Client
                        }));
                        break;
                }
            }

            return JsonConvert.SerializeObject(UserClients);
        }

        #endregion

        #region To UI (Generic)

        public void NotifySpeckleFrame( string eventType, string streamId, string eventInfo )
        {
            if (!SpeckleIsReady)
            {
                Debug.WriteLine("Speckle was not ready, trying to send " + eventType);
                return;
            }

            var script = $"window.EventBus.$emit('{eventType}', '{streamId}', '{eventInfo}')";

            try
            {
                Browser.GetMainFrame().EvaluateScriptAsync(script);
            }
            catch
            {
                Debug.WriteLine("For some reason, this browser was not initialised.");
            }
        }
        #endregion

        #region From UI (..)

        public void BakeClient(string clientId)
        {
            //TODO: Implement client baking
        }

        public void BakeLayer(string clientId, string layerGuid)
        {
            //TODO: Implement baking
        }

        public void SetClientPause(string clientId, bool status)
        {
            var myClient = UserClients.FirstOrDefault(c => c.GetClientId() == clientId);
            myClient?.TogglePaused(status);
        }

        public void SetClientVisibility(string clientId, bool status)
        {
            var myClient = UserClients.FirstOrDefault(c => c.GetClientId() == clientId);
            myClient?.ToggleVisibility(status);
        }

        public void SetClientHover(string clientId, bool status)
        {
            var myClient = UserClients.FirstOrDefault(c => c.GetClientId() == clientId);
            myClient?.ToggleVisibility(status);
        }

        public void SetLayerVisibility(string clientId, string layerId, bool status)
        {
            //TODO: create geometry previews
        }

        public void SetLayerHover(string clientId, string layerId, bool status)
        {
            //TODO: highlight geometry previews
        }

        public void SetObjectHover(string clientId, string layerId, bool status)
        {
            //TODO: implement object hover
        }

        //public void AddRemoveObjects( string clientId, string _guids, bool remove )
        //{
        //  string[ ] guids = JsonConvert.DeserializeObject<string[ ]>( _guids );

        //  var myClient = UserClients.FirstOrDefault( c => c.GetClientId() == clientId );
        //  if ( myClient != null )
        //    try
        //    {
        //      if ( !remove )
        //        ( ( RhinoSender ) myClient ).AddTrackedObjects( guids );
        //      else ( ( RhinoSender ) myClient ).RemoveTrackedObjects( guids );

        //    }
        //    catch { throw new Exception( "Force send client was not a sender. whoopsie poopsiee." ); }
        //}

        public void RefreshClient(string clientId)
        {
            var myClient = UserClients.FirstOrDefault(c => c.GetClientId() == clientId);
            if (myClient == null) return;

            try
            {
                ((RevitReceiver) myClient).UpdateGlobal();
            }
            catch
            {
                throw new Exception("Refresh client was not a receiver. whoopsie poopsiee.");
            }
        }

        //public void forceSend( string clientId )
        //{
        //  var myClient = UserClients.FirstOrDefault( c => c.GetClientId() == clientId );
        //  if ( myClient != null )
        //    try
        //    {
        //      ( ( RhinoSender ) myClient ).ForceUpdate();
        //    }
        //    catch { throw new Exception( "Force send client was not a sender. whoopsie poopsiee." ); }
        //}

        public void OpenUrl(string url)
        {
            Process.Start(url);
        }

        //public void setName( string clientId, string name )
        //{
        //  var myClient = UserClients.FirstOrDefault( c => c.GetClientId() == clientId );
        //  if ( myClient != null && myClient is RhinoSender )
        //  {
        //    ( ( RhinoSender ) myClient ).Client.Stream.Name = name;
        //    ( ( RhinoSender ) myClient ).Client.BroadcastMessage( new { eventType = "update-name" } );
        //  }
        //}

        #endregion

        #region Sender Helpers

        //public string getLayersAndObjectsInfo( bool ignoreSelection = false )
        //{
        //  List<RhinoObject> SelectedObjects;
        //  List<LayerSelection> layerInfoList = new List<LayerSelection>();

        //  if ( !ignoreSelection )
        //  {
        //    SelectedObjects = RhinoDoc.ActiveDoc.Objects.GetSelectedObjects( false, false ).ToList();
        //    if ( SelectedObjects.Count == 0 || SelectedObjects[ 0 ] == null )
        //      return JsonConvert.SerializeObject( layerInfoList );
        //  }
        //  else
        //  {
        //    SelectedObjects = RhinoDoc.ActiveDoc.Objects.ToList();
        //    if ( SelectedObjects.Count == 0 || SelectedObjects[ 0 ] == null )
        //      return JsonConvert.SerializeObject( layerInfoList );

        //    foreach ( Rhino.DocObjects.Layer ll in RhinoDoc.ActiveDoc.Layers )
        //    {
        //      layerInfoList.Add( new LayerSelection()
        //      {
        //        objectCount = 0,
        //        layerName = ll.FullPath,
        //        color = System.Drawing.ColorTranslator.ToHtml( ll.Color ),
        //        ObjectGuids = new List<string>(),
        //        ObjectTypes = new List<string>()
        //      } );
        //    }
        //  }

        //  SelectedObjects = SelectedObjects.OrderBy( o => o.Attributes.LayerIndex ).ToList();

        //  foreach ( var obj in SelectedObjects )
        //  {
        //    var layer = RhinoDoc.ActiveDoc.Layers[ obj.Attributes.LayerIndex ];
        //    var myLInfo = layerInfoList.FirstOrDefault( l => l.layerName == layer.FullPath );

        //    if ( myLInfo != null )
        //    {
        //      myLInfo.objectCount++;
        //      myLInfo.ObjectGuids.Add( obj.Id.ToString() );
        //      myLInfo.ObjectTypes.Add( obj.Geometry.GetType().ToString() );
        //    }
        //    else
        //    {
        //      var myNewLinfo = new LayerSelection()
        //      {
        //        objectCount = 1,
        //        layerName = layer.FullPath,
        //        color = System.Drawing.ColorTranslator.ToHtml( layer.Color ),
        //        ObjectGuids = new List<string>( new string[ ] { obj.Id.ToString() } ),
        //        ObjectTypes = new List<string>( new string[ ] { obj.Geometry.GetType().ToString() } )
        //      };
        //      layerInfoList.Add( myNewLinfo );
        //    }
        //  }

        //  return Convert.ToBase64String( System.Text.Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( layerInfoList ) ) );
        //}
        #endregion
    }

    [Serializable]
    public class LayerSelection
    {
        public string LayerName;
        public int ObjectCount;
        public string Color;
        public List<string> ObjectGuids;
        public List<string> ObjectTypes;
    }
}
