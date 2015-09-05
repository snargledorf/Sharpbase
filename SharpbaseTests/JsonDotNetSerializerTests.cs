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
            IJsonNode tree = serializer.Deserialize("{name:\"User 0\"}");

            IJsonNodeCollection children = tree.Children;
            IJsonNode nameNode = children["name"];
            var name = nameNode.Value<string>();

            Assert.AreEqual("User 0", name);
        }

        [TestMethod]
        public void Array()
        {
            const string Json = "{ objects: [ 1, 3, { name: \"User 0\" } ] }";
            IJsonNode tree = serializer.Deserialize(Json);

            IJsonNode objectsNode = tree.Children["objects"];

            var firstObject = objectsNode.Children[0].Value<int>();
            var secondObject = objectsNode.Children[1].Value<int>();
            string thirdObject = objectsNode.Children[2].Value<NameObject>().Name;

            Assert.AreEqual(1, firstObject);
            Assert.AreEqual(3, secondObject);
            Assert.AreEqual("User 0", thirdObject);
        }

        [TestMethod]
        public void ToJson()
        {
            const string Json = "{ objects: [ 1, 3, { name: \"User 0\" } ] }";
            IJsonNode tree = serializer.Deserialize(Json);
            string json = tree.ToJson();

            Assert.AreEqual(Json, json);
        }
    }

    public class NameObject
    {
        public string Name { get; set; }
    }
}
