using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sharpbase.JsonSerilization;
using Sharpbase.JsonSerilization.JsonDotNetSerializer;

namespace SharpbaseTests
{
    [TestClass]
    public class JsonDotNetSerializerTests
    {
        private JsonDotNetSerializer serializer;

        [TestInitialize]
        public void Initialize()
        {
            serializer = new JsonDotNetSerializer();
        }

        [TestMethod]
        public void DeserializeKnownType()
        {
            var nameHolder = serializer.Deserialize<NameObject>("{name:\"User 0\" }");

            Assert.IsNotNull(nameHolder);
            Assert.AreEqual("User 0", nameHolder.Name);
        }

        [TestMethod]
        public void Children()
        {
            IJsonObject tree = serializer.Deserialize("{name:\"User 0\"}");

            IJsonNodeCollection children = tree.Children;
            IJsonObject nameObject = children["name"];
            var name = nameObject.Value<string>();

            Assert.AreEqual("User 0", name);
        }

        [TestMethod]
        public void Array()
        {
            const string Json = "{ objects: [ 1, 3, { name: \"User 0\" } ] }";
            IJsonObject tree = serializer.Deserialize(Json);

            IJsonObject objectsObject = tree.Children["objects"];

            var firstObject = objectsObject.Children[0].Value<int>();
            var secondObject = objectsObject.Children[1].Value<int>();
            string thirdObject = objectsObject.Children[2].Value<NameObject>().Name;

            Assert.AreEqual(1, firstObject);
            Assert.AreEqual(3, secondObject);
            Assert.AreEqual("User 0", thirdObject);
        }

        [TestMethod]
        public void ToJson()
        {
            const string Json = "{ objects: [ 1, 3, { name: \"User 0\" } ] }";
            IJsonObject tree = serializer.Deserialize(Json);
            string json = tree.ToJson();

            Assert.AreEqual(Json, json);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyJson()
        {
            serializer.Deserialize(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullJson()
        {
            serializer.Deserialize(null);
        }

        [TestMethod]
        public void NullValue()
        {
            object value = serializer.Deserialize("null").Value();

            Assert.IsNull(value);
        }
    }
}
