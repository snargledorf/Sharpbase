namespace Sharpbase
{
    public class Context : IContext
    {
        public Context()
        {
            Serializer = new JsonDotNetSerializer();
        }

        public AuthToken AuthToken { get; set; }

        public IJsonSerializer Serializer { get; set; }
    }
}