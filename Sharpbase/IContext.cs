namespace Sharpbase
{
    public interface IContext
    {
        IJsonSerializer Serializer { get; }
    }
}