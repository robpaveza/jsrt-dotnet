using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Scripting
{
    public class ScriptSource
    {
        private static IntPtr sourceContextId = IntPtr.Zero;

        public ScriptSource(string sourceLocation, string sourceText)
        {
            if (null == sourceLocation)
                throw new ArgumentNullException(nameof(sourceLocation));
            if (null == sourceText)
                throw new ArgumentNullException(nameof(sourceText));

            SourceLocation = sourceLocation;
            SourceText = sourceText;

            while (true)
            {
                IntPtr mySrcContextId = sourceContextId;
                IntPtr incremented = (sourceContextId + 1);

                Interlocked.CompareExchange(ref sourceContextId, incremented, mySrcContextId);
                if (sourceContextId == incremented)
                {
                    SourceContextId = mySrcContextId;
                    break;
                }
            }
        }

        public string SourceLocation
        {
            get;
            private set;
        }

        public string SourceText
        {
            get;
            private set;
        }

        internal IntPtr SourceContextId
        {
            get;
            private set;
        }
    }
}
