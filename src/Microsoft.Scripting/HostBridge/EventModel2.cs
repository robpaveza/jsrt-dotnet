using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.HostBridge
{
    internal class EventModel2
    {
        private EventInfo event_;
        private bool static_;
        
        private EventModel2(EventInfo info, bool isStatic)
        {
            Debug.Assert(info != null);
            if (info == null) throw new ArgumentNullException(nameof(info));

            event_ = info;
            static_ = isStatic;
        }

        public static bool TryCreate(EventInfo info, bool isStatic, out EventModel2 result)
        {
            result = null;
            try
            {
                result = new EventModel2(info, isStatic);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                var attr = event_.GetCustomAttribute<JavaScriptHostMemberAttribute>();

                return attr?.JavaScriptName ?? event_.Name.ToLower();
            }
        }

        public string FullSetterName
        {
            get
            {
                if (static_)
                    return $"js#{event_.DeclaringType.FullName}.on{Name}.set";

                return $"js#{event_.DeclaringType.FullName}.prototype.on{Name}.set";
            }
        }

        public bool IsStatic
        {
            get
            {
                return static_;
            }
        }
    }
}
