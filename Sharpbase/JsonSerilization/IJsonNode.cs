namespace Sharpbase.JsonSerilization
{
    public interface IJsonNode
    {
        string Name { get; }

        IJsonNode Property(string name);

        bool HasValue { get; }

        IJsonNode Parent { get; }

        IJsonNodeCollection Properties { get; }

        T Value<T>();
    }
}