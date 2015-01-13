using System;
using System.Linq;
using System.Reflection;
using BLToolkit.Reflection.Emit;

namespace DynamicCodeGeneration.Proxy
{
    public class BLToolkitTracingProxyGenerator
    {
        public static TObject CreateProxy<TObject>(TObject instance)
            where TObject : class
        {
            if (!typeof(TObject).IsInterface)
                throw new ArgumentException("Only interface types are supported", "TObject");

            var name = string.Format("Proxy_{0}", Guid.NewGuid());

            // define type
            var typeBuilderHelper = new AssemblyBuilderHelper(name).DefineType("Proxy", typeof(object), typeof(TObject));

            // define field
            var fieldBuilder = typeBuilderHelper.DefineField("_instance", typeof(TObject), FieldAttributes.Private);

            // define constructor
            var ctorEmitter = typeBuilderHelper.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                                                            new[] { typeof(TObject) }).Emitter;
            ctorEmitter
                .ldarg_0
                .call(typeof (object).GetConstructors().First())
                .ldarg_0
                .ldarg_1
                .stfld(fieldBuilder)
                .ret();

            // define methods
            foreach (var method in typeof(TObject).GetMethods())
            {
                var methodEmitter = typeBuilderHelper.DefineMethod(method.Name,
                                                             MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Public,
                                                             method.CallingConvention,
                                                             method.ReturnType,
                                                             method.GetParameters().Select(p => p.ParameterType).ToArray()).Emitter;
                if (method.ReturnType != typeof(void))
                    methodEmitter.DeclareLocal(method.ReturnType);
                
                methodEmitter
                    .ldstr("Before " + method.Name)
                    .call(typeof (Console).GetMethod("WriteLine", new[] {typeof (string)}))
                    .ldarg_0
                    .ldfld(fieldBuilder);

                for (byte i = 1; i <= method.GetParameters().Length; i++)
                     methodEmitter.ldarg_s(i);

                methodEmitter.callvirt(method);

                if (method.ReturnType != typeof(void))
                    methodEmitter.stloc(0);

                methodEmitter
                    .ldstr("After " + method.Name)
                    .call(typeof (Console).GetMethod("WriteLine", new[] {typeof (string)}));

                if (method.ReturnType != typeof(void))
                    methodEmitter.ldloc(0);

                methodEmitter.ret();
            }

            // create type and instance
            return Activator.CreateInstance(typeBuilderHelper.Create(), instance) as TObject;
        }
    }
}