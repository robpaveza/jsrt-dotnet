using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public sealed class JavaScriptMemoryAllocationEventArgs
    {
        private bool cancelled_;

        internal JavaScriptMemoryAllocationEventArgs(ulong amount, JavaScriptMemoryAllocationEventType type)
        {
            Amount = amount;
            Type = type;
        }

        public ulong Amount
        {
            get;
            private set;
        }

        public JavaScriptMemoryAllocationEventType Type
        {
            get;
            private set;
        }

        public bool Cancel
        {
            get { return cancelled_; }
            set
            {
                // once one event listener cancels, it's cancelled.
                cancelled_ |= value;
            }
        }

        public bool IsCancelable
        {
            get { return Type == JavaScriptMemoryAllocationEventType.AllocationRequest; }
        }
    }
}
