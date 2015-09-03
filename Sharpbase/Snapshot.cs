using Sharpbase.JsonSerilization;

namespace Sharpbase
{
    public class Snapshot
    {
        private readonly string json;

        private readonly IContext context;

        public Snapshot(string json, IContext context, Firebase reference)
        {
            Reference = reference;
            this.json = json;
            this.context = context;
        }

        public Firebase Reference { get; set; }

        public string Key => Reference.Key;

        public bool Exists => !string.IsNullOrEmpty(json);

        public T Value<T>()
        {
            return Exists ? context.Serializer.Deserialize<T>(json) : default(T);
        }

        public IJsonNode Value()
        {
            return Exists ? context.Serializer.Deserialize(json) : null;
        }
    }
}