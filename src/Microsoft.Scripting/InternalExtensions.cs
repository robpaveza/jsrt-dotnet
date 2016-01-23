using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting
{
    internal static class InternalExtensions
    {
        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> sequence, params T[] itemsToPrepend)
        {
            foreach (var item in itemsToPrepend)
                yield return item;

            foreach (var item in sequence)
                yield return item;
        }

        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> sequence, T itemToPrepend)
        {
            yield return itemToPrepend;

            foreach (var item in sequence)
                yield return item;
        }

        public static bool NoneOrAll<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            // e.g. i => i % 2 == 0 (even numbers)
            // [0, 2, 4, 6] -- returnedTrue = true, returnedFalse = false, true ^ false == true
            // [1, 3, 5, 7] -- returnedTrue = false, returnedFalse = true, false ^ true == true
            // [0, 1, 2, 3] == returnedTrue = true, returnedFalse = true, true ^ true == false
            // [] == returnedTrue = false, returnedFalse = false.  Want this to succeed, so any = false == true.

            bool returnedTrue = false;
            bool returnedFalse = false;
            bool any = false;

            foreach (var item in sequence)
            {
                any = true;

                bool result = predicate(item);
                if (result)
                {
                    returnedTrue = true;
                }
                else
                {
                    returnedFalse = true;
                }
            }

            if (!any) return true;

            return returnedTrue ^ returnedFalse;
        }
    }
}
