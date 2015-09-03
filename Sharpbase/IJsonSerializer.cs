namespace Sharpbase
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
        object Deserilize(string json);
        string Serialize(object obj);
    }
}