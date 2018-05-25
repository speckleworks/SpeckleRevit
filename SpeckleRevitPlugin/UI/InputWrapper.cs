using System;
using System.ComponentModel;
using Autodesk.Revit.DB;

namespace SpeckleRevitPlugin.UI
{
    public sealed class InputWrapper : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool AcceptsLocalData { get; set; }
        public LocalDataType DataType { get; set; }
        public bool IsInstance { get; set; }
        public bool IsRequired { get; set; }

        public InputWrapper()
        {
        }

        public InputWrapper(Parameter p, bool isInstance = true, bool isRequired = false)
        {
            Name = p.Definition.Name;
            Id = p.Id.IntegerValue;
            IsInstance = isInstance;
            IsRequired = isRequired;
            DataType = StorageTypeToDataType(p);

            // (Konrad) We would make an assumption that a parameter input
            // always accepts a local input: int, bool, element etc.
            AcceptsLocalData = true;
        }

        public override bool Equals(object obj)
        {
            var item = obj as InputWrapper;
            return item != null && Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static LocalDataType StorageTypeToDataType(Parameter p)
        {
            switch (p.StorageType)
            {
                case StorageType.None:
                    return LocalDataType.None;
                case StorageType.Integer:
                    return p.Definition.ParameterType == ParameterType.YesNo
                        ? LocalDataType.Boolean
                        : LocalDataType.Integer;
                case StorageType.Double:
                    return LocalDataType.Double;
                case StorageType.String:
                    return LocalDataType.String;
                case StorageType.ElementId:
                    return LocalDataType.Element;
                default:
                    throw new ArgumentOutOfRangeException(nameof(p.StorageType), p.StorageType, null);
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }
    }

    public enum LocalDataType
    {
        None,
        Element,
        Integer,
        Boolean,
        String,
        Double
    }
}
