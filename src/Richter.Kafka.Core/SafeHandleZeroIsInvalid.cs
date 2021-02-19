using System;
using System.Runtime.InteropServices;

namespace Richter.Kafka.Core
{
    abstract class SafeHandleZeroIsInvalid : SafeHandle
    {
        private string handleName;

        internal SafeHandleZeroIsInvalid(string handleName) : base(IntPtr.Zero, true)
        {
            this.handleName = handleName;
        }

        internal SafeHandleZeroIsInvalid(string handleName, bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
        {
            this.handleName = handleName;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
    }
}
