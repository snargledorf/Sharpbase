using System;
using System.Linq;

namespace Sharpbase
{
    internal class Path
    {
        private static readonly string[] EmptyPath = new string[0];

        private readonly string[] parts;

        public Path(string path)
        {
            parts = GetPathParts(path);
        }

        private Path(string[] parts)
        {
            this.parts = parts;
        }

        public Path()
        {
            parts = EmptyPath;
        }

        public string LastSegment
        {
            get
            {
                if (parts.Length == 0)
                    return string.Empty;

                return parts[parts.Length - 1];
            }
        }

        public Path Child(Path path)
        {
            string[] newParts = parts.Concat(path.parts).ToArray();
            return new Path(newParts);
        }

        public override string ToString()
        {
            return "/" + string.Join("/", parts);
        }

        private static string[] GetPathParts(string path)
        {
            return path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}