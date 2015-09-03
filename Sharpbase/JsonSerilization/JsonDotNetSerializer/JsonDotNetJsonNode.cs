using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace Sharpbase.JsonSerilization.JsonDotNetSerializer
{
    internal class JsonDotNetJsonNode : IJsonNode
    {
        private readonly JToken token;

        public JsonDotNetJsonNode(JToken token, IJsonNode parent = null)
        {
            this.token = token;
            Parent = parent;
            Properties = CreateJsonNodeCollection(token);
        }

        private IJsonNodeCollection CreateJsonNodeCollection(JToken token)
        {
            var jsonNodeCollection = new JsonNodeCollection();
            foreach (JToken subToken in token.Children())
            {
                var node = new JsonDotNetJsonNode(subToken, this);
                jsonNodeCollection.Add(node);
            }
            return jsonNodeCollection;
        }

        public IJsonNode Property(string name)
        {
            return Properties[name];
        }

        public bool HasValue => !token.HasValues;

        public IJsonNode Parent { get; }

        public IJsonNodeCollection Properties { get; }

        public string Name => token.Path;

        public T Value<T>() => token.Value<T>();
    }

    internal class JsonNodeCollection : IJsonNodeCollection
    {
        private readonly Dictionary<string, IJsonNode> nodes = new Dictionary<string, IJsonNode>();

        public IEnumerator<IJsonNode> GetEnumerator() => nodes.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => nodes.Count;

        public IJsonNode this[string name] => GetNode(name);

        public IJsonNode GetNode(string name) => nodes[name];

        public bool ContainsNode(string name) => nodes.ContainsKey(name);

        public void Add(IJsonNode node) => nodes.Add(node.Name, node);
    }
}