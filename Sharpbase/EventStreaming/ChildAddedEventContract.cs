using Sharpbase.EventStreaming;

namespace Sharpbase
{
    internal class ChildAddedEventContract : EventContract
    {
        private readonly Firebase.ChildAddedEvent @event;

        public ChildAddedEventContract(Firebase reference, Firebase.ChildAddedEvent @event)
            : base(reference, EventType.ChildAdded)
        {
            this.@event = @event;
        }

        public override void CallEvent(Snapshot snapshot)
        {
            var childAddedEventArgs = new ChildAddedEventArgs(snapshot);
            @event(childAddedEventArgs);
        }

        public override void CallEvent(FirebaseException firebaseException)
        {
            var childAddedEventArgs = new ChildAddedEventArgs(firebaseException);
            @event(childAddedEventArgs);
        }
    }
}