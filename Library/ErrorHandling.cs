using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace AntTweakBar
{
    /// <summary>
    /// The exception that is thrown when an error occurs in the AntTweakBar library.
    /// </summary>
    [Serializable]
    public sealed class AntTweakBarException : Exception
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
            Details = Tw.GetLastError();
        }

        public AntTweakBarException(String message, Exception innerException) : base(message, innerException)
        {
            Details = Tw.GetLastError();
        }

        public AntTweakBarException() : base()
        {
            Details = Tw.GetLastError();
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
        private AntTweakBarException(SerializationInfo info, StreamingContext context) : base (info, context)
        {
            this.Details = info.GetString("AntTweakBarException.Details");
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("AntTweakBarException.Details", this.Details);
        }
    }

    /// <summary>
    /// A low-level wrapper to the AntTweakBar API.
    /// </summary>
    public static partial class Tw
    {
        /// <summary>
        /// Event arguments for an AntTweakBar error.
        /// </summary>
        public class ErrorEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the error string.
            /// </summary>
            public String Error { get; private set; }

            /// <summary>
            /// Initializes event arguments for an AntTweakBar error.
            /// </summary>
            public ErrorEventArgs(String error)
            {
                Error = error;
            }
        }

        /// <summary>
        /// Occurs when an AntTweakBar error occurs.
        /// </summary>
        public static event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// Initializes the AntTweakBar.NET wrapper.
        /// </summary>
        static Tw()
        {
            NativeMethods.TwHandleErrors(error =>
            {
                var handler = Error;

                if (handler != null) {
                    handler(null, new ErrorEventArgs(error));
                }
            });
        }
    }
}
