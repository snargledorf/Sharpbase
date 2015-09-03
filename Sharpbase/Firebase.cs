using System;
using System.Threading.Tasks;

namespace Sharpbase
{
    public partial class Firebase
    {
        private readonly Path path;

        private readonly IContext context;

        private IFirebaseClient client;

        public Firebase(string firebaseUrl)
            : this (firebaseUrl, new Context())
        {
        }

        public Firebase(string firebaseUrl, IContext context)
            : this(new FirebaseClient(firebaseUrl, context), context, new Path())
        {
        }

        private Firebase(Firebase firebase, Path child)
            : this(firebase.client, firebase.context, child)
        {
        }

        private Firebase(IFirebaseClient client, IContext context, Path path)
        {
            this.client = client;
            this.context = context;
            this.path = path;
        }

        public string Key
        {
            get
            {
                return path.LastSegment;
            }
        }

        public AuthToken AuthToken
        {
            get
            {
                return context.AuthToken;
            }

            set
            {
                context.AuthToken = value;
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
            return client.Set(path, obj, AuthToken);
        }

        public void Remove(Action complete = null)
        {
            RemoveAsync().Wait();
            if (complete != null)
                complete();
        }

        public Task RemoveAsync()
        {
            return client.Remove(path, AuthToken);
        }

        public Firebase Push(object obj = null)
        {
            return PushAsync(obj).Result;
        }

        public async Task<Firebase> PushAsync(object obj = null)
        {
            Result result = await client.Push(path, obj, AuthToken);
            if (!result.Success)
                throw result.Error.Exception;

            var pushResult = result.Snapshot.Value<PushResult>();

            return Child(pushResult.Name);
        }

        public Firebase Child(string childPath)
        {
            return new Firebase(this, path.Child(new Path(childPath)));
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
