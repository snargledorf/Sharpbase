using System;

namespace Sharpbase.Exceptions
{
    public class AuthDeniedException : FirebaseException
    {
        public AuthDeniedException()
            : base("Operation failed due to denied authentication")
        {
        }

        public AuthDeniedException(string message)
            : base($"Operation failed due to denied authentication. Message: {message}")
        {
        }

        public AuthDeniedException(string message, Exception innerException) 
            : base($"Operation failed due to denied authentication. Message: {message}", innerException)
        {
        }
    }
}