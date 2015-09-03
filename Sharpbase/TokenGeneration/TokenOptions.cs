using System;

namespace Sharpbase.TokenGeneration
{
    public struct TokenOptions : IEquatable<TokenOptions>
    {
        public static TokenOptions Empty = new TokenOptions();

        private DateTime? notBefore;

        private readonly DateTime? expires;

        private readonly bool admin;

        private readonly bool debug;

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
            this.notBefore = notBefore;
            this.expires = expires;
            this.admin = admin;
            this.debug = debug;
        }

        public DateTime? Expires
        {
            get
            {
                return expires;
            }
        }

        public DateTime? NotBefore
        {
            get
            {
                return notBefore;
            }
            private set
            {
                notBefore = value;
            }
        }

        public bool Admin
        {
            get
            {
                return admin;
            }
        }

        public bool Debug
        {
            get
            {
                return debug;
            }
        }

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
            return notBefore.Equals(other.notBefore) && expires.Equals(other.expires) && admin == other.admin
                   && debug == other.debug;
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
                int hashCode = notBefore.GetHashCode();
                hashCode = (hashCode * 397) ^ expires.GetHashCode();
                hashCode = (hashCode * 397) ^ admin.GetHashCode();
                hashCode = (hashCode * 397) ^ debug.GetHashCode();
                return hashCode;
            }
        }
    }
}