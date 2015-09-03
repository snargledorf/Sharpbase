namespace Sharpbase
{
    internal class Cache : ICache
    {
        public void Replace(Path path, string jsonData)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Path path, string jsonData)
        {
            throw new System.NotImplementedException();
        }
    }

    internal interface ICache
    {
    }
}