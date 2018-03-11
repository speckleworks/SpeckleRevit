using System;
using System.Runtime.Serialization;


namespace SpeckleRevit
{
    /// <summary>
    /// Generalises some methhods for both senders and receivers.
    /// </summary>
    public interface ISpeckleClient : IDisposable, ISerializable
    {
        SpeckleCore.ClientRole GetRole();

        string GetClientId();

        void TogglePaused(bool status);

        void ToggleVisibility(bool status);

        void ToggleLayerVisibility(string layerId, bool status);

        void ToggleLayerHover(string layerId, bool status);

        void Dispose(bool delete = false);
    }
}
