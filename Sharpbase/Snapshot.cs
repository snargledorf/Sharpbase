using Sharpbase.JsonSerilization;

namespace Sharpbase
{
    public class Snapshot
    {
        public Snapshot(IJsonObject jsonObject, Firebase reference)
        {
            JsonObject = jsonObject;
            Reference = reference;
        }

        public Firebase Reference { get; set; }

        public string Key => Reference.Key;

        public bool Exists => JsonObject.IsNull;

        public T Value<T>() => JsonObject.Value<T>();

        public object Value() => JsonObject.Value();

        public IJsonObject JsonObject { get; }
    }
}