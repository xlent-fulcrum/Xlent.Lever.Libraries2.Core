namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <summary>
    /// Methods for mapping data between the client and server models.
    /// </summary>
    /// <typeparam name="TClientModel">The model the client uses when updating items.</typeparam>
    /// <typeparam name="TServerModel">The model that the server uses. </typeparam>
    public interface IReadMapper<out TClientModel, in TServerModel>
    {
        /// <summary>
        /// Map fields from the server
        /// </summary>
        /// <param name="source">The record to map from</param>
        /// <returns>A new client object with the mapped values from <paramref name="source"/>.</returns>
        TClientModel MapFromServer(TServerModel source);
    }
}