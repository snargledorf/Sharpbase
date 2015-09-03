using System;
using System.Net;

using Sharpbase.Exceptions;

namespace Sharpbase
{
    public class FirebaseException : Exception
    {
        public FirebaseException(string message)
            : base(message)
        {
        }

        public FirebaseException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        public static FirebaseException ForHttpStatusCode(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new AuthDeniedException();
            }

            return new FirebaseException($"Request failed due to error code {(int)statusCode} ({Enum.GetName(typeof(HttpStatusCode), statusCode)})");
        }
    }
}