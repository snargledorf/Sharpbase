using System.Collections.Generic;

namespace Sharpbase.JsonSerilization
{
    public interface IJsonNodeCollection : IEnumerable<IJsonObject>
    {
        int Count { get; }

        IJsonObject this[string key] { get; }
        IJsonObject this[int index] { get; }

        bool ContainsNode(string key);
    }
}