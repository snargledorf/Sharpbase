using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Sharpbase.JsonSerilization.JsonDotNetSerializer
{
    internal class JsonDotNetJsonNode : IJsonNode
    {
        private readonly JToken value;
        
        private JsonDotNetJsonNode(JToken value, string key = null)
            : this(JsonDotNetNodeCollection.Empty, value, key)
        {
        }

        private JsonDotNetJsonNode(IJsonNodeCollection children, JToken value = null, string key = null)
        {
            Children = children;
            this.value = value;
            Key = key;

            SetChildrenParent();
        }

        public string Key { get; }
        
        public IJsonNode Parent { get; private set; }

        public IJsonNodeCollection Children { get; }

        public static JsonDotNetJsonNode Parse(string json)
        {
            JObject obj = JObject.Parse(json);
            return ParseToken(obj);
        }

        private static JsonDotNetJsonNode ParseToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    IJsonNodeCollection children = ParseChildren(token);
                    return new JsonDotNetJsonNode(children, token);
                case JTokenType.Array:
                    IJsonNodeCollection items = ParseArrayItems(token);
                    return new JsonDotNetJsonNode(items);
            }

            return new JsonDotNetJsonNode(token);
        }

        private static IJsonNodeCollection ParseChildren(JToken parent)
        {
            IJsonNode[] nodes = parent.Cast<KeyValuePair<string, JToken>>().Select(ParseToken).ToArray();
            return new JsonDotNetNodeCollection(nodes);
        }

        private static IJsonNode ParseToken(KeyValuePair<string, JToken> token)
        {
            string key = token.Key;
            switch (token.Value.Type)
            {
                case JTokenType.Object:
                    IJsonNodeCollection children = ParseChildren(token.Value);
                    return new JsonDotNetJsonNode(children, token.Value, key);
                case JTokenType.Array:
                    IJsonNodeCollection items = ParseArrayItems(token.Value);
                    return new JsonDotNetJsonNode(items, key: key);
            }

            return new JsonDotNetJsonNode(token.Value, key);
        }

        private static IJsonNodeCollection ParseArrayItems(JToken token)
        {
            JsonDotNetJsonNode[] items = token.Select(ParseToken).ToArray();
            return new JsonDotNetNodeCollection(items);
        }

        public T Value<T>() => value.ToObject<T>();

        public IEnumerator<IJsonNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return ToJson();
        }

        public string ToJson()
        {
            string json = ValueToJson();

            if (string.IsNullOrEmpty(Key))
                return json;

            return $"{Key}: {json}";
        }

        private string ValueToJson()
        {
            if (Children.Count != 0)
            {
                if (value != null)
                    return CreateObjectJsonString();

                return CreateArrayJsonString();
            }

            Debug.Assert(value != null);

            switch (value.Type)
            {
                case JTokenType.String:
                    return $"\"{value}\"";
            }

            return value.ToString();
        }

        private string CreateArrayJsonString()
        {
            return $"[ {string.Join(", ", Children)} ]";
        }

        private string CreateObjectJsonString()
        {
            return $"{{ {string.Join(", ", Children)} }}";
        }

        private void SetChildrenParent()
        {
            foreach (IJsonNode jsonNode in Children)
            {
                var child = (JsonDotNetJsonNode)jsonNode;
                child.Parent = this;
            }
        }
    }
}