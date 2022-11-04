using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xunit.Sdk;

namespace Flowent.Test.Specs.Drivers
{
    public class FlowBuilderDriver
    {
        SomeContext _context = new SomeContext()
        {
            ContextVar1 = 1,
            ContextVar2 = "2",

        };

        public FlowBuilder<TestCommand> FlowInstance { get; private set; }
        public TestCommand CommandInstance { get; private set; }
        public AggregateException? RunException { get; private set; }

        public FlowBuilder<TestCommand> Create() => FlowInstance = new FlowBuilder<TestCommand>();



        public async void RunFlow(bool rethrowException = false)
        {
            try
            {
                CommandInstance = await FlowInstance.Run();
            }
            catch (AggregateException ex)
            {
                RunException = ex;
                if (rethrowException) throw;
            }
        }






        public bool IsExecutedSuccessfully() =>
            CommandInstance != null &&
            CommandInstance.Output == $"{CommandInstance.IntProp} - {CommandInstance.Status}";


        // Initialization
        public void DefineInitialization() => FlowInstance.Init.By(cmd => cmd.IntProp = 2, p => p.Status = "3");
        public bool IsInitialized() => CommandInstance.IntProp == 2 && CommandInstance.Status == "3";


        // Validation
        public void DefineValidValidation() => FlowInstance.Validate
                    .IfIsNot(cmd => cmd.Status == "3").Throw(cmd => new Exception("Status must be '3'"))
                    .If(cmd => cmd.Status == "1").Throw(cmd => new Exception("Status can not be '1'"));
        public bool IsValidated() => CommandInstance.Status == "3" && CommandInstance.Status != "1";

        public void DefineInValidValidation() => FlowInstance.Validate
                   .IfIsNot(cmd => cmd.Status == "Some Value").Throw(cmd => new Exception("Status must be 'Some Value'"))
                   .If(cmd => cmd.Status == "Invalid Status Value").Throw(cmd => new Exception("Status can not be 'Invalid Status Value'"));

        public void DefineOnExecutedHandler() => FlowInstance.On
            .ExecutedAsync(cmd => cmd.Status = "OnExecuted hanlder result",
                       cmd => cmd.IntProp = 100);

        public bool IsOnExecutedHandlerExectured() =>
            CommandInstance.Status == "OnExecuted hanlder result" &&
            CommandInstance.IntProp == 100 &&
            CommandInstance.Output == $"0 - "; // Output field's value will change in execution function while we are changing the values in OnExecuted event



        public bool IsInvalidated() =>
            RunException != null &&
            RunException.InnerExceptions.Any(e => e.Message == "Status must be 'Some Value'") &&
            RunException.InnerExceptions.Any(e => e.Message == "Status can not be 'Invalid Status Value'");


        public FlowBuilder GetSampleFlowBuilder()
        {
            var command = new FlowBuilder<TestCommand>()
               .Init.By(cmd => cmd.IntProp = 2, p => p.Status = "3")
               .Validate
                   .IfIsNot(cmd => cmd.Status == "3").Throw(cmd => new Exception("Status must be '3'"))
                   .If(cmd => cmd.Status == "1").Throw(cmd => new Exception("Status can not be '1'"))
               .EndValidate
               .On
                   .Exception<ArgumentNullException>((cmd, ex) => Task.Run(() => _context.Message = ex.Message))
                   .ExecutedAsync(p => p.Status = "Done",
                               p => _context.Message = "Complted!",
                               p => _context.ContextVar2 = p.Output)
               .EndOn
               .Then()
                   .Do<TestCommand2>(new FlowBuilder<TestCommand2>())
               .EndThen
               .If(cmd => cmd.IntProp == 11)
                   .Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                            .Init.By(p => p.Status = "Init2"))
               .ElseIf(p => true)
                   .Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                            .Init.By(() => new TestCommand2
                                            {
                                                IntProp = 12,
                                                Status = "Else condition",
                                            }))
               .EndIf;

            return command;
        }


    }

    public class SomeContext
    {
        public int ContextVar1 { get; set; }
        public string ContextVar2 { get; set; }

        public string Message { get; set; }
    }

    public class TestCommand : ICommand
    {
        // INPUT/OUTPUT value
        public int IntProp { get; set; }

        // INPUT value
        public string Status { get; set; }

        // OUTPUT value
        public string Output { get; private set; }

        // OUTPUT value
        public string Message => $"Here is Test Command with output {Output}";

        public Task Execute()
        {
            Output = $"{IntProp} - {Status}";
            return Task.CompletedTask;
        }
    }

    public class TestCommand2 : ICommand
    {
        public int IntProp { get; set; }
        public string Status { get; set; }

        // OUTPUT value
        public string Message => $"Here is Test Command2 with Status {Status}";


        public Task Execute()
        {
            return Task.CompletedTask;
        }

        public void OnExecuted() { }

        public void OnExecuting() { }

        //public void OnValidated() { }

        //public void OnValidating() { }

    }
}
