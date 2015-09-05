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
        public void PushCallback()
        {
            var reset = new ManualResetEventSlim();

            Firebase child = firebase.Child(TestData.DefaultChild);
            FirebaseException callbackError = null;
            Firebase callbackRef = null;

            child.Push((error, reference) =>
                {
                    callbackRef = reference;
                    callbackError = error;
                    reset.Set();
                });

            bool timedOut = !reset.Wait(TimeSpan.FromSeconds(5));

            Assert.IsFalse(timedOut, "timedOut");
            Assert.IsNull(callbackError);
            Assert.IsNotNull(callbackRef);
            Assert.IsFalse(string.IsNullOrEmpty(callbackRef.Key));
            Assert.AreNotEqual(child, callbackRef);
        }

        [TestMethod]
        public async Task PushAsync()
        {
            Result result = await firebase.Child(TestData.DefaultChild).PushAsync();

            Assert.IsNotNull(result, "result != null");
            Assert.IsNotNull(result.Reference, "result.Reference != null");
            Assert.IsFalse(string.IsNullOrEmpty(result.Reference.Key), "string.IsNullOrEmpty(pushRef.Key)");
        }

        [TestMethod]
        public void PushValue()
        {
            Firebase pushRef = firebase.Child(TestData.DefaultChild).Push(user);

            Assert.IsNotNull(pushRef, "pushRef != null");
            Assert.IsFalse(string.IsNullOrEmpty(pushRef.Key), "string.IsNullOrEmpty(pushRef.Key)");
        }

        [TestMethod]
        public void PushValueCallback()
        {
            var reset = new ManualResetEventSlim();

            Firebase child = firebase.Child(TestData.DefaultChild);
            FirebaseException callbackError = null;
            Firebase callbackRef = null;

            child.Push(user, (error, reference) =>
            {
                callbackRef = reference;
                callbackError = error;
                reset.Set();
            });

            bool timedOut = !reset.Wait(TimeSpan.FromSeconds(5));

            Assert.IsFalse(timedOut, "timedOut");
            Assert.IsNull(callbackError);
            Assert.IsNotNull(callbackRef);
            Assert.IsFalse(string.IsNullOrEmpty(callbackRef.Key));
            Assert.AreNotEqual(child, callbackRef);
        }

        [TestMethod]
        public async Task PushValueAsync()
        {
            Result result = await firebase.Child(TestData.DefaultChild).PushAsync(user);

            Assert.IsNotNull(result, "result = null");
            Assert.IsNotNull(result.Reference, "result.Reference != null");
            Assert.IsFalse(string.IsNullOrEmpty(result.Reference.Key), "string.IsNullOrEmpty(pushRef.Reference.Key)");
        }

        [TestMethod]
        public void ChildSetValue()
        {
            var reset = new ManualResetEventSlim();

            Firebase child = firebase.Child(TestData.DefaultChild);
            FirebaseException error = null;
            Firebase childRef = null;

            child.Set(user,
                (e, c) =>
                    {
                        childRef = c;
                        error = e;
                        reset.Set();
                    });

            bool timedOut = !reset.Wait(TimeSpan.FromSeconds(5));

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
            firebase.Child(TestData.ProtectedChild).Push();
        }

        [TestMethod]
        public async Task WriteToProtectedAsync()
        {
            Result result = await firebase.Child(TestData.ProtectedChild).PushAsync();

            Assert.IsNotNull(result.Error, "result.Error != null");
            Assert.IsInstanceOfType(result.Error, typeof(AuthDeniedException), "result.Error.GetType() == typeof(AuthDeniedException)");
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
        public void GetValue()
        {
            Firebase child = firebase.Child(TestData.DefaultChild);
            child.Set(true);

            Snapshot snapshot = child.Get();
            Firebase reference = snapshot.Reference;
            var value = snapshot.Value<bool>();

            Assert.AreEqual(child, reference);
            Assert.IsTrue(value);
        }

        [TestMethod]
        public void GetObjectValue()
        {
            Firebase child = firebase.Child(TestData.DefaultChild);
            child.Set(true);

            Snapshot snapshot = child.Get();
            Firebase reference = snapshot.Reference;
            object value = snapshot.Value();

            Assert.AreEqual(child, reference);
            Assert.AreEqual(true, value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNullValueNonNullableType()
        {
            firebase
                .Child(TestData.DefaultChild)
                .Get()
                .Value<bool>();
        }

        [TestMethod]
        public void GetNullValueNullableType()
        {
            Firebase child = firebase.Child(TestData.DefaultChild);

            Snapshot snapshot = child.Get();
            Firebase reference = snapshot.Reference;
            var value = snapshot.Value<bool?>();

            Assert.AreEqual(child, reference);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void GetNullObjectValue()
        {
            Firebase child = firebase.Child(TestData.DefaultChild);

            Snapshot snapshot = child.Get();
            Firebase reference = snapshot.Reference;
            object value = snapshot.Value();

            Assert.AreEqual(child, reference);
            Assert.IsNull(value);
        }
    }
}
