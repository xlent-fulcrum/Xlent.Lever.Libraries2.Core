namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="ICrudMapper{TClientModel,TServerModel}" />
    /// <typeparam name="TClientModel">The model the client uses when updating items.</typeparam>
    /// <typeparam name="TServerModel">The model that the server uses. </typeparam>
    public interface ICrudMapper<TClientModel, TServerModel> : ICrudMapper<TClientModel, TClientModel, TServerModel>, ICrdMapper<TClientModel, TServerModel>
    {
    }

    /// <summary>
    /// Methods for mapping data between the client and server models.
    /// </summary>
    /// <typeparam name="TClientModelCreate">The model that the client uses when creating items.</typeparam>
    /// <typeparam name="TClientModel">The model the client uses when updating items.</typeparam>
    /// <typeparam name="TServerModel">The model that the server uses. </typeparam>
    public interface ICrudMapper<in TClientModelCreate, TClientModel, TServerModel> : ICrdMapper<TClientModelCreate, TClientModel, TServerModel>, IRudMapper<TClientModel, TServerModel>
    {
        /// <summary>
        /// Map fields to the server
        /// </summary>
        /// <param name="source">The client object that we should map to a server record.</param>
        /// <returns>A new server record with the mapped values from <paramref name="source"/>.</returns>
        TServerModel MapToServer(TClientModel source);
    }
}