using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sharpbase.JsonSerilization.JsonDotNetSerializer
{
    internal class JsonDotNetNodeCollection : IJsonNodeCollection
    {
        private static readonly IJsonNode[] EmptyNodesArray = new IJsonNode[0];

        public static readonly JsonDotNetNodeCollection Empty = new JsonDotNetNodeCollection(EmptyNodesArray);

        private readonly ICollection<IJsonNode> nodes;

        public JsonDotNetNodeCollection(ICollection<IJsonNode> nodes)
        {
            this.nodes = nodes;
        }
        
        public IEnumerator<IJsonNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => nodes.Count;

        public IJsonNode this[string key] => nodes.FirstOrDefault(node => node.Key.Equals(key));

        IJsonNode IJsonNodeCollection.this[int index] => nodes.ElementAt(index);

        public bool ContainsNode(string key) => nodes.Any(node => node.Key.Equals(key));
    }
}