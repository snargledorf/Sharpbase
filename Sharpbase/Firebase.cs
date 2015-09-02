﻿using System;
using System.Threading.Tasks;

namespace Sharpbase
{
    public partial class Firebase
    {
        private static readonly IContext DefaultContext = new Context(new JsonDotNetSerializer());
        
        private readonly Path path;

        private IFirebaseClient client;

        public Firebase(string firebaseUrl)
            : this (firebaseUrl, DefaultContext)
        {
        }

        public Firebase(string firebaseUrl, IContext context)
            : this(new FirebaseClient(firebaseUrl, context))
        {
        }

        internal Firebase(IFirebaseClient client)
            : this(client, new Path())
        {
        }

        internal Firebase(IFirebaseClient client, Path path)
        {
            this.client = client;
            this.path = path;
        }

        public string Key
        {
            get
            {
                return path.LastSegment;
            }
        }

        public void Set(object obj, Action<Error, Firebase> complete = null)
        {
            Result result = SetAsync(obj).Result;
            if (complete != null)
                complete(result.Error, this);
        }

        public Task<Result> SetAsync(object obj)
        {
            return client.Set(path, obj);
        }

        public void Remove(Action complete = null)
        {
            RemoveAsync().Wait();
            if (complete != null)
                complete();
        }

        public Task RemoveAsync()
        {
            return client.Remove(path);
        }

        public Firebase Push(object obj = null)
        {
            return PushAsync(obj).Result;
        }

        public async Task<Firebase> PushAsync(object obj = null)
        {
            Result result = await client.Push(path, obj);
            if (!result.Success)
                throw result.Error.Exception;

            var pushResult = result.Snapshot.Value<PushResult>();

            return Child(pushResult.Name);
        }

        public Firebase Child(string childPath)
        {
            return new Firebase(client, path.Child(new Path(childPath)));
        }

        public void AuthWithCustomToken(string token, Action<Error, AuthData> complete = null)
        {

        }
    }

    public partial class Firebase : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                IFirebaseClient c = client;
                client = null;
                if (c != null) c.Dispose();
            }

            disposed = true;
        }
    }
}
