using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Guards
{
    public class TestException : Exception
    {
        public TestException(string message)
        :base(message)
        {
        }
    }
}
