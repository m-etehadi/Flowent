

using Flowent.Command;

namespace Flowent.Samples.Basic.Commands
{
    public class TestCommand1 : ICommand, ICommandInitializer
    {
        public int IntProp { get; set; }

        public string Status { get; set; }

        public bool ThrowException { get; set; } = false;

        public string Message { get; set; }

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

        public async Task Initialize()
        {
            Message = $"Initializing command {this.GetType().Name} by inside function:{nameof(Initialize)}";
            Console.WriteLine(Message);
            await Task.CompletedTask;
        }
    }

}
