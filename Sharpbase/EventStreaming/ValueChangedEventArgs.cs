namespace Sharpbase.EventStreaming
{
    public class ValueChangedEventArgs : ChangeEventArgs
    {
        public ValueChangedEventArgs(Snapshot snapshot)
            : base(snapshot)
        {
        }

        public ValueChangedEventArgs(FirebaseException firebaseException)
            : base(firebaseException)
        {
        }
    }
}