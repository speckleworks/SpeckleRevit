using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using SpeckleRevitPlugin.UI;

namespace SpeckleRevitPlugin.Tools.WallTool
{
    public class SpeckleWallViewModel : ViewModelBase
    {
        #region Parameters

        private SpeckleWallModel Model { get; set; }
        public RelayCommand CloseFlyout { get; set; }
        public RelayCommand AddInput { get; set; }
        public RelayCommand<MetroWindow> WindowClosed { get; set; }

        private ObservableCollection<InputViewModel> _inputs;
        public ObservableCollection<InputViewModel> Inputs
        {
            get { return _inputs; }
            set { _inputs = value; RaisePropertyChanged(() => Inputs); }
        }

        private ObservableCollection<InputWrapper> _availableInputs;
        public ObservableCollection<InputWrapper> AvailableInputs
        {
            get { return _availableInputs; }
            set { _availableInputs = value; RaisePropertyChanged(() => AvailableInputs); }
        }

        private InputWrapper _selectedInput;
        public InputWrapper SelectedInput
        {
            get { return _selectedInput; }
            set { _selectedInput = value; RaisePropertyChanged(() => SelectedInput); }
        }

        private bool _addInputChecked;
        public bool AddInputChecked
        {
            get { return _addInputChecked; }
            set { _addInputChecked = value; RaisePropertyChanged(() => AddInputChecked); }
        }

        #endregion

        public SpeckleWallViewModel(SpeckleWallModel model)
        {
            Model = model;
            AvailableInputs = Model.GetAllAvailableInputs();

            CloseFlyout = new RelayCommand(OnCloseFlyout);
            AddInput = new RelayCommand(OnAddInput);
            WindowClosed = new RelayCommand<MetroWindow>(OnWindowClosed);

            Inputs = new ObservableCollection<InputViewModel>
            {
                // (Konrad) Forces the Curve input to only use Speckle Stream as source of data.
                new InputViewModel(new InputModel(), new InputWrapper
                {
                    Name = "Curve",
                    AcceptsLocalData = false,
                    IsRequired = true
                }),
                //new InputViewModel(new InputModel(), new InputWrapper
                //{
                //    Name = "Level",
                //    AcceptsLocalData = true,
                //    StorageType = LocalDataType.Element
                //}),
                //new InputViewModel(new InputModel(), new InputWrapper
                //{
                //    Name = "Structural",
                //    AcceptsLocalData = true,
                //    StorageType = LocalDataType.Boolean
                //})
            };

            Messenger.Default.Register<InputDeleted>(this, OnInputDeleted);
        }

        private void OnWindowClosed(MetroWindow obj)
        {
            // (Konrad) Removes all Messanger bindings.
            Cleanup();
        }

        private void OnInputDeleted(InputDeleted obj)
        {
            // (Konrad) Restore the input in the dropdown and cleanup
            AvailableInputs.Add(obj.InputViewModel.Input);
            Inputs.Remove(obj.InputViewModel);
        }

        private void OnAddInput()
        {
            if (SelectedInput == null) return;

            OnCloseFlyout();

            Inputs.Add(new InputViewModel(new InputModel(), SelectedInput));

            // (Konrad) Cleanup Available Inputs
            AvailableInputs.Remove(SelectedInput);
            SelectedInput = null;
        }

        private void OnCloseFlyout()
        {
            AddInputChecked = false;
        }
    }
}
