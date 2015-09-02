using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sharpbase;

namespace SharpbaseTests
{
    [TestClass]
    public class FirebaseTests
    {

        private Firebase firebase;

        private User user;

        [TestInitialize]
        public void Initialize()
        {
            user = new User { FirstName = "Ryan", LastName = "Esteves" };

            firebase = new Firebase(TestData.IntanceUrl);
        }

        [TestCleanup]
        public void Cleanup()
        {
            user = null;

            // Remove all data from firebase
            firebase.Remove();
            firebase = null;
        }

        [TestMethod]
        public void Push()
        {
            Firebase pushRef = firebase.Push();

            Assert.IsNotNull(pushRef);
            Assert.IsFalse(string.IsNullOrEmpty(pushRef.Key));
        }

        [TestMethod]
        public async Task PushAsync()
        {
            Firebase pushRef = await firebase.PushAsync();

            Assert.IsNotNull(pushRef);
            Assert.IsFalse(string.IsNullOrEmpty(pushRef.Key));
        }

        [TestMethod]
        public void Child()
        {
            Firebase childRef = firebase.Child(TestData.ChildKey);

            Assert.IsNotNull(childRef);
            Assert.AreEqual(TestData.ChildKey, childRef.Key);
        }

        [TestMethod]
        public void PushValue()
        {
            Firebase pushRef = firebase.Push(user);

            Assert.IsNotNull(pushRef);
            Assert.IsFalse(string.IsNullOrEmpty(pushRef.Key));
        }

        [TestMethod]
        public async Task PushAsyncValue()
        {
            Firebase pushRef = await firebase.PushAsync(user);

            Assert.IsNotNull(pushRef);
            Assert.IsFalse(string.IsNullOrEmpty(pushRef.Key));
        }

        [TestMethod]
        public void ChildSetValue()
        {
            var reset = new ManualResetEventSlim();

            Firebase child = firebase.Child(TestData.ChildKey);
            Error error = null;
            Firebase childRef = null;

            child.Set(user,
                (e, c) =>
                    {
                        childRef = c;
                        error = e;
                        reset.Set();
                    });

            reset.Wait();

            Assert.IsNull(error);
            Assert.IsNotNull(childRef);
            Assert.IsNotNull(childRef.Key);
            Assert.AreEqual(TestData.ChildKey, childRef.Key);
        }

        [TestMethod]
        public void AuthSecret()
        {
            firebase.AuthWithCustomToken(TestData.Secret,
                (e, a) =>
                    {
                        
                    });
        }
    }
}
