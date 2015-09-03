using System;

namespace Sharpbase
{
    internal static class ArgUtils
    {
        public static void CheckForNull(object obj, string argumentName)
        {
            if (obj == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}