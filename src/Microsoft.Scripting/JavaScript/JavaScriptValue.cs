using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Microsoft.Scripting.JavaScript
{
    public class JavaScriptValue : DynamicObject, IDisposable
    {
        internal JavaScriptValueSafeHandle handle_;
        internal JavaScriptValueType type_;
        internal WeakReference<JavaScriptEngine> engine_;
        internal ChakraApi api_;

        internal JavaScriptEngine GetEngine()
        {
            JavaScriptEngine result;
            if (!engine_.TryGetTarget(out result))
                throw new ObjectDisposedException(nameof(JavaScriptEngine));

            return result;
        }

        internal JavaScriptValue(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine)
        {
            Debug.Assert(handle != null);
            Debug.Assert(engine != null);
            Debug.Assert(Enum.IsDefined(typeof(JavaScriptValueType), type));
            handle.SetEngine(engine);
            api_ = engine.Api;

            uint count;
            Errors.ThrowIfIs(api_.JsAddRef(handle.DangerousGetHandle(), out count));

            handle_ = handle;
            type_ = type;
            engine_ = new WeakReference<JavaScriptEngine>(engine);
        }

        public override string ToString()
        {
            var engine = GetEngine();
            return engine.Converter.ToString(this);
        }

        public JavaScriptValueType Type
        {
            get { return type_; }
        }

        public bool IsTruthy
        {
            get
            {
                var engine = GetEngine();
                return engine.Converter.ToBoolean(this);
            }
        }

        public bool SimpleEquals(JavaScriptValue other)
        {
            var eng = GetEngine();
            bool result;
            Errors.ThrowIfIs(api_.JsEquals(this.handle_, other.handle_, out result));

            return result;
        }

        public bool StrictEquals(JavaScriptValue other)
        {
            var eng = GetEngine();
            bool result;
            Errors.ThrowIfIs(api_.JsStrictEquals(this.handle_, other.handle_, out result));

            return result;
        }

        #region DynamicObject overrides

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var eng = GetEngine();
            if (binder.Type == typeof(int))
            {
                result = eng.Converter.ToInt32(this);
                return true;
            }
            else if (binder.Type == typeof(double))
            {
                result = eng.Converter.ToDouble(this);
                return true;
            }
            else if (binder.Type == typeof(string))
            {
                result = eng.Converter.ToString(this);
                return true;
            }
            else if (binder.Type == typeof(bool))
            {
                result = eng.Converter.ToBoolean(this);
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            var eng = GetEngine();

            switch (binder.Operation)
            {
                case ExpressionType.IsFalse:
                    result = !IsTruthy;
                    return true;
                case ExpressionType.IsTrue:
                    result = IsTruthy;
                    return true;

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    switch (Type)
                    {
                        case JavaScriptValueType.Number:
                            double n = eng.Converter.ToDouble(this);
                            result = -n;
                            return true;

                        case JavaScriptValueType.Boolean:
                            if (IsTruthy)
                                result = -1;
                            else
                                result = -0;
                            return true;
                            
                        // TODO
                        // case JavaScriptValueType.String:
                    }

                    result = double.NaN;
                    return true;

                case ExpressionType.UnaryPlus:
                    switch (Type)
                    {
                        case JavaScriptValueType.Number:
                            result = eng.Converter.ToDouble(this);
                            return true;

                        case JavaScriptValueType.Boolean:
                            if (IsTruthy)
                                result = 1;
                            else
                                result = 0;

                            return true;
                    }

                    result = double.NaN;
                    return true;
            }

            return base.TryUnaryOperation(binder, out result);
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (handle_ != null)
                {
                    handle_.Dispose();
                    handle_ = null;
                }
            }
        }

        ~JavaScriptValue()
        {
            Dispose(false);
        }
        #endregion
    }
}
