// Decompiled with JetBrains decompiler
// Type: Xlent.Lever.Authentication.Sdk.Logic.ITokenRefresher
// Assembly: Xlent.Lever.Authentication.Sdk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19725B22-C3AF-4803-B23A-188B770C1A91
// Assembly location: C:\Git\Xlent.Lever\Xlent.Lever.Configurations\packages\Xlent.Lever.Authentication.Sdk.1.5.0\lib\Xlent.Lever.Authentication.Sdk\Xlent.Lever.Authentication.Sdk.dll

using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Platform.Authentication
{
    /// <summary>
    /// A Service Client that can refresh tokens.
    /// </summary>
    public interface ITokenRefresher
    {
        /// <summary>
        /// Get a cached <see cref="IAuthenticationToken"/>. If no token is cached or the token is becoming old, a new one is returned.
        /// </summary>
        /// <returns></returns>
        Task<IAuthenticationToken> GetJwtTokenAsync();
    }
}
