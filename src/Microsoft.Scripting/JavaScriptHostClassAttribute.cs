using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting
{
    /// <summary>
    /// When applied to a class, modifies the way in which a host-provided class or object will be 
    /// projected into JavaScript.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class JavaScriptHostClassAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="JavaScriptHostClassAttribute"/> with the specified 
        /// <see cref="HostClassMode"/>.
        /// </summary>
        /// <param name="mode">The <see cref="HostClassMode"/> to use when projecting
        /// this class.</param>
        public JavaScriptHostClassAttribute(HostClassMode mode = HostClassMode.FullClass)
        {
            if (!Enum.IsDefined(typeof(HostClassMode), mode))
                throw new ArgumentException(nameof(mode));

            Mode = mode;
        }

        /// <summary>
        /// Gets the mode that was specified for this class.
        /// </summary>
        public HostClassMode Mode
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Specifies the mode in which the host class will be projected to JavaScript.
    /// </summary>
    public enum HostClassMode
    {
        /// <summary>
        /// Specifies that all public members will project along with the inheritance chain.  This
        /// mechanism is the default mode.
        /// </summary>
        FullClass,
        /// <summary>
        /// Specifies that only public members with the <see cref="JavaScriptHostMemberAttribute"/> 
        /// will project, and inherited classes will only project if they also have the 
        /// <see cref="JavaScriptHostClassAttribute"/>.
        /// </summary>
        OptIn,
    }
}
