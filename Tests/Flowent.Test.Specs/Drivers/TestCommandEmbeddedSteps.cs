using Flowent.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Flowent.Test.Specs.Drivers
{
    public readonly record struct MethodExectuionTrack(DateTime Time, string Message, [CallerMemberName] string MethodName = "Unknown");

    public class TestCommandEmbeddedSteps : ICommand, ICommandInitializer, ICommandValidator
    {
        public List<MethodExectuionTrack> ExecutionTracks { get; set; } = new();
        public bool IsValid1 { get; set; } = true;
        public bool IsValid2 { get; set; } = true;


        // Mehods
        public async Task Execute()
        {
            ExecutionTracks.Add(new MethodExectuionTrack(DateTime.Now, "Command executed successfully"));
            await Task.CompletedTask;
        }

        public async Task Initialize()
        {
            ExecutionTracks.Add(new MethodExectuionTrack(DateTime.Now, "EmbededInitializer executed"));
            await Task.CompletedTask;
        }

        public async Task<AggregateException?> Validate()
        {
            List<Exception> validationExceptions = new List<Exception>();
            if (!IsValid1)
                validationExceptions.Add(new Exception($"{nameof(IsValid1)} is {IsValid1}") { HResult = 1 });

            if (!IsValid2)
                validationExceptions.Add(new Exception($"{nameof(IsValid2)} is {IsValid2}") { HResult = 2 });

            if (validationExceptions.Any())
                return await Task.FromResult<AggregateException>(new AggregateException(validationExceptions));
            else
                return await Task.FromResult<AggregateException?>(result: null);
        }
    }
}
