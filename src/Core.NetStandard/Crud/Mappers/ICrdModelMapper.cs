﻿using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TClientId"></typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface ICrdModelMapper<TClientModel, in TClientId, TServerModel> : 
        ICrdModelMapper<TClientModel, TClientModel, TClientId, TServerModel>
    {
    }

    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModelCreate">The client model to create an item.</typeparam>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TClientId"></typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface ICrdModelMapper<in TClientModelCreate, TClientModel, in TClientId, TServerModel> : ICreateModelMapper<TClientModelCreate, TClientId, TServerModel>, IReadModelMapper<TClientModel, TServerModel>
    where TClientModel : TClientModelCreate
    {
    }
}
