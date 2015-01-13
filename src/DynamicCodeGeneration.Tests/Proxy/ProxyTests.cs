using System;
using DynamicCodeGeneration.Proxy;
using NUnit.Framework;

namespace DynamicCodeGeneration.Tests.Proxy
{
    [TestFixture]
    public class ProxyTests
    {
        [Test]
        public void SimpleProxy()
        {
            ITest test = new Test();
            Console.WriteLine("== {0} ==", test.GetType().FullName);
            test.DoSomething(2, 3);

            ITest testProxy = new TestProxy(test);
            Console.WriteLine("== {0} ==", testProxy.GetType().FullName);
            testProxy.DoSomething(2, 3);
        }

        [Test]
        public void EmitProxy()
        {
            ITest test = new Test();
            Console.WriteLine("== {0} ==", test.GetType().FullName);
            test.DoSomething(2, 3);

            ITest testDynamiProxy = TracingProxyGenerator.CreateProxy(test);
            Console.WriteLine("== {0} ==", testDynamiProxy.GetType().FullName);
            testDynamiProxy.DoSomething(2, 3);
        }

        [Test]
        public void BLToolkitProxy()
        {
            ITest test = new Test();
            Console.WriteLine("== {0} ==", test.GetType().FullName);
            test.DoSomething(2, 3);

            ITest testDynamiProxy = BLToolkitTracingProxyGenerator.CreateProxy(test);
            Console.WriteLine("== {0} ==", testDynamiProxy.GetType().FullName);
            testDynamiProxy.DoSomething(2, 3);
        }

        [Test]
        public void RunSharpProxy()
        {
            ITest test = new Test();
            Console.WriteLine("== {0} ==", test.GetType().FullName);
            test.DoSomething(2, 3);

            ITest testDynamiProxy = RunSharpTracingProxyGenerator.CreateProxy(test);
            Console.WriteLine("== {0} ==", testDynamiProxy.GetType().FullName);
            testDynamiProxy.DoSomething(2, 3);
        }

        public interface ITest
        {
            int DoSomething(int a, int b);
        }

        public class Test : ITest
        {
            public int DoSomething(int a, int b)
            {
                Console.WriteLine("Inside DoSomething. Args: {0}, {1}.", a, b);
                return 1;
            }
        }

        public class TestProxy : ITest
        {
            private readonly ITest _instance;

            public TestProxy(ITest instance)
            {
                this._instance = instance;
            }

            public int DoSomething(int a, int b)
            {
                Console.WriteLine("Before DoSomething");

                var result = this._instance.DoSomething(a, b);

                Console.WriteLine("Before DoSomething");

                return result;
            }
        }
    }
}