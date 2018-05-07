using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Crud.MemoryStorage;
using Xlent.Lever.Libraries2.Core.Test.NuGet;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Storage
{
    [TestClass]
    public class MemoryCrudTestParameters : TestParameters
    {

        /// <inheritdoc />
        public MemoryCrudTestParameters() : base(new CrudMemory<TestItemBare, Guid>())
        {
            FulcrumApplicationHelper.UnitTestSetup(nameof(MemoryCrudTestParameters));
        }
    }
}
