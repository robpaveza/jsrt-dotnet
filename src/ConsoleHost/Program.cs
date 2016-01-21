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
                        engine.AddTypeToGlobal<Point3D>();
                        engine.AddTypeToGlobal<Point>();
                        engine.AddTypeToGlobal<Toaster>();
                        engine.AddTypeToGlobal<ToasterOven>();
                        var pt = new Point3D { X = 18, Y = 27, Z = -1 };
                        //engine.SetGlobalVariable("pt", engine.Converter.FromObject(pt));
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
    //var t = new ToasterOven();
    //t.addEventListener('toastcompleted', function(e) {
    //    echo('Toast is done!');
    //    echo('{0}', JSON.stringify(e));
    //});
    //t.addEventListener('loaftoasted', function(e) {
    //    echo('Loaf is done!');
    //    echo('{0}', JSON.stringify(e.e));
    //    echo('Cooked {0} pieces', e.e.PiecesCookied);
    //});
    //t.StartToasting();

    var o = new Point3D(1, 2, 3);
    echo(o.toString());
    o.X = 254;
    echo('{0}', o.X);
    o.Y = 189;
    o.Z = -254.341;
    echo('o after mutation? {0}', o.ToString());
    echo('{0}, {1}!', 'Hello', 'world');
    //echo('{0}', pt.X);
    //echo('{0}', pt.Y);
    //echo('{0}', pt.ToString());
    //pt.Y = 207;
    //echo('{0}', pt.ToString());

    sum(5, 20, 35, 100, 7).then(function(result) {
        echo('The result is {0} (or in hex, 0x{0:x8})', result);
    }, function(e) {
        echo('There was an error summing.');
        echo(e.ToString());
    });
})(this);");
                        fn.Invoke(Enumerable.Empty<JavaScriptValue>());

                        dynamic fnAsDynamic = fn;
                        fnAsDynamic.foo = 24;
                        dynamic global = engine.GlobalObject;
                        global.echo("{0}, {1}, via dynamic!", "Hey there", "world");

                        dynamic echo = global.echo;
                        echo("Whoa, {0}, that {1} {2}???", "world", "really", "worked");

                        foreach (dynamic name in global.Object.getOwnPropertyNames(global))
                        {
                            echo(name);
                        }

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

    public class Point
    {
        public double X
        {
            get;
            set;
        }
        
        public double Y
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public class Point3D : Point
    {
        public double Z
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}
