using System;

namespace AntTweakBar
{
    /// <summary>
    /// Represents an internal AntTweakBar error.
    /// </summary>
    public class AntTweakBarException : Exception
    {
        private readonly String details;

        /// <summary>
        /// Gets a detailed description of the exception which occurred.
        /// </summary>
        public String Details { get { return details; } }

        /// <summary>
        /// Initializes a new AntTweakBar exception with a descriptive message.
        /// </summary>
        public AntTweakBarException(String message) : base(message)
        {
            details = TW.GetLastError();
        }
    }
}
