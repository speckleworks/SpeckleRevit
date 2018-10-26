using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeckleCore;
using SpeckleRevitPlugin.Classes;

namespace SpeckleRevitPlugin.UI.StreamSelector
{
    public class StreamSelectorModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DataStream> GetStreams()
        {
            var streams = FormMainDock.Store.UserClients.Where(x => x.GetRole() == ClientRole.Receiver)
                .Cast<RevitReceiver>()
                .Select(x => x.Client.Stream)
                .OrderBy(x => x.Name)
                .ToList();
            return streams.Any()
                ? streams
                : new List<DataStream>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public List<SpeckleLayer> GetLayers(DataStream stream)
        {
            return stream.Layers;
        }
    }
}
