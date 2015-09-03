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

        Task<Firebase> Push(Firebase reference, object content = null);

        void AddEventListener(IEventContract contract);
        void RemoveEventListener(IEventContract contract);
    }
}