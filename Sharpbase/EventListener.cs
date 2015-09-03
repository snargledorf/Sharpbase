using System;

namespace Sharpbase
{
    internal class EventListener
    {
        public event Action<Snapshot> ValueChanged;
    }
}