using System;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    internal class ItemWithId : IUniquelyIdentifiable<Guid>
    {
        /// <inheritdoc />
        public Guid Id { get; set; }

        public string Value { get; set; }
    }
}