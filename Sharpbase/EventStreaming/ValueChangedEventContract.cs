namespace Sharpbase.EventStreaming
{
    internal class ValueChangedEventContract : EventContract
    {
        private readonly Firebase.ValueChangedEvent @event;

        public ValueChangedEventContract(Firebase reference, Firebase.ValueChangedEvent @event)
            : base(reference, EventType.ValueChanged)
        {
            this.@event = @event;
        }

        public override void CallEvent(Snapshot snapshot)
        {
            var valueChangedEventArgs = new ValueChangedEventArgs(snapshot);
            @event(valueChangedEventArgs);
        }

        public override void CallEvent(FirebaseException firebaseException)
        {
            var valueChangedEventArgs = new ValueChangedEventArgs(firebaseException);
            @event(valueChangedEventArgs);
        }
    }
}