using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting
{
    /// <summary>
    /// When applied to properties, methods, or events on a class, allows the name of the member to 
    /// be changed.  If the type is set to <c>OptIn</c> with the <see>JavaScriptHostClassAttribute</see>, 
    /// only those members with this attribute will be projected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class JavaScriptHostMemberAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="JavaScriptHostMemberAttribute"/>.
        /// </summary>
        public JavaScriptHostMemberAttribute()
        {

        }

        /// <summary>
        /// Gets or sets the projected name of the member.
        /// </summary>
        public string JavaScriptName
        {
            get;
            set;
        }
    }
}
