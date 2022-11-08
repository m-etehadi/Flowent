using Flowent.Command;

namespace Flowent.Test.Specs.Drivers
{
    public class TestCommand2 : ICommand
    {
        public int IntProp { get; set; }
        public string? Status { get; set; }

        // OUTPUT value
        public string Message => $"Here is Test Command2 with Status {Status}";


        public Task Execute()
        {
            return Task.CompletedTask;
        }

    }
}
