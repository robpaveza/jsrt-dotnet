using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Microsoft.Scripting.JavaScript
{
    public sealed class JavaScriptArray : JavaScriptObject, IEnumerable<JavaScriptValue>
    {
        internal JavaScriptArray(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine):
            base(handle, type, engine)
        {

        }

        public int Length
        {
            get
            {
                var eng = GetEngine();
                return eng.Converter.ToInt32(GetPropertyByName("length"));
            }
        }

        public JavaScriptValue this[int index]
        {
            get { return GetAt(index); }
            set { SetAt(index, value); }
        }

        public JavaScriptValue GetAt(int index)
        {
            var eng = GetEngineAndClaimContext();

            JavaScriptValueSafeHandle resultHandle;
            using (var temp = eng.Converter.FromInt32(index))
            {
                Errors.ThrowIfIs(NativeMethods.JsGetIndexedProperty(handle_, temp.handle_, out resultHandle));
            }
            return eng.CreateValueFromHandle(resultHandle);
        }

        public void SetAt(int index, JavaScriptValue value)
        {
            var eng = GetEngineAndClaimContext();

            using (var temp = eng.Converter.FromInt32(index))
            {
                Errors.ThrowIfIs(NativeMethods.JsSetIndexedProperty(handle_, temp.handle_, value.handle_));
            }
        }

        private JavaScriptFunction GetArrayBuiltin(string name)
        {
            var eng = GetEngineAndClaimContext();
            var arrayCtor = eng.GlobalObject.GetPropertyByName("Array") as JavaScriptFunction;
            if (arrayCtor == null)
                Errors.ThrowIOEFmt(Errors.DefaultFnOverwritten, "Array");
            var arrayPrototype = arrayCtor.Prototype;
            if (arrayPrototype == null)
                Errors.ThrowIOEFmt(Errors.DefaultFnOverwritten, "Array.prototype");
            var fn = arrayPrototype.GetPropertyByName(name) as JavaScriptFunction;
            if (fn == null)
                Errors.ThrowIOEFmt(Errors.DefaultFnOverwritten, "Array.prototype." + name);

            return fn;
        }

        public JavaScriptValue Pop()
        {
            var fn = GetArrayBuiltin("pop");
            return fn.Invoke(new JavaScriptValue[] { this });
        }
        public void Push(JavaScriptValue value)
        {
            var fn = GetArrayBuiltin("pop");
            fn.Invoke(new JavaScriptValue[] { this, value });
        }
        public void Reverse()
        {
            var fn = GetArrayBuiltin("reverse");
            fn.Invoke(new JavaScriptValue[] { this });
        }

        public JavaScriptValue Shift()
        {
            var fn = GetArrayBuiltin("shift");
            return fn.Invoke(new JavaScriptValue[] { this });
        }
        public int Unshift(IEnumerable<JavaScriptValue> valuesToInsert)
        {
            var eng = GetEngine();
            var fn = GetArrayBuiltin("unshift");
            return eng.Converter.ToInt32(fn.Invoke(valuesToInsert.PrependWith(this)));
        }
        public void Sort(JavaScriptFunction compareFunction = null)
        {
            var fn = GetArrayBuiltin("sort");
            List<JavaScriptValue> args = new List<JavaScriptValue>();
            args.Add(this);
            if (compareFunction != null)
                args.Add(compareFunction);

            fn.Invoke(args);
        }
        public JavaScriptArray Splice(uint index, uint numberToRemove, IEnumerable<JavaScriptValue> valuesToInsert)
        {
            if (valuesToInsert == null)
                valuesToInsert = Enumerable.Empty<JavaScriptValue>();

            var eng = GetEngine();
            var args = valuesToInsert.PrependWith(this, eng.Converter.FromDouble(index), eng.Converter.FromDouble(numberToRemove));

            var fn = GetArrayBuiltin("splice");
            return fn.Invoke(args) as JavaScriptArray;
        }
        public JavaScriptArray Concat(IEnumerable<JavaScriptValue> itemsToConcatenate)
        {
            JavaScriptArray otherIsArray = itemsToConcatenate as JavaScriptArray;
            List<JavaScriptValue> args = new List<JavaScriptValue>();
            args.Add(this);
            if (otherIsArray != null)
            {
                args.Add(otherIsArray);
            }
            else
            {
                foreach (var item in itemsToConcatenate)
                    args.Add(item);
            }

            var fn = GetArrayBuiltin("concat");
            return fn.Invoke(args) as JavaScriptArray;
        }
        public string Join(string separator = "")
        {
            var eng = GetEngineAndClaimContext();
            List<JavaScriptValue> args = new List<JavaScriptValue>();
            args.Add(this);
            if (!string.IsNullOrEmpty(separator))
                args.Add(eng.Converter.FromString(separator));

            var fn = GetArrayBuiltin("join");
            return eng.Converter.ToString(fn.Invoke(args));
        }
        public JavaScriptArray Slice(int beginning)
        {
            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(GetEngine().Converter.FromInt32(beginning));

            return GetArrayBuiltin("slice").Invoke(args) as JavaScriptArray;
        }
        public JavaScriptArray Slice(int beginning, int end)
        {
            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(GetEngine().Converter.FromInt32(beginning));
            args.Add(GetEngine().Converter.FromInt32(end));

            return GetArrayBuiltin("slice").Invoke(args) as JavaScriptArray;
        }
        public int IndexOf(JavaScriptValue valueToFind)
        {
            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(valueToFind);

            return GetEngine().Converter.ToInt32(GetArrayBuiltin("indexOf").Invoke(args));
        }
        public int IndexOf(JavaScriptValue valueToFind, int startIndex)
        {
            var eng = GetEngine();
            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(valueToFind);
            args.Add(eng.Converter.FromInt32(startIndex));

            return eng.Converter.ToInt32(GetArrayBuiltin("indexOf").Invoke(args));
        }
        public int LastIndexOf(JavaScriptValue valueToFind)
        {
            var eng = GetEngine();
            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(valueToFind);

            return eng.Converter.ToInt32(GetArrayBuiltin("lastIndexOf").Invoke(args));
        }
        public int LastIndexOf(JavaScriptValue valueToFind, int lastIndex)
        {
            var eng = GetEngine();
            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(valueToFind);
            args.Add(eng.Converter.FromInt32(lastIndex));

            return eng.Converter.ToInt32(GetArrayBuiltin("lastIndexOf").Invoke(args));
        }

        public void ForEach(JavaScriptFunction callee)
        {
            if (callee == null)
                throw new ArgumentNullException(nameof(callee));

            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(callee);

            GetArrayBuiltin("forEach").Invoke(args);
        }
        public bool Every(JavaScriptFunction predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(predicate);

            return GetEngine().Converter.ToBoolean(GetArrayBuiltin("every").Invoke(args));
        }
        public bool Some(JavaScriptFunction predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(predicate);

            return GetEngine().Converter.ToBoolean(GetArrayBuiltin("some").Invoke(args));
        }
        public JavaScriptArray Filter(JavaScriptFunction predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(predicate);

            return GetArrayBuiltin("filter").Invoke(args) as JavaScriptArray;
        }
        public JavaScriptArray Map(JavaScriptFunction converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(converter);

            return GetArrayBuiltin("map").Invoke(args) as JavaScriptArray;
        }
        public JavaScriptValue Reduce(JavaScriptFunction aggregator)
        {
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));

            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(aggregator);

            return GetArrayBuiltin("reduce").Invoke(args);
        }
        public JavaScriptValue Reduce(JavaScriptFunction aggregator, JavaScriptValue initialValue)
        {
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));

            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(aggregator);
            args.Add(initialValue);

            return GetArrayBuiltin("reduce").Invoke(args);
        }
        public JavaScriptValue ReduceRight(JavaScriptFunction aggregator)
        {
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));

            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(aggregator);

            return GetArrayBuiltin("reduceRight").Invoke(args);
        }
        public JavaScriptValue ReduceRight(JavaScriptFunction aggregator, JavaScriptValue initialValue)
        {
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));

            var args = new List<JavaScriptValue>();
            args.Add(this);
            args.Add(aggregator);
            args.Add(initialValue);

            return GetArrayBuiltin("reduceRight").Invoke(args);
        }

        public IEnumerator<JavaScriptValue> GetEnumerator()
        {
            var len = this.Length;
            for (int i = 0; i < len; i++)
            {
                yield return GetAt(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
