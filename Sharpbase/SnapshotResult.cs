namespace Sharpbase
{
    internal class SnapshotResult : Result
    {
        public SnapshotResult(Snapshot snapshot, Firebase reference) : base(reference)
        {
            Snapshot = snapshot;
        }

        public SnapshotResult(FirebaseException error) : base(error)
        {
        }

        public Snapshot Snapshot { get; }
    }
}