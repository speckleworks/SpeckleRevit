#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endregion

namespace SpeckleRevitPlugin.Utilities
{
    public static class BinaryFormatterUtilities
    {
        public static T Read<T>(byte[] content, Assembly currentAssembly) where T : new()
        {
            var result = default(T);

            try
            {
                using (var ms = new MemoryStream())
                {
                    ms.Write(content, 0, content.Length);
                    ms.Seek(0, SeekOrigin.Begin);
                    var bf = new BinaryFormatter
                    {
                        Binder = new SearchAssembliesBinder(currentAssembly, true)
                    };
                    result = (T)bf.Deserialize(ms);
                }
            }
            catch
            {
                // ignored
            }

            return result;
        }

        //public static void Write<T>(T obj, string filename)
        //{
        //    FileStream fileStream = new FileStream(filename, FileMode.Create);
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    try
        //    {
        //        formatter.Serialize(fileStream, obj);
        //    }
        //    finally
        //    {
        //        fileStream.Close();
        //    }
        //}
    }

    internal sealed class SearchAssembliesBinder : SerializationBinder
    {
        private readonly bool _searchInDlls;
        private readonly Assembly _currentAssembly;

        public SearchAssembliesBinder(Assembly currentAssembly, bool searchInDlls)
        {
            _currentAssembly = currentAssembly;
            _searchInDlls = searchInDlls;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            var assemblyNames = new List<AssemblyName>
            {
                _currentAssembly.GetName()
            };

            if (_searchInDlls)
            {
                assemblyNames.AddRange(_currentAssembly.GetReferencedAssemblies());
            }

            foreach (var an in assemblyNames)
            {
                var typeToDeserialize = GetTypeToDeserialize(typeName, an);
                if (typeToDeserialize != null)
                {
                    return typeToDeserialize; // found
                }
            }

            return null; // not found
        }

        private static Type GetTypeToDeserialize(string typeName, AssemblyName an)
        {
            var fullTypeName = $"{typeName}, {an.FullName}";
            var typeToDeserialize = Type.GetType(fullTypeName);
            return typeToDeserialize;
        }
    }
}
