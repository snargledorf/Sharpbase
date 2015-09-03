using System;

namespace Sharpbase
{
    public struct AuthToken : IEquatable<AuthToken>
    {
        public static readonly AuthToken Empty = new AuthToken();

        private readonly string token;

        public AuthToken(string token)
        {
            this.token = token;
        }

        public string Token
        {
            get
            {
                return token;
            }
        }

        public override string ToString()
        {
            return token;
        }

        public static bool operator ==(AuthToken left, AuthToken right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AuthToken left, AuthToken right)
        {
            return !(left == right);
        }

        public bool Equals(AuthToken other)
        {
            return string.Equals(token, other.token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is AuthToken && Equals((AuthToken)obj);
        }

        public override int GetHashCode()
        {
            return (token != null ? token.GetHashCode() : 0);
        }
    }
}