using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sharpbase;

namespace SharpbaseTests
{
    [TestClass]
    public class CacheTests
    {
        private ICache cache;

        [TestInitialize]
        public void Initialize()
        {
            cache = new Cache();
        }

        [TestMethod]
        public void Update()
        {
            //cache.Update();
        }
    }
}
