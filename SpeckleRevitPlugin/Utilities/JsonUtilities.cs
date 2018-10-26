using System;
using Newtonsoft.Json;

namespace SpeckleRevitPlugin.Utilities
{
    public static class JsonUtilities
    {
        public static JsonSerializerSettings Settings { get; set; }

        static JsonUtilities()
        {
            Settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                CheckAdditionalContent = true,
                Formatting = Formatting.Indented
            };
        }

        public static T Deserialize<T>(string json) where T : new()
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<T>(json, Settings);
                return obj;
            }
            catch (Exception e)
            {
                throw new Exception("Failed deserializing Json: " + e.Message);
            }
        }

        public static string Serialize(object obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj, Settings);
                return json;
            }
            catch (Exception e)
            {
                throw new Exception("Failed serializing Json: " + e.Message);
            }
        }

    }
}
