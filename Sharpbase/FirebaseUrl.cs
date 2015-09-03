using System;

namespace Sharpbase
{
    internal struct FirebaseUrl
    {
        public FirebaseUrl(Uri baseUrl, Path path)
        {
            BaseUrl = baseUrl;
            Path = path;
        }

        public static FirebaseUrl Parse(string url)
        {
            var uri = new Uri(url);

            var path = new Path(uri.AbsolutePath);
            var baseUrl = new Uri($"{uri.Scheme}://{uri.Host}");

            return new FirebaseUrl(baseUrl, path);
        }

        public Uri BaseUrl { get; }

        public Path Path { get; }
    }
}