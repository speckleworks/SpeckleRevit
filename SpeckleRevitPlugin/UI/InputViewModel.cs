#region Namespaces

using System;
using Autodesk.Revit.DB;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SpeckleRevitPlugin.Tools.WallTool;
using SpeckleRevitPlugin.UI.BooleanSelector;
using SpeckleRevitPlugin.UI.ElementSelector;
using SpeckleRevitPlugin.UI.StreamSelector;

#endregion

namespace SpeckleRevitPlugin.UI
{
    public class InputViewModel : ViewModelBase
    {
        #region Properties

        public InputModel Model { get; set; }
        public RelayCommand ToggleMenu { get; set; }
        public RelayCommand Delete { get; set; }
        public RelayCommand<bool> ToggleSpeckleInput { get; set; }

        private InputWrapper _input;
        public InputWrapper Input
        {
            get { return _input; }
            set { _input = value; RaisePropertyChanged(() => Input); }
        }

        private bool _showMenu;
        public bool ShowMenu
        {
            get { return _showMenu; }
            set { _showMenu = value; RaisePropertyChanged(() => ShowMenu); }
        }

        private bool _showInputs;
        public bool ShowInputs
        {
            get { return _showInputs; }
            set { _showInputs = value; RaisePropertyChanged(() => ShowInputs); }
        }

        private bool _isSpeckleInput;
        public bool IsSpeckleInput
        {
            get { return _isSpeckleInput; }
            set { _isSpeckleInput = value; RaisePropertyChanged(() => IsSpeckleInput); }
        }

        private ViewModelBase _selectedDataInput;
        public ViewModelBase SelectedDataInput
        {
            get { return _selectedDataInput; }
            set { _selectedDataInput = value; RaisePropertyChanged(() => SelectedDataInput); }
        }

        #endregion

        public InputViewModel(InputModel model, InputWrapper input)
        {
            Model = model;
            Input = input;

            ToggleMenu = new RelayCommand(OnToggleMenu);
            Delete = new RelayCommand(OnDelete);
            ToggleSpeckleInput = new RelayCommand<bool>(OnToggleSpeckleInput);

            // (Konrad) Set defaults. In case that input doesn't accept
            // locally defined data we need to ensure that Speckle Stream
            // input is shown at all times.
            if (!Input.AcceptsLocalData) IsSpeckleInput = true;
            OnToggleSpeckleInput(IsSpeckleInput);
        }

        private void OnDelete()
        {
            Messenger.Default.Send(new InputDeleted { InputViewModel = this });
            ShowMenu = false;
        }

        private void OnToggleMenu()
        {
            ShowMenu = !ShowMenu;
        }

        #region Events

        private void OnToggleSpeckleInput(bool isChecked)
        {
            if (isChecked)
            {
                // toggle speckle stream on
                SelectedDataInput = new StreamSelectorViewModel(new StreamSelectorModel());
            }
            else
            {
                // toggle local data input
                switch (Input.DataType)
                {
                    case LocalDataType.None:
                        break;
                    case LocalDataType.Element:
                        SelectedDataInput = new ElementSelectorViewModel();
                        break;
                    case LocalDataType.Integer:
                        break;
                    case LocalDataType.Boolean:
                        SelectedDataInput = new BooleanSelectorViewModel(new BooleanSelectorModel());
                        break;
                    case LocalDataType.String:
                        break;
                    case LocalDataType.Double:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        #endregion
    }
}
