using System;

namespace Sharpbase.Exceptions
{
    public class AuthDeniedException : Exception
    {
        public AuthDeniedException()
            : base("Operation failed due to denied authentication")
        {
            
        }
    }
}