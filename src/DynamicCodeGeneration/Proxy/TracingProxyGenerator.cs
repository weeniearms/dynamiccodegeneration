using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicCodeGeneration.Proxy
{
    public class TracingProxyGenerator
    {
         public static TObject CreateProxy<TObject>(TObject instance)
             where TObject : class
         {
             if (!typeof (TObject).IsInterface)
                 throw new ArgumentException("Only interface types are supported", "TObject");

             var name = string.Format("Proxy_{0}", Guid.NewGuid());

             // define assembly
             var assemblyName = new AssemblyName(name);
             var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

             // define module
             var moduleBuilder = assemblyBuilder.DefineDynamicModule(name);

             // define type
             var typeBuilder = moduleBuilder.DefineType("Proxy", TypeAttributes.Public, typeof(object), new[] { typeof(TObject) });

             // define field
             var fieldBuilder = typeBuilder.DefineField("_instance", typeof (TObject), FieldAttributes.Private);

             // define constructor
             var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                                                             new[] {typeof (TObject)});
             var ctorIl = ctorBuilder.GetILGenerator();
             ctorIl.Emit(OpCodes.Ldarg_0);
             ctorIl.Emit(OpCodes.Call, typeof(object).GetConstructors().First());
             ctorIl.Emit(OpCodes.Ldarg_0);
             ctorIl.Emit(OpCodes.Ldarg_1);
             ctorIl.Emit(OpCodes.Stfld, fieldBuilder);
             ctorIl.Emit(OpCodes.Ret);

             // define methods
             foreach(var method in typeof(TObject).GetMethods())
             {
                 var methodBuilder = typeBuilder.DefineMethod(method.Name, 
                                                              MethodAttributes.Virtual | MethodAttributes.Public,
                                                              method.CallingConvention,
                                                              method.ReturnType,
                                                              method.GetParameters().Select(p => p.ParameterType).ToArray());
                 var methodIl = methodBuilder.GetILGenerator();

                 if (method.ReturnType != typeof(void))
                     methodIl.DeclareLocal(method.ReturnType);

                 methodIl.Emit(OpCodes.Ldstr, "Before " + method.Name);
                 methodIl.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new [] { typeof(string) }));

                 methodIl.Emit(OpCodes.Ldarg_0);
                 methodIl.Emit(OpCodes.Ldfld, fieldBuilder);

                 for (short i = 1; i <= method.GetParameters().Length; i++)
                     methodIl.Emit(OpCodes.Ldarg_S, i);

                 methodIl.Emit(OpCodes.Callvirt, method);

                 if (method.ReturnType != typeof(void))
                     methodIl.Emit(OpCodes.Stloc_0);
                 
                 methodIl.Emit(OpCodes.Ldstr, "After " + method.Name);
                 methodIl.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));

                 if (method.ReturnType != typeof(void))
                     methodIl.Emit(OpCodes.Ldloc_0);
                 methodIl.Emit(OpCodes.Ret);
             }

             // create type and instance
             return Activator.CreateInstance(typeBuilder.CreateType(), instance) as TObject;
         }
    }
}