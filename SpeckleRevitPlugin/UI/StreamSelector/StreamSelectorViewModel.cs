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
        public RelayCommand<SpeckleStream> StreamSelected { get; set; }

        private List<SpeckleStream> _streams = new List<SpeckleStream>();
        public List<SpeckleStream> Streams
        {
            get { return _streams; }
            set { _streams = value; RaisePropertyChanged(() => Streams); }
        }

        private SpeckleStream _selectedStream;
        public SpeckleStream SelectedStream
        {
            get { return _selectedStream; }
            set { _selectedStream = value; RaisePropertyChanged(() => SelectedStream); }
        }

        private List<Layer> _layers = new List<Layer>();
        public List<Layer> Layers
        {
            get { return _layers; }
            set { _layers = value; RaisePropertyChanged(() => Layers); }
        }

        private Layer _selectedLayer;
        public Layer SelectedLayer
        {
            get { return _selectedLayer; }
            set { _selectedLayer = value; RaisePropertyChanged(() => SelectedLayer); }
        }

        #endregion

        public StreamSelectorViewModel(StreamSelectorModel model)
        {
            Model = model;
            Streams = Model.GetStreams();
            StreamSelected = new RelayCommand<SpeckleStream>(OnStreamSelected);
        }

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void OnStreamSelected(SpeckleStream stream)
        {
            // (Konrad) It's possible for this to be null since we can hide the comboboxes
            if (stream == null) return;

            Layers = Model.GetLayers(stream);
        }

        #endregion
    }
}
