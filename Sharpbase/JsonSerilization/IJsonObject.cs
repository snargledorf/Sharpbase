using System.Collections.Generic;

namespace Sharpbase.JsonSerilization
{
    public interface IJsonObject : IEnumerable<IJsonObject>
    {
        string Key { get; }

        IJsonObject Parent { get; }

        IJsonNodeCollection Children { get; }

        bool IsNull { get; }

        T Value<T>();

        object Value();

        string ToJson();
    }
}