using System;
using System.Threading.Tasks;

using Sharpbase.EventStreaming;

namespace Sharpbase
{
    internal interface IFirebaseClient : IDisposable
    {
        IContext Context { get; }

        Task<Result> Remove(Firebase reference);
        Task<Result> Set(Firebase reference, object content = null);
        Task<Result> Push(Firebase reference, object content = null);
        Task<SnapshotResult> Get(Firebase reference);

        void AddEventListener(IEventContract contract);
        void RemoveEventListener(IEventContract contract);
    }
}