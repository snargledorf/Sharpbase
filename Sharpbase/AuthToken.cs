using System;

namespace Sharpbase
{
    public struct AuthToken : IEquatable<AuthToken>
    {
        public static readonly AuthToken Empty = new AuthToken();

        public AuthToken(string token)
        {
            Token = token;
        }

        public string Token { get; }

        public override string ToString()
        {
            return Token;
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
            return string.Equals(Token, other.Token);
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
            return (Token != null ? Token.GetHashCode() : 0);
        }
    }
}