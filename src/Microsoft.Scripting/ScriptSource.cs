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
        private static long sourceContextId = 0;

        public ScriptSource(string sourceLocation, string sourceText)
        {
            if (null == sourceLocation)
                throw new ArgumentNullException(nameof(sourceLocation));
            if (null == sourceText)
                throw new ArgumentNullException(nameof(sourceText));

            SourceLocation = sourceLocation;
            SourceText = sourceText;
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

        internal long SourceContextId
        {
            get;
        } = Interlocked.Increment(ref sourceContextId);
    }
}
