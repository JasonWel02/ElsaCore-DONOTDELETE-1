using Elsa.Workflows.Core;

namespace Elsa.Samples.AspNet.JasonsJobServer
{
    public class GreeterLocal : CodeActivity
    {
        protected override void Execute(ActivityExecutionContext context)
        {
            Console.WriteLine("Hello, world from GreeterLocal");
        }
    }
}