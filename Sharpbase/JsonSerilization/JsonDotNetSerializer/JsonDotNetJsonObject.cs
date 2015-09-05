using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Sharpbase.JsonSerilization.JsonDotNetSerializer
{
    internal class JsonDotNetJsonObject : IJsonObject
    {
        private readonly JToken value;
        
        private JsonDotNetJsonObject(JToken value, string key = null)
            : this(JsonDotNetNodeCollection.Empty, value, key)
        {
        }

        private JsonDotNetJsonObject(IJsonNodeCollection children, JToken value = null, string key = null)
        {
            Children = children;
            this.value = value;
            Key = key;

            SetChildrenParent();
        }

        public string Key { get; }
        
        public IJsonObject Parent { get; private set; }

        public IJsonNodeCollection Children { get; }

        public bool IsNull => value != null && value.Type == JTokenType.Null;

        public static JsonDotNetJsonObject Parse(string json)
        {
            ArgUtils.CheckForNull(json, nameof(json));

            if (string.IsNullOrEmpty(json))
                throw new ArgumentException($"Invalid json string: {json}", nameof(json));

            JToken token = JToken.Parse(json);
            return ParseToken(token);
        }

        private static JsonDotNetJsonObject ParseToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    IJsonNodeCollection children = ParseChildren(token);
                    return new JsonDotNetJsonObject(children, token);
                case JTokenType.Array:
                    IJsonNodeCollection items = ParseArrayItems(token);
                    return new JsonDotNetJsonObject(items);
            }

            return new JsonDotNetJsonObject(token);
        }

        private static IJsonNodeCollection ParseChildren(JToken parent)
        {
            IJsonObject[] objects = parent.Cast<KeyValuePair<string, JToken>>().Select(ParseToken).ToArray();
            return new JsonDotNetNodeCollection(objects);
        }

        private static IJsonObject ParseToken(KeyValuePair<string, JToken> token)
        {
            string key = token.Key;
            switch (token.Value.Type)
            {
                case JTokenType.Object:
                    IJsonNodeCollection children = ParseChildren(token.Value);
                    return new JsonDotNetJsonObject(children, token.Value, key);
                case JTokenType.Array:
                    IJsonNodeCollection items = ParseArrayItems(token.Value);
                    return new JsonDotNetJsonObject(items, key: key);
            }

            return new JsonDotNetJsonObject(token.Value, key);
        }

        private static IJsonNodeCollection ParseArrayItems(JToken token)
        {
            JsonDotNetJsonObject[] items = token.Select(ParseToken).ToArray();
            return new JsonDotNetNodeCollection(items);
        }

        public T Value<T>() => value.ToObject<T>();

        public object Value() => value.ToObject<object>();

        public IEnumerator<IJsonObject> GetEnumerator()
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
            foreach (IJsonObject jsonNode in Children)
            {
                var child = (JsonDotNetJsonObject)jsonNode;
                child.Parent = this;
            }
        }
    }
}