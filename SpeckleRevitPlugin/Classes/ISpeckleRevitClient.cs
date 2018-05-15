using System;
using System.Runtime.Serialization;
using SpeckleCore;

namespace SpeckleRevitPlugin.Classes
{
    /// <summary>
    /// Generalises some methhods for both senders and receivers.
    /// </summary>
    public interface ISpeckleRevitClient : IDisposable, ISerializable
    {
        ClientRole GetRole();

        string GetClientId();

        void TogglePaused(bool status);

        void ToggleVisibility(bool status);

        void ToggleLayerVisibility(string layerId, bool status);

        void ToggleLayerHover(string layerId, bool status);

        void Dispose(bool delete = false);
    }
}
