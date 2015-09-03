﻿using Newtonsoft.Json;

namespace Sharpbase
{
    internal class JsonDotNetSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public object Deserilize(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
