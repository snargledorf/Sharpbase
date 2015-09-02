using System;
using System.Threading.Tasks;

namespace Sharpbase
{
    internal interface IFirebaseClient : IDisposable
    {
        Task Remove(Path path);
        Task<Result> Push(Path path, object obj);
        Task<Result> Set(Path path, object obj);
    }
}