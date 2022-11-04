

namespace Flowent.Samples.Basic.Commands
{
    public class TestCommand1 : ICommand
    {
        public int IntProp { get; set; }

        public string Status { get; set; }

        public bool ThrowException { get; set; } = false;

        // OUTPUT value
        public string Output { get; private set; }

        public Task Execute()
        {
            if (ThrowException) 
                throw new ApplicationException();

            Output = $"{IntProp} - {Status}";
            Console.WriteLine($"Here is Test Command with output {Output}");
            return Task.CompletedTask;
        }
    }

}
