namespace Sharpbase.JsonSerilization
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
        IJsonObject Deserialize(string json);
        string Serialize(object obj);
    }
}