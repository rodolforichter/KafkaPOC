using System;
using System.Collections.Generic;
using System.Text;
using SystemMarshal = System.Runtime.InteropServices.Marshal;

namespace Richter.Kafka.Core
{
    internal static class Util
    {
        internal static class Marshal
        {
            /// <summary>
            ///     Interpret a zero terminated c string as UTF-8.
            /// </summary>
            public unsafe static string PtrToStringUTF8(IntPtr strPtr)
            {
                if (strPtr == IntPtr.Zero)
                {
                    return null;
                }

                // TODO: Is there a built in / vectorized / better way to implement this?              
                byte* pTraverse = (byte*)strPtr;
                while (*pTraverse != 0) { pTraverse += 1; }
                var length = (int)(pTraverse - (byte*)strPtr);
#if NET45
                var strBuffer = new byte[length];
                System.Runtime.InteropServices.Marshal.Copy(strPtr, strBuffer, 0, length);
                return Encoding.UTF8.GetString(strBuffer);
#else
                // Avoid unnecessary data copying on NET45+
                return Encoding.UTF8.GetString((byte*)strPtr.ToPointer(), length);
#endif
            }

            public static T PtrToStructure<T>(IntPtr ptr)
            {
#if NET45
                return (T)SystemMarshal.PtrToStructure(ptr, typeof(T));
#else
                return SystemMarshal.PtrToStructure<T>(ptr);
#endif
            }

            public static int SizeOf<T>()
            {
#if NET45
                return SystemMarshal.SizeOf(typeof(T));
#else
                return SystemMarshal.SizeOf<T>();
#endif
            }

            public static IntPtr OffsetOf<T>(string fieldName)
            {
#if NET45
                return SystemMarshal.OffsetOf(typeof(T), fieldName);
#else
                return SystemMarshal.OffsetOf<T>(fieldName);
#endif
            }
        }
    }
}
