using System.Collections.Generic;

namespace Sharpbase.JsonSerilization
{
    public interface IJsonNodeCollection : IEnumerable<IJsonNode>
    {
        int Count { get; }

        IJsonNode this[string name] { get; }

        IJsonNode GetNode(string name);

        bool ContainsNode(string name);
    }
}