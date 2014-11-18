using System;

namespace AntTweakBar
{
    /// <summary>
    /// The exception that is thrown when an error occurs in the AntTweakBar library.
    /// </summary>
    public class AntTweakBarException : Exception
    {
        /// <summary>
        /// Gets a detailed description of the error from AntTweakBar.
        /// </summary>
        public String Details { get; private set; }

        /// <summary>
        /// Initializes a new AntTweakBar exception with an error message.
        /// </summary>
        /// <param name="message">A descriptive error message.</param>
        public AntTweakBarException(String message) : base(message)
        {
            Details = TW.GetLastError();
        }
    }
}
