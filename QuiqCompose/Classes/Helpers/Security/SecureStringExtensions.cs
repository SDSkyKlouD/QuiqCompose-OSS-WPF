using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace SDSK.QuiqCompose.WinDesktop.Classes.Helpers.Security {
    internal static class SecureStringExtensions {
        internal static SecureString ToSecureString(this string value, bool sealString = true) {
            if(string.IsNullOrEmpty(value)) {
                return null;
            }

            SecureString securedValue = new SecureString();
            value.ToCharArray().ToList().ForEach(securedValue.AppendChar);

            if(sealString) {
                securedValue.MakeReadOnly();
            }

            return securedValue;
        }

        internal static string ReadAsString(this SecureString value) {
            if(value == null) {
                return null;
            }

            IntPtr valuePtr = IntPtr.Zero;
            try {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            } finally {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
