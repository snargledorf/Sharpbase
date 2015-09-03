using System;
using System.Threading.Tasks;

namespace Sharpbase
{
    public partial class Firebase
    {
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
            Path = path;
        }

        public string Key => Path.LastSegment;

        internal Path Path { get; }

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
        
        public event Action<Snapshot> ValueChanged
        {
            add
            {
                client.AddValueChangedListener(Path, value);
            }
            remove
            {
                client.RemoveEventListener(Path, value);
            }
        }

        public void Set(object obj, Action<Error, Firebase> complete = null)
        {
            Result result = SetAsync(obj).Result;
            complete?.Invoke(result.Error, this);
        }

        public Task<Result> SetAsync(object obj)
        {
            return client.Set(Path, obj, AuthToken);
        }

        public void Remove(Action complete = null)
        {
            RemoveAsync().Wait();
            complete?.Invoke();
        }

        public Task RemoveAsync()
        {
            return client.Remove(Path, AuthToken);
        }

        public Firebase Push(object obj = null)
        {
            return PushAsync(obj).Result;
        }

        public async Task<Firebase> PushAsync(object obj = null)
        {
            Result result = await client.Push(Path, obj, AuthToken);
            if (!result.Success)
                throw result.Error.Exception;

            var pushResult = result.Snapshot.Value<PushResult>();

            return Child(pushResult.Name);
        }

        public Firebase Child(string childPath)
        {
            return new Firebase(this, Path.Child(new Path(childPath)));
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
                c?.Dispose();
            }

            disposed = true;
        }
    }
}
