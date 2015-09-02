using System;
using System.Diagnostics;

using Sharpbase;

namespace SharpbaseConsoleTests
{
    class Program
    {
        private const string TestInstance = "https://sharpbase.firebaseio.com/";

        private static Firebase firebase;

        private static Action[] tests = { Push };

        static void Main(string[] args)
        {
            foreach (Action test in tests)
            {
                Setup();
                test();
                Cleanup();
            }
        }

        private static void Setup()
        {
            firebase = new Firebase(TestInstance);
            firebase.Remove();
        }

        private static void Cleanup()
        {
            firebase.Remove();
        }

        private static void Push()
        {
            Firebase pushRef = firebase.Push();

            Debug.Assert(pushRef != null);
            Debug.Assert(string.IsNullOrEmpty(pushRef.Key));
        }
    }
}
