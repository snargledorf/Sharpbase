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
            const string Name = "User 0";
            const string Json = "{name:\"User 0\" }";

            var nameHolder = serializer.Deserialize<NameHolder>(Json);

            Assert.IsNotNull(nameHolder);
            Assert.AreEqual(Name, nameHolder.Name);
        }

        [TestMethod]
        public void DeserializeKnownTypeAnonymously()
        {
            const string Name = "User 0";
            const string Json = "{name:\"User 0\" }";

            IJsonNode tree = serializer.Deserialize(Json);

            IJsonNode nameNode = tree.Property("name");
            string name = nameNode.Value<string>();

            Assert.IsNotNull(tree);
            Assert.IsTrue(nameNode.HasValue);
            Assert.AreEqual(Name, name);
        }

        [TestMethod]
        public void DeserializeUnknownType()
        {
            const string Name = "User 0";
            const int Age = 25;
            const string Json = "{name:\"User 0\", age: 25 }";

            IJsonNode tree = serializer.Deserialize(Json);

            IJsonNode nameNode = tree.Property("name");
            var name = nameNode.Value<string>();

            IJsonNode ageNode = tree.Property("age");
            var age = ageNode.Value<int>();
            
            Assert.IsTrue(nameNode.HasValue);
            Assert.AreEqual(Name, name);
            Assert.IsTrue(ageNode.HasValue);
            Assert.AreEqual(Age, age);
        }

        [TestMethod]
        public void NodeParent()
        {
            const string Json = "{name:\"User 0\"}";

            IJsonNode tree = serializer.Deserialize(Json);
            IJsonNode nameNode = tree.Property("name");
            
            Assert.AreEqual(tree, nameNode.Parent);
        }

        [TestMethod]
        public void Children()
        {
            const string Json = "{name:\"User 0\"}";

            IJsonNode tree = serializer.Deserialize(Json);

            IJsonNodeCollection properties = tree.Properties;

            Assert.IsNotNull(properties);
            Assert.AreEqual(1, properties.Count);
        }
    }

    public class NameHolder
    {
        public string Name { get; set; }
    }
}
