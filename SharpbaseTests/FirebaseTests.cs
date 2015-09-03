using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sharpbase;
using Sharpbase.Exceptions;

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
            firebase.AuthToken = new AuthToken(TestData.Secret); // Auth as admin to ensure we can delete everything
            firebase.Remove();
            firebase = null;
        }

        [TestMethod]
        public void Child()
        {
            Firebase childRef = firebase.Child(TestData.DefaultChild);

            Assert.IsNotNull(childRef, "childRef != null");
            Assert.AreEqual(TestData.DefaultChild, childRef.Key, TestData.DefaultChild + " == childReg.Key");
        }

        [TestMethod]
        public void Push()
        {
            Firebase pushRef = firebase.Child(TestData.DefaultChild).Push();

            Assert.IsNotNull(pushRef, "pushRef != null");
            Assert.IsFalse(string.IsNullOrEmpty(pushRef.Key), "string.IsNullOrEmpty(pushRef.Key)");
        }

        [TestMethod]
        public async Task PushAsync()
        {
            Firebase pushRef = await firebase.Child(TestData.DefaultChild).PushAsync();

            Assert.IsNotNull(pushRef, "pushRef != null");
            Assert.IsFalse(string.IsNullOrEmpty(pushRef.Key), "string.IsNullOrEmpty(pushRef.Key)");
        }

        [TestMethod]
        public void PushValue()
        {
            Firebase pushRef = firebase.Child(TestData.DefaultChild).Push(user);

            Assert.IsNotNull(pushRef, "pushRef != null");
            Assert.IsFalse(string.IsNullOrEmpty(pushRef.Key), "string.IsNullOrEmpty(pushRef.Key)");
        }

        [TestMethod]
        public async Task PushAsyncValue()
        {
            Firebase pushRef = await firebase.Child(TestData.DefaultChild).PushAsync(user);

            Assert.IsNotNull(pushRef, "pushRef != null");
            Assert.IsFalse(string.IsNullOrEmpty(pushRef.Key), "string.IsNullOrEmpty(pushRef.Key)");
        }

        [TestMethod]
        public void ChildSetValue()
        {
            var reset = new ManualResetEventSlim();

            Firebase child = firebase.Child(TestData.DefaultChild);
            Error error = null;
            Firebase childRef = null;

            child.Set(user,
                (e, c) =>
                    {
                        childRef = c;
                        error = e;
                        reset.Set();
                    });

            bool timedOut = !reset.Wait(TimeSpan.FromSeconds(10));

            Assert.IsFalse(timedOut, "timedOut");
            Assert.IsNull(error, "error != null");
            Assert.IsNotNull(childRef, "childRef != null");
            Assert.IsNotNull(childRef.Key, "childRef.Key != null");
            Assert.AreEqual(TestData.DefaultChild, childRef.Key, TestData.DefaultChild + " == childRef.Key");
        }

        [TestMethod]
        [ExpectedException(typeof(AuthDeniedException))]
        public void WriteToProtected()
        {
            try
            {
                firebase.Child(TestData.ProtectedChild).Push();
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AuthDeniedException))]
        public async Task WriteToProtectedAsync()
        {
            await firebase.Child(TestData.ProtectedChild).PushAsync();
        }

        [TestMethod]
        public void AuthSecret()
        {
            firebase.AuthToken = new AuthToken(TestData.Secret);
            Firebase push = firebase.Child(TestData.ProtectedChild).Push();

            Assert.IsNotNull(push, "push != null");
            Assert.IsFalse(string.IsNullOrEmpty(push.Key), "string.IsNullOrEmpty(push.Key)");
        }

        [TestMethod]
        public void ValueChanged()
        {
            var reset = new ManualResetEventSlim();

            Firebase child = firebase.Child(TestData.DefaultChild);

            child.Set(true);

            Snapshot snap = null;
            child.ValueChanged += snapshot =>
            {
                snap = snapshot;
                reset.Set();
            };

            bool timedOut = !reset.Wait(TimeSpan.FromSeconds(10));

            Assert.IsFalse(timedOut, "timedOut");
            Assert.IsNotNull(snap, "snap != null");
            Assert.IsTrue(snap.Value<bool>(), "snap.Value<bool>()");
        }
    }
}
