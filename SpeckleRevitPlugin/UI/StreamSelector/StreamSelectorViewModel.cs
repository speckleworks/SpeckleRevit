#region Namespaces

using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SpeckleCore;

#endregion

namespace SpeckleRevitPlugin.UI.StreamSelector
{
    public class StreamSelectorViewModel : ViewModelBase
    {
        #region Properties

        public StreamSelectorModel Model { get; set; }
        public RelayCommand<DataStream> StreamSelected { get; set; }

        private List<DataStream> _streams = new List<DataStream>();
        public List<DataStream> Streams
        {
            get { return _streams; }
            set { _streams = value; RaisePropertyChanged(() => Streams); }
        }

        private DataStream _selectedStream;
        public DataStream SelectedStream
        {
            get { return _selectedStream; }
            set { _selectedStream = value; RaisePropertyChanged(() => SelectedStream); }
        }

        private List<SpeckleLayer> _layers = new List<SpeckleLayer>();
        public List<SpeckleLayer> Layers
        {
            get { return _layers; }
            set { _layers = value; RaisePropertyChanged(() => Layers); }
        }

        private SpeckleLayer _selectedLayer;
        public SpeckleLayer SelectedLayer
        {
            get { return _selectedLayer; }
            set { _selectedLayer = value; RaisePropertyChanged(() => SelectedLayer); }
        }

        #endregion

        public StreamSelectorViewModel(StreamSelectorModel model)
        {
            Model = model;
            Streams = Model.GetStreams();
            StreamSelected = new RelayCommand<DataStream>(OnStreamSelected);
        }

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void OnStreamSelected(DataStream stream)
        {
            // (Konrad) It's possible for this to be null since we can hide the comboboxes
            if (stream == null) return;

            Layers = Model.GetLayers(stream);
        }

        #endregion
    }
}
