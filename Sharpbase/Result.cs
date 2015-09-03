namespace Sharpbase
{
    public class Result
    {
        public Result(Firebase reference)
        {
            Reference = reference;
        }

        public Result(FirebaseException error)
        {
            Error = error;
        }

        public Firebase Reference { get; }
        public FirebaseException Error { get; }

        public bool Success => Error == null && Reference != null;
    }
}