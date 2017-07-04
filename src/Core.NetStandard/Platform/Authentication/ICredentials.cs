namespace Xlent.Lever.Libraries2.Core.Platform.Authentication
{
    /// <summary>
    /// Interface for credentials (á la username and password)
    /// </summary>
    public interface ICredentials
    {
        ///
        ///  The identity for the client that provides these credentials
        ///
        string ClientId { get; set; }

        ///
        ///  The proof that the client is who it claims it is
        /// 
        string ClientSecret { get; set; }
    }
}
