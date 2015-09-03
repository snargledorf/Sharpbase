namespace Sharpbase
{
    public interface IContext
    {
        AuthToken AuthToken { get; set; }
        IJsonSerializer Serializer { get; }
    }
}