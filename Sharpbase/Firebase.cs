using System;
using System.Threading.Tasks;

using Sharpbase.EventStreaming;

namespace Sharpbase
{
    public partial class Firebase
    {
        private IFirebaseClient client;

        public Firebase(string firebaseUrl)
            : this (firebaseUrl, new Context())
        {
        }

        public Firebase(string firebaseUrl, IContext context)
            : this(FirebaseUrl.Parse(firebaseUrl), context)
        {
        }

        internal Firebase(FirebaseUrl firebaseUrl, IContext context)
            : this(new FirebaseClient(firebaseUrl.BaseUrl, context), firebaseUrl.Path)
        {
        }

        internal Firebase(IFirebaseClient client, Path path)
        {
            this.client = client;
            Path = path;
        }

        public delegate void ValueChangedEvent(ValueChangedEventArgs snapshot);
        public delegate void ChildAddedEvent(ChildAddedEventArgs snapshot);
        public delegate void CompleteAction(FirebaseException firebaseException, Firebase reference);

        public string Key => Path.LastSegment;

        internal Path Path { get; }

        public AuthToken AuthToken
        {
            get
            {
                return client.Context.AuthToken;
            }

            set
            {
                client.Context.AuthToken = value;
            }
        }
        
        public event ValueChangedEvent ValueChanged
        {
            add
            {
                var contract = new ValueChangedEventContract(this, value);
                client.AddEventListener(contract);
            }

            remove
            {
                var contract = new ValueChangedEventContract(this, value);
                client.RemoveEventListener(contract);
            }
        }

        public event ChildAddedEvent ChildAdded
        {
            add
            {
                var contract = new ChildAddedEventContract(this, value);
                client.AddEventListener(contract);
            }

            remove
            {
                var contract = new ChildAddedEventContract(this, value);
                client.RemoveEventListener(contract);
            }
        }

        public void Set(object obj, CompleteAction complete = null)
        {
            Result result = SetAsync(obj).Result;
            complete?.Invoke(result.Error, result.Reference);
        }

        public Task<Result> SetAsync(object content)
        {
            return client.Set(this, content);
        }

        public void Remove(CompleteAction complete = null)
        {
            Result result = RemoveAsync().Result;
            complete?.Invoke(result.Error, result.Reference);
        }

        public Task<Result> RemoveAsync()
        {
            return client.Remove(this);
        }

        public Firebase Push(object content = null)
        {
            return PushAsync(content).Result;
        }

        public async Task<Firebase> PushAsync(object content = null)
        {
            return await client.Push(this, content);
        }

        public Firebase Child(string childPath)
        {
            return new Firebase(client, Path.Child(new Path(childPath)));
        }
    }

    // Equality implementation
    public partial class Firebase : IEquatable<Firebase>
    {

        public bool Equals(Firebase other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return disposed == other.disposed && Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Firebase)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = disposed.GetHashCode();
                hashCode = (hashCode * 397) ^ (Path?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(Firebase left, Firebase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Firebase left, Firebase right)
        {
            return !Equals(left, right);
        }
    }

    // IDisposable implementation
    public partial class Firebase : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                IDisposable c = client;
                client = null;
                c?.Dispose();
            }

            disposed = true;
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(Firebase));
        }
    }
}
