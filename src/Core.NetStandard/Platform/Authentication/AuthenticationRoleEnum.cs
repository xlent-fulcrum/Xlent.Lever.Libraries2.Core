namespace Xlent.Lever.Libraries2.Core.Platform.Authentication
{
    /// <summary>
    /// Enumeration for the roles that authenication handles
    /// </summary>
    public enum AuthenticationRoleEnum
    {
        // TODO: Add more detailed documentation
        /// <summary>
        /// Have all rights
        /// </summary>
        SysAdminUser,
        /// <summary>
        /// A Fulcrum internal system
        /// </summary>
        InternalSystemUser,
        /// <summary>
        /// An external system (belonging to the organization)
        /// </summary>
        ExternalSystemUser,
        /// <summary>
        /// A human administrator
        /// </summary>
        Administrator
    }
}
