using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace DynamicCodeGeneration.Debugging
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName("HelloAssembly"), AssemblyBuilderAccess.RunAndSave);
     
            var daCtor = typeof(DebuggableAttribute).GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
            var daBuilder = new CustomAttributeBuilder(daCtor,
                                                       new object[]
                                                           {
                                                               DebuggableAttribute.DebuggingModes.DisableOptimizations |
                                                               DebuggableAttribute.DebuggingModes.Default
                                                           });
            assemblyBuilder.SetCustomAttribute(daBuilder);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule("HelloAssembly.exe", true);

            // define document with source code
            var sourceCode = moduleBuilder.DefineDocument(@"Hello.txt", Guid.Empty, Guid.Empty, Guid.Empty);

            var typeBuilder = moduleBuilder.DefineType("Hello", TypeAttributes.Public | TypeAttributes.Class, typeof(object), new[] {typeof(IHello)});
            var methodbuilder = typeBuilder.DefineMethod("SayHello",
                                                         MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Public,
                                                         typeof (void), new[] {typeof (string)});

            var methodIl = methodbuilder.GetILGenerator();

            // declare local variable and set symbol info
            var localHelloMessage = methodIl.DeclareLocal(typeof(string));
            localHelloMessage.SetLocalSymInfo("helloMessage");

            // emit sequence point for line 5
            methodIl.MarkSequencePoint(sourceCode, 1, 1, 1, 100);
            methodIl.Emit(OpCodes.Ldstr, "Hello, ");
            methodIl.Emit(OpCodes.Ldarg_1);
            methodIl.Emit(OpCodes.Call, typeof (string).GetMethod("Concat", new[] {typeof (string), typeof (string)}));
            methodIl.Emit(OpCodes.Stloc_0);

            // emit sequence point for line 6
            methodIl.MarkSequencePoint(sourceCode, 2, 1, 2, 100);
            methodIl.Emit(OpCodes.Ldloc_0);
            methodIl.Emit(OpCodes.Call, typeof (Console).GetMethod("WriteLine", new[] {typeof (string)}));

            // emit sequence point for line 7
            methodIl.MarkSequencePoint(sourceCode, 3, 1, 3, 100);
            methodIl.Emit(OpCodes.Ret);

            var hello = Activator.CreateInstance(typeBuilder.CreateType()) as IHello;
            hello.SayHello("Asmodeus");
        }
    }
}
