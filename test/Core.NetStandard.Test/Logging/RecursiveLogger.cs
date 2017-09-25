﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    internal class RecursiveLogger : IFulcrumFullLogger
    {
        public static string Message { get; private set; }
        public static bool HasFailed { get; private set; }
        public static bool IsRunning { get; set; }
        public static int InstanceCount { get; set; }
        private static readonly object ClassLock = new object();

        /// <inheritdoc />
        public void Log(LogSeverityLevel logSeverityLevel, string message)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task LogAsync(LogInstanceInformation message)
        {
            lock (ClassLock)
            {
                InstanceCount++;
                IsRunning = true;
            }
            var uniqueString = Guid.NewGuid().ToString();
            var recursive = message.Message == uniqueString;
            if (recursive || !Logging.Log.OnlyForUnitTest_LoggingInProgress)
            {
                if (!HasFailed)
                {
                    HasFailed = true;
                    Message =
                        $"The {nameof(LogAsync)}() method should never be called recursively. {nameof(recursive)} = {recursive}, {nameof(Logging.Log.OnlyForUnitTest_LoggingInProgress)} = {Logging.Log.OnlyForUnitTest_LoggingInProgress}";
                }
            }
            Console.WriteLine(message.Message);
            // Try to provoke a recursive log call of this method
            Logging.Log.LogError(uniqueString);
            await Task.Yield();
            lock (ClassLock)
            {
                InstanceCount--;
                if (InstanceCount < 0)
                {
                    InstanceCount = 0;
                    if (!HasFailed)
                    {
                        HasFailed = true;
                        Message =
                            $"Unexpectedly had an {nameof(InstanceCount)} with value {InstanceCount} < 0";
                    }
                }
                if (InstanceCount == 0) IsRunning = false;
            }
        }
    }
}
