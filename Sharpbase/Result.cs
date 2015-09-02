namespace Sharpbase
{
    public class Result
    {
        public Snapshot Snapshot { get; internal set; }
        public Error Error { get; internal set; }

        public bool Success
        {
            get
            {
                return Error == null && Snapshot != null;
            }
        }
    }
}