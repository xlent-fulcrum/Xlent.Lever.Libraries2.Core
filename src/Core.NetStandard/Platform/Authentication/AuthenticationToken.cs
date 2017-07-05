using System;

namespace Xlent.Lever.Libraries2.Core.Platform.Authentication
{
    /// <summary>
    /// A JWT token and some metadata for that token
    /// </summary>
    public class AuthenticationToken : IAuthenticationToken
    {

        /// <inheritdoc />
        public string AccessToken { get; set; }
        /// <inheritdoc />
        public JwtTokenTypeEnum Type => JwtTokenTypeEnum.Bearer;

        /// <inheritdoc />
        public DateTimeOffset ExpiresOn { get; set; }
    }
}
