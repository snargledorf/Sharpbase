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

        public IJsonNode Deserialize(string json)
        {
            var o = (JObject)JsonConvert.DeserializeObject(json);
            return new JsonDotNetJsonNode(o);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
