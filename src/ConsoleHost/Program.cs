using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleHost
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var runtime = new JavaScriptRuntime())
            using (var engine = runtime.CreateEngine())
            {
                engine.SetGlobalFunction("echo", Echo);
                var fn = engine.EvaluateScriptText(@"(function() {
    echo('{0}, {1}!', 'Hello', 'world');
})();");
                var rEcho = fn.Invoke(Enumerable.Empty<JavaScriptValue>());
                Console.WriteLine("rEcho.Type: {0}", rEcho.Type); // Undefined

                var createPerson = engine.EvaluateScriptText(@"(function(){ return { name: 'Cameron', age: 36 } })();");
                var person = createPerson.Invoke(Enumerable.Empty<JavaScriptValue>());
                Console.WriteLine("person.Type: {0}", person.Type); // Object
                Console.WriteLine("person.GetType: {0}", person.GetType());
                var prsn = (JavaScriptObject)person;

                foreach (var mn in prsn.GetOwnPropertyNames())
                {
                    Console.WriteLine("mn: {0}", mn);
                }

                var name = prsn.GetPropertyByName("name");
                Console.WriteLine("name type: {0}", name.GetType());
                Console.WriteLine("name: {0}", name.ToString());

                var nm = prsn.GetPropertyByName("name");
                Console.WriteLine("name type: {0}", nm.Type);
                Console.WriteLine("name: {0}", nm.ToString());

                // How to I get the Number value?
                var age = prsn.GetPropertyByName("age");
                Console.WriteLine("age type: {0}", age.Type);
                //var a = age as JavaScriptNum
                //Console.WriteLine("age: {0}", age);

                // Is it possible to pass a JavaScriptObject back in?
                var getName = engine.EvaluateScriptText(@"(function(person){ return person.name; });");
                Console.WriteLine("getName type: {0}", getName.Type); // Function

                var getName2 = getName.Invoke(Enumerable.Empty<JavaScriptValue>()); // JsErrorScriptException
                Console.WriteLine("getName2 type: {0}", getName2.Type); // Function

                var getName3 = ((JavaScriptFunction)getName2).Invoke(new []{ person }); // JsErrorScriptException
                Console.WriteLine("getName3 type: {0}", getName3.Type); // String
                Console.WriteLine("getName3: {0}", getName3); // Cameron

                

                //Console.WriteLine("n: {0}", n.ToString());


                //dynamic fnAsDynamic = fn;
                //fnAsDynamic.foo = 24;
                //dynamic global = engine.GlobalObject;
                //global.echo("{0}, {1}, via dynamic!", "Hey there", "world");

                //dynamic echo = global.echo;
                //echo("Whoa, {0}, that {1} {2}???", "world", "really", "worked");

                //foreach (dynamic name in global.Object.getOwnPropertyNames(global))
                //{
                //    echo(name);
                //}
            }
            Console.ReadLine();
        }

        static JavaScriptValue Echo(JavaScriptEngine engine, bool construct, JavaScriptValue thisValue, IEnumerable<JavaScriptValue> arguments)
        {
            Console.WriteLine(arguments.First().ToString(), (object[])arguments.Skip(1).ToArray());
            return engine.UndefinedValue;
        }
    }
}
