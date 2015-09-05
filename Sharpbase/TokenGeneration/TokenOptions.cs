using System;

namespace Sharpbase.TokenGeneration
{
    public struct TokenOptions : IEquatable<TokenOptions>
    {
        public static TokenOptions Empty = new TokenOptions();

        /// <summary>
        /// Constructor.  All options are optional.
        /// </summary>
        /// <param name="notBefore">The date/time before which the token should not be considered valid. (default is now)</param>
        /// <param name="expires">The date/time at which the token should no longer be considered valid. (default is 24 hours from now)</param>
        /// <param name="admin">Set to true to bypass all security rules. (you can use this for trusted server code)</param>
        /// <param name="debug">Set to true to enable debug mode. (so you can see the results of Rules API operations)</param>
        public TokenOptions(
            DateTime? notBefore = null,
            DateTime? expires = null,
            bool admin = false,
            bool debug = false)
        {
            NotBefore = notBefore;
            Expires = expires;
            Admin = admin;
            Debug = debug;
        }

        public DateTime? Expires { get; }

        public DateTime? NotBefore { get; private set; }

        public bool Admin { get; }

        public bool Debug { get; }

        public static bool operator ==(TokenOptions left, TokenOptions right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TokenOptions left, TokenOptions right)
        {
            return !(left == right);
        }

        public bool Equals(TokenOptions other)
        {
            return NotBefore.Equals(other.NotBefore) && Expires.Equals(other.Expires) && Admin == other.Admin
                   && Debug == other.Debug;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is TokenOptions && Equals((TokenOptions)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = NotBefore.GetHashCode();
                hashCode = (hashCode * 397) ^ Expires.GetHashCode();
                hashCode = (hashCode * 397) ^ Admin.GetHashCode();
                hashCode = (hashCode * 397) ^ Debug.GetHashCode();
                return hashCode;
            }
        }
    }
}