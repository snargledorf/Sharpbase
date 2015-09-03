using System;

namespace Sharpbase.EventStreaming
{
    internal interface IEventContract : IEquatable<IEventContract>
    {
        EventType SupportedEventType { get; }

        Firebase Reference { get; }

        bool SupportsEventType(EventType eventType);

        void CallEvent(Snapshot snapshot);
        void CallEvent(FirebaseException firebaseException);
    }
}