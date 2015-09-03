namespace Sharpbase.JsonSerilization
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
        IJsonNode Deserialize(string json);
        string Serialize(object obj);
    }
}