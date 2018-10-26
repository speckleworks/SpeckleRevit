using System;
using System.Runtime.Serialization;
using SpeckleCore;

namespace SpeckleRevitPlugin.Classes
{
    /// <summary>
    /// Generalises some methods for both senders and receivers.
    /// </summary>
    public interface ISpeckleRevitClient : IDisposable, ISerializable
    {
        ClientRole GetRole();

        string GetClientId();

        void TogglePaused(bool status);

        void ToggleVisibility(bool status);

        void ToggleSpeckleLayerVisibility(string layerId, bool status);

        void ToggleSpeckleLayerHover(string layerId, bool status);

        void Dispose(bool delete = false);
    }
}
