using System;
using System.Threading.Tasks;

namespace Sharpbase
{
    internal interface IFirebaseClient : IDisposable
    {
        Task Remove(Path path, AuthToken authToken);
        Task<Result> Push(Path path, object obj, AuthToken authToken);
        Task<Result> Set(Path path, object obj, AuthToken authToken);

        void AddValueChangedListener(Path path, Action<Snapshot> listener);
        void RemoveEventListener(Path path, Action<Snapshot> listener);
    }
}