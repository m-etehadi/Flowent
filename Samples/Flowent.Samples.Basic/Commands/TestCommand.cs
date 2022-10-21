

namespace Flowent.Samples.Basic.Commands
{
    public class TestCommand : ICommand
    {
        // INPUT/OUTPUT value
        public int IntProp { get; set; }

        // INPUT value
        public string Status { get; set; }

        // OUTPUT value
        public string Output { get; private set; }

        public Task Execute()
        {
            Output = $"{IntProp} - {Status}";
            Console.WriteLine($"Here is Test Command with output {Output}");
            return Task.CompletedTask;
        }
    }

}
