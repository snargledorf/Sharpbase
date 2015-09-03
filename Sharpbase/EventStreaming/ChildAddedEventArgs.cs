namespace Sharpbase.EventStreaming
{
    public class ChildAddedEventArgs : ChangeEventArgs
    {
        public ChildAddedEventArgs(Snapshot snapshot)
            : base(snapshot)
        {
        }

        public ChildAddedEventArgs(FirebaseException firebaseException)
            : base(firebaseException)
        {
        }
    }
}