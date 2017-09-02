﻿using System;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// Interface for basic logging
    /// </summary>
    public interface IFulcrumLogger
    {
        /// <summary>
        /// Log <paramref name="message"/> with level <paramref name="logSeverityLevel"/>.
        /// </summary>
        void Log(LogSeverityLevel logSeverityLevel, string message);
    }
}