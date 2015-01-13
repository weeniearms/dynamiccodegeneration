public class Hello : DynamicCodeGeneration.Debugging.IHello
{
    public void SayHello(string name)
    {
        var helloMessage = "Hello, " + name;
        System.Console.WriteLine(helloMessage);
    }
}