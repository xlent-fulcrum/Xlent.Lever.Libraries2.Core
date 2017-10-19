using System;
using System.Threading;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    public sealed class TestSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
            Console.WriteLine("post");
            d(state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            Console.WriteLine("send");
            d(state);
        }
    }
}