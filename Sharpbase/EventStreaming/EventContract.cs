namespace Sharpbase.EventStreaming
{
    internal abstract class EventContract : IEventContract
    {
        protected EventContract(Firebase reference, EventType supportedEventType)
        {
            Reference = reference;
            SupportedEventType = supportedEventType;
        }

        public Firebase Reference { get; }

        public EventType SupportedEventType { get; }

        public bool SupportsEventType(EventType eventType)
        {
            return eventType == SupportedEventType;
        }

        public abstract void CallEvent(Snapshot snapshot);

        public abstract void CallEvent(FirebaseException firebaseException);

        public bool Equals(IEventContract other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Reference.Equals(other.Reference);
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
            return Equals((IEventContract)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Reference.GetHashCode();
            }
        }
    }
}