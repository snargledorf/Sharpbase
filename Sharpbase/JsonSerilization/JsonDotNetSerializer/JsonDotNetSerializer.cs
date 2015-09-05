using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sharpbase.JsonSerilization.JsonDotNetSerializer
{
    internal class JsonDotNetSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public IJsonObject Deserialize(string json)
        {
            return JsonDotNetJsonObject.Parse(json);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
