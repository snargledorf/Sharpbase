using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sharpbase.JsonSerilization.JsonDotNetSerializer
{
    internal class JsonDotNetNodeCollection : IJsonNodeCollection
    {
        private static readonly IJsonObject[] EmptyObjectsArray = new IJsonObject[0];

        public static readonly JsonDotNetNodeCollection Empty = new JsonDotNetNodeCollection(EmptyObjectsArray);

        private readonly ICollection<IJsonObject> nodes;

        public JsonDotNetNodeCollection(ICollection<IJsonObject> nodes)
        {
            this.nodes = nodes;
        }
        
        public IEnumerator<IJsonObject> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => nodes.Count;

        public IJsonObject this[string key] => nodes.FirstOrDefault(node => node.Key.Equals(key));

        IJsonObject IJsonNodeCollection.this[int index] => nodes.ElementAt(index);

        public bool ContainsNode(string key) => nodes.Any(node => node.Key.Equals(key));
    }
}