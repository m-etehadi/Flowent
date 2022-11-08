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

    public class TestCommandEmbeddedSteps : ICommand, ICommandInitializer
    {
        public List<MethodExectuionTrack> ExecutionTracks { get; set; } = new();

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
    }
}
