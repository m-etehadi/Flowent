using Flowent.Command;

namespace Flowent.Test.Specs.Drivers
{
    public class TestCommand1 : ICommand
    {
        public bool ThrowException { get; set; } = false;

        public int IntProp { get; set; }

        public string Status { get; set; }

        public string Output { get; private set; }

        public string Message => $"Here is Test Command with output {Output}";



        public Task Execute()
        {
            if (ThrowException)
                throw new ApplicationException();

            Output = $"{IntProp} - {Status}";
            return Task.CompletedTask;
        }
    }
}
