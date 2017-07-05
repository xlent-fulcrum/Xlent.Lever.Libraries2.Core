namespace Xlent.Lever.Libraries2.Core.Platform.Authentication
{
    /// <summary>
    /// Interface for credentials (á la username and password)
    /// </summary>
    public interface IAuthenticationCredentials
    {
        ///
        /// The identity for the client that provides these credentials.
        /// Corresponds to the "User name" in "User name and password".
        ///
        string ClientId { get; set; }

        ///
        /// The proof that the client is who it claims it is.
        /// Corresponds to the "password" in "User name and password"
        /// 
        string ClientSecret { get; set; }
    }
}
