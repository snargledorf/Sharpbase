namespace Sharpbase.EventStreaming
{
    public abstract class ChangeEventArgs
    {
        protected ChangeEventArgs(Snapshot snapshot)
        {
            Snapshot = snapshot;
        }

        protected ChangeEventArgs(FirebaseException firebaseException)
        {
            FirebaseException = firebaseException;
        }

        public Snapshot Snapshot { get; }

        public FirebaseException FirebaseException { get; }
    }
}