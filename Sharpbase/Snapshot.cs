namespace Sharpbase
{
    public class Snapshot
    {
        private readonly string json;

        private readonly IContext context;

        public Snapshot(string json, IContext context)
        {
            this.json = json;
            this.context = context;
        }

        public T Value<T>()
        {
            return context.Serializer.Deserialize<T>(json);
        }
    }
}