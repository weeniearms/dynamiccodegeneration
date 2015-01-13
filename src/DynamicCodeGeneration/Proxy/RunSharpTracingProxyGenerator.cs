using System;
using System.Linq;
using TriAxis.RunSharp;

namespace DynamicCodeGeneration.Proxy
{
    public class RunSharpTracingProxyGenerator
    {
        public static TObject CreateProxy<TObject>(TObject instance)
            where TObject : class
        {
            if (!typeof(TObject).IsInterface)
                throw new ArgumentException("Only interface types are supported", "TObject");

            var name = string.Format("Proxy_{0}", Guid.NewGuid());

            var asm = new AssemblyGen(name);

            TypeGen proxy = asm.Public.Class(name, typeof (object), new[] {typeof (TObject)});
            {
                FieldGen inst = proxy.Field(typeof (TObject), "_instance");

                CodeGen ctor = proxy.Public.Constructor()
                    .Parameter(typeof (TObject), "instance");
                {
                    ctor.Assign(inst, ctor.Arg("instance"));
                }

                foreach (var method in typeof(TObject).GetMethods())
                {
                    MethodGen meth = proxy.MethodImplementation(typeof (TObject), method.ReturnType, method.Name);
                    meth = method.GetParameters().Aggregate(meth, (current, p) => current.Parameter(p.ParameterType, p.Name));
                    CodeGen g = meth;
                    {
                        Operand retParam = null;

                        g.WriteLine("Before " + method.Name);

                        if (method.ReturnType != typeof(void))
                        {
                            retParam = g.Local(method.ReturnType,
                                                  inst.Invoke(method.Name,
                                                              method.GetParameters().Select(p => g.Arg(p.Name)).ToArray()));
                        }
                        else
                        {
                            g.Invoke(inst, method.Name,
                                        method.GetParameters().Select(p => g.Arg(p.Name)).ToArray());
                        }

                        g.WriteLine("After " + method.Name);

                        if (method.ReturnType != typeof(void))
                            g.Return(retParam);
                        else
                            g.Return();
                    }
                }
            }

            // create type and instance
            return Activator.CreateInstance(proxy.GetCompletedType(true), instance) as TObject;
        }
    }
}