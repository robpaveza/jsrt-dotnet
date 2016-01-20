using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public sealed class JavaScriptExecutionContext : IDisposable
    {
        private JavaScriptEngine engine_;
        private Action release_;

        internal JavaScriptExecutionContext(JavaScriptEngine engine, Action release)
        {
            Debug.Assert(engine != null);
            Debug.Assert(release != null);

            engine_ = engine;
            release_ = release;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~JavaScriptExecutionContext()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (release_ != null)
                release_();

            if (disposing)
            {
                engine_ = null;
                release_ = null;
            }
        }
    }
}
