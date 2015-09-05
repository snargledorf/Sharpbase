using System.Collections.Generic;

namespace Sharpbase.JsonSerilization
{
    public interface IJsonNodeCollection : IEnumerable<IJsonNode>
    {
        int Count { get; }

        IJsonNode this[string key] { get; }
        IJsonNode this[int index] { get; }

        bool ContainsNode(string key);
    }
}