using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace AntTweakBar
{
    /// <summary>
    /// A low-level wrapper to the AntTweakBar API.
    /// </summary>
    public partial class Tw
    {
        #if STANDALONE

        private static bool AlreadyUnpacked(String path, String checksum)
        {
            if (!File.Exists(path)) {
                return false;
            }

            using (var md5 = MD5.Create()) {
                using (var file = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    return BitConverter.ToString(md5.ComputeHash(file)).Replace("-", "").ToLower() == checksum;
                }
            }
        }

        private static void UnpackDLL()
        {
            var checksums = new Dictionary<String, String>()
            {
                { "AntTweakBar.Resources.AntTweakBar32.dll", "04ec8f97dffcba20d4fe00bfabe4ff46" },
                { "AntTweakBar.Resources.AntTweakBar64.dll", "bfe4a0029ef35e1299fcd52682cc9557" },
            };

            if (Environment.OSVersion.Platform != PlatformID.Win32NT) {
                var dstName = NativeMethods.DLLName + ".dll"; /* Unpacking is Windows-only. */
                var srcName = "AntTweakBar.Resources.AntTweakBar" + (IntPtr.Size * 8) + ".dll";

                if (AlreadyUnpacked(dstName, checksums[srcName])) {
                    return;
                }

                using (var src = Assembly.GetExecutingAssembly().GetManifestResourceStream(srcName)) {
                    using (var dst = new FileStream(dstName, FileMode.OpenOrCreate)) {
                        src.CopyTo(dst);
                    }
                }
            }
        }

        #endif

        /// <summary>
        /// Initializes the AntTweakBar.NET wrapper.
        /// </summary>
        static Tw()
        {
            #if STANDALONE
            UnpackDLL();
            #endif

            NativeMethods.TwHandleErrors(ptr =>
            {
                var handler = Error;

                if (handler != null) {
                    handler(null, new ErrorEventArgs(Helpers.StrFromPtr(ptr)));
                }
            });
        }
    }
}

