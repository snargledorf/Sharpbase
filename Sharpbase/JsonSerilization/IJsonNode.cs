using System.Collections.Generic;

namespace Sharpbase.JsonSerilization
{
    public interface IJsonNode : IEnumerable<IJsonNode>
    {
        string Key { get; }

        IJsonNode Parent { get; }

        IJsonNodeCollection Children { get; }

        T Value<T>();

        string ToJson();
    }
}