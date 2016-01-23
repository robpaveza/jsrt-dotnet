using Microsoft.Scripting;
using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var runtime = new JavaScriptRuntime())
            {
                runtime.MemoryChanging += Runtime_MemoryChanging;

                using (var engine = runtime.CreateEngine())
                {
                    using (var context = engine.AcquireContext())
                    {
                        engine.SetGlobalFunction("echo", Echo);
                        var sumFn = engine.CreateFunction(Sum);
                        engine.GlobalObject.SetPropertyByName("sum", sumFn);

                        var point = new Point3D() { X = 25, Y = 75, Z = -24.3 };
                        engine.SetGlobalFunction("printPoint", (e, c, t, a) =>
                        {
                            Console.WriteLine(point.ToString());
                            return e.UndefinedValue;
                        });
                        engine.SetGlobalVariable("point",
                            engine.Converter.FromObjectViaNewBridge(point));
                        
                        engine.RuntimeExceptionRaised += (sender, e) =>
                        {
                            dynamic error = engine.GetAndClearException();
                            dynamic glob = engine.GlobalObject;
                            var color = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;
                            var err = glob.JSON.stringify(error);
                            if ((string)err == "{}")
                                err = engine.Converter.ToString(error);
                            Console.WriteLine("Script error occurred: {0}", (string)err);
                            Console.ForegroundColor = color;
                        };

                        var fn = engine.EvaluateScriptText(@"(function(global) {
    //echo(point.ToString());
    printPoint();
    point.x = -15;
    for (var key in point) { echo('point: {0} = {1}', key, point[key]); }
for (var key in point.__proto__) { try { echo('point.__proto__: {0} = {1}', key, point.__proto__[key]); } catch(e) { } }
for (var key in point.__proto__.__proto__) { try { echo('point.__proto__.__proto__: {0} = {1}', key, point.__proto__.__proto__[key]); } catch(e) { } }
//for (var key in point.__proto__.__proto__.__proto__) { try { echo('point.__proto__.__proto__.__proto__: {0} = {1}', key, point.__proto__.__proto__.__proto__[key]); } catch (e) { } }
//for (var key in point.__proto__.__proto__.__proto__.constructor) { echo('point.__proto__.__proto__.__proto__.constructor: {0} = {1}', key, point.__proto__.__proto__.__proto__.constructor[key]); }
    echo(point.toString());
    //printPoint();
    point.y = 0;
    echo(point.toString());
    //printPoint();
    point.z = NaN;
    echo(point.toString());
    //printPoint();
    echo('Done');
})(this);");
                        fn.Invoke(Enumerable.Empty<JavaScriptValue>());

                    } // release context 

                    Console.ReadLine();
                }
            }
        }

        private static async Task<JavaScriptValue> Sum(JavaScriptEngine callingEngine, JavaScriptValue thisValue, IEnumerable<JavaScriptValue> args)
        {
            int[] values = args.Select(jsv => callingEngine.Converter.ToInt32(jsv)).ToArray();
            await Task.Delay(5000);

            using (var ctx = callingEngine.AcquireContext())
            {
                return callingEngine.Converter.FromInt32(values.Sum());
            }
        }

        private static void Runtime_MemoryChanging(object sender, JavaScriptMemoryAllocationEventArgs e)
        {
            Console.WriteLine($"Allocation/Change: {e.Type} :: {e.Amount}");
        }

        static JavaScriptValue Echo(JavaScriptEngine engine, bool construct, JavaScriptValue thisValue, IEnumerable<JavaScriptValue> arguments)
        {
            string fmt = arguments.First().ToString();
            object[] args = (object[])arguments.Skip(1).ToArray();
            Console.WriteLine(fmt, args);
            return engine.UndefinedValue;
        }
    }

    [JavaScriptHostClass(HostClassMode.OptIn)]
    public class Point
    {
        [JavaScriptHostMember(JavaScriptName = "x")]
        public double X
        {
            get;
            set;
        }
        
        [JavaScriptHostMember(JavaScriptName = "y")]
        public double Y
        {
            get;
            set;
        }

        [JavaScriptHostMember(JavaScriptName = "toString")]
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    [JavaScriptHostClass(HostClassMode.OptIn)]
    public class Point3D : Point
    {
        [JavaScriptHostMember(JavaScriptName = "z")]
        public double Z
        {
            get;
            set;
        }

        [JavaScriptHostMember(JavaScriptName = "toString")]
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}
