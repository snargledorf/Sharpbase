namespace Sharpbase
{
    public class Context : IContext
    {
        public Context()
            : this(new JsonDotNetSerializer())
        {
        }

        public Context(IJsonSerializer serializer)
        {
            Serializer = serializer;
        }

        public IJsonSerializer Serializer { get; private set; }
    }
}