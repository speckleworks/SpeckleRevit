using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpeckleCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleRevit
{
  /// <summary>
  /// Class that holds a rhino receiver client warpped around the
  /// SpeckleApiClient.
  /// </summary>
  [Serializable]
  public class RevitReceiver : ISpeckleClient
  {
    public Interop Context { get; set; }

    public SpeckleApiClient Client { get; set; }

    public List<SpeckleObject> Objects { get; set; }

    public string StreamId { get; private set; }

    public bool Paused { get; set; } = false;

    public bool Visible { get; set; } = true;

    public RevitReceiver( string _payload, Interop _parent )
    {
      Context = _parent;
      dynamic payload = JsonConvert.DeserializeObject( _payload );

      StreamId = ( string ) payload.streamId;

      Client = new SpeckleApiClient( ( string ) payload.account.restApi, true );

      Client.OnReady += Client_OnReady;
      Client.OnLogData += Client_OnLogData;
      Client.OnWsMessage += Client_OnWsMessage;
      Client.OnError += Client_OnError;

      Client.IntializeReceiver( ( string ) payload.streamId, Context.GetDocumentName(), "Revit", Context.GetDocumentGuid(), ( string ) payload.account.apiToken );

      Objects = new List<SpeckleObject>();
    }

    #region events
    private void Client_OnError( object source, SpeckleEventArgs e )
    {
      Context.NotifySpeckleFrame( "client-error", StreamId, JsonConvert.SerializeObject( e.EventData ) );
    }

    public virtual void Client_OnLogData( object source, SpeckleEventArgs e )
    {
      Context.NotifySpeckleFrame( "client-log", StreamId, JsonConvert.SerializeObject( e.EventData ) );
    }

    public virtual void Client_OnReady( object source, SpeckleEventArgs e )
    {
      Context.NotifySpeckleFrame( "client-add", StreamId, JsonConvert.SerializeObject( new { stream = Client.Stream, client = Client } ) );

      Context.UserClients.Add( this );

      UpdateGlobal();
    }

    public virtual void Client_OnWsMessage( object source, SpeckleEventArgs e )
    {
      if ( Paused )
      {
        Context.NotifySpeckleFrame( "client-expired", StreamId, "" );
        return;
      }

      switch ( ( string ) e.EventObject.args.eventType )
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
          Context.NotifySpeckleFrame( "client-log", StreamId, JsonConvert.SerializeObject( "Unkown event: " + ( string ) e.EventObject.args.eventType ) );
          break;
      }
    }
    #endregion

    #region updates

    public void UpdateName( )
    {
      try
      {
        var response = Client.StreamGetNameAsync( StreamId );
        Client.Stream.Name = response.Result.Name;
        Context.NotifySpeckleFrame( "client-metadata-update", StreamId, Client.Stream.ToJson() ); // i'm lazy
      }
      catch ( Exception err )
      {
        Context.NotifySpeckleFrame( "client-error", Client.Stream.StreamId, JsonConvert.SerializeObject( err.Message ) );
        Context.NotifySpeckleFrame( "client-done-loading", StreamId, "" );
        return;
      }
    }

    public void UpdateMeta( )
    {
      Context.NotifySpeckleFrame( "client-log", StreamId, JsonConvert.SerializeObject( "Metadata update received." ) );

      try
      {
        var streamGetResponse = Client.StreamGet( StreamId );

        if ( streamGetResponse.Success == false )
        {
          Context.NotifySpeckleFrame( "client-error", StreamId, streamGetResponse.Message );
          Context.NotifySpeckleFrame( "client-log", StreamId, JsonConvert.SerializeObject( "Failed to retrieve global update." ) );
        }

        Client.Stream = streamGetResponse.Stream;

        Context.NotifySpeckleFrame( "client-metadata-update", StreamId, Client.Stream.ToJson() );
      }
      catch(Exception err)
      {
        Context.NotifySpeckleFrame( "client-error", Client.Stream.StreamId, JsonConvert.SerializeObject( err.Message ) );
        Context.NotifySpeckleFrame( "client-done-loading", StreamId, "" );
        return;
      }

    }

    public void UpdateGlobal( )
    {
      Context.NotifySpeckleFrame( "client-log", StreamId, JsonConvert.SerializeObject( "Global update received." ) );

      try
      {
        var streamGetResponse = Client.StreamGet( StreamId );
        if ( streamGetResponse.Success == false )
        {
          Context.NotifySpeckleFrame( "client-error", StreamId, streamGetResponse.Message );
          Context.NotifySpeckleFrame( "client-log", StreamId, JsonConvert.SerializeObject( "Failed to retrieve global update." ) );
        }


        Client.Stream = streamGetResponse.Stream;
        var COPY = Client.Stream;
        Context.NotifySpeckleFrame( "client-metadata-update", StreamId, Client.Stream.ToJson() );
        Context.NotifySpeckleFrame( "client-is-loading", StreamId, "" );

        // prepare payload
        PayloadObjectGetBulk payload = new PayloadObjectGetBulk();
        payload.Objects = Client.Stream.Objects.Where( o => !Context.SpeckleObjectCache.ContainsKey( o ) );

        // bug in speckle core, no sync method for this :(
        Client.ObjectGetBulkAsync( "omit=displayValue", payload ).ContinueWith( tres =>
           {
             if ( tres.Result.Success == false )
               Context.NotifySpeckleFrame( "client-error", StreamId, streamGetResponse.Message );
             var copy = tres.Result;

           // add to cache
           foreach ( var obj in tres.Result.Objects )
               Context.SpeckleObjectCache[ obj.DatabaseId ] = obj;

           // populate real objects
           Objects.Clear();
             foreach ( var objId in Client.Stream.Objects )
               Objects.Add( Context.SpeckleObjectCache[ objId ] );

             //DisplayContents();
             Context.NotifySpeckleFrame( "client-done-loading", StreamId, "" );
           } );
      }
      catch ( Exception err )
      {
        Context.NotifySpeckleFrame( "client-error", Client.Stream.StreamId, JsonConvert.SerializeObject( err.Message ) );
        Context.NotifySpeckleFrame( "client-done-loading", StreamId, "" );
        return;
      }

    }
    public void UpdateChildren( )
    {
      try
      {
        var getStream = Client.StreamGet( StreamId );
        Client.Stream = getStream.Stream;

        Context.NotifySpeckleFrame( "client-children", StreamId, Client.Stream.ToJson() );
      }
      catch ( Exception err )
      {
        Context.NotifySpeckleFrame( "client-error", Client.Stream.StreamId, JsonConvert.SerializeObject( err.Message ) );
        Context.NotifySpeckleFrame( "client-done-loading", StreamId, "" );
        return;
      }
    }
    #endregion

    #region display & helpers

    public SpeckleLayer GetLayerFromIndex( int index )
    {
      return Client.Stream.Layers.FirstOrDefault( layer => ( ( index >= layer.StartIndex ) && ( index < layer.StartIndex + layer.ObjectCount ) ) );
    }

    public System.Drawing.Color GetColorFromLayer( SpeckleLayer layer )
    {
      System.Drawing.Color layerColor = System.Drawing.ColorTranslator.FromHtml( "#AEECFD" );
      try
      {
        if ( layer != null && layer.Properties != null )
          layerColor = System.Drawing.ColorTranslator.FromHtml( layer.Properties.Color.Hex );
      }
      catch
      {
        Debug.WriteLine( "Layer '{0}' had no assigned color", layer.Name );
      }
      return layerColor;
    }

    public string GetClientId( )
    {
      return Client.ClientId;
    }

    public ClientRole GetRole( )
    {
      return ClientRole.Receiver;
    }
    #endregion

    #region Toggles

    public void TogglePaused( bool status )
    {
      this.Paused = status;
    }

    public void ToggleVisibility( bool status )
    {
            throw new NotImplementedException();
    }

    public void ToggleLayerHover( string layerId, bool status )
    {
            throw new NotImplementedException();
        }

    public void ToggleLayerVisibility( string layerId, bool status )
    {
            throw new NotImplementedException();
        }
    #endregion

    #region serialisation & end of life

    public void Dispose( bool delete = false )
    {
      Client.Dispose( delete );
      //Display.Enabled = false;
      //Rhino.RhinoDoc.ActiveDoc?.Views.Redraw();
    }

    public void Dispose( )
    {
      Client.Dispose();
      //Display.Enabled = false;
      //Rhino.RhinoDoc.ActiveDoc?.Views.Redraw();
    }

    protected RevitReceiver( SerializationInfo info, StreamingContext context )
    {
      JsonConvert.DefaultSettings = ( ) => new JsonSerializerSettings()
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };

      //Display = new SpeckleDisplayConduit();
      //Display.Enabled = true;

      Objects = new List<SpeckleObject>();

      byte[ ] serialisedClient = Convert.FromBase64String( ( string ) info.GetString( "client" ) );

      using ( var ms = new MemoryStream() )
      {
        ms.Write( serialisedClient, 0, serialisedClient.Length );
        ms.Seek( 0, SeekOrigin.Begin );
        Client = ( SpeckleApiClient ) new BinaryFormatter().Deserialize( ms );
        StreamId = Client.StreamId;
      }

      Client.OnReady += Client_OnReady;
      Client.OnLogData += Client_OnLogData;
      Client.OnWsMessage += Client_OnWsMessage;
      Client.OnError += Client_OnError;
    }

    public void GetObjectData( SerializationInfo info, StreamingContext context )
    {
      using ( var ms = new MemoryStream() )
      {
        var formatter = new BinaryFormatter();
        formatter.Serialize( ms, Client );
        info.AddValue( "client", Convert.ToBase64String( ms.ToArray() ) );
        info.AddValue( "paused", Paused );
        info.AddValue( "visible", Visible );
      }
    }

        internal void BakeLayer(string layerGuid)
        {
            throw new NotImplementedException();
        }

        internal void Bake()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
