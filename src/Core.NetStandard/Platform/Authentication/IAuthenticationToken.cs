using System;

namespace Xlent.Lever.Libraries2.Core.Platform.Authentication
{
    /// <summary>
    /// An interface for a JWT token and some metadata for that token
    /// </summary>
    public interface IAuthenticationToken
    {

        /// <summary>
        /// The actual token
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// The token type for this token
        /// </summary>
        JwtTokenTypeEnum Type { get; }

        /// <summary>
        /// The time that this token expires
        /// </summary>
        DateTimeOffset ExpiresOn { get; }
    }
}