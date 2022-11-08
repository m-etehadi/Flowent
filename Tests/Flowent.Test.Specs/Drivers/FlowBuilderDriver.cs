using Flowent.Command;
using Flowent.Exceptions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xunit.Sdk;
using ICommand = Flowent.Command.ICommand;

namespace Flowent.Test.Specs.Drivers
{
    public class FlowBuilderDriver
    {
        public FlowBuilder<TestCommand1> ConfiguredFlowInstance { get; private set; }
        public TestCommand1? CommandInstance { get; private set; }
        public TestCommandEmbeddedSteps? CommandWithEmbededSteps { get; private set; }

        public ValidatorException? ValidationException { get; private set; }
        public Exception? ExecutionException { get; set; }

        public FlowBuilder<TestCommand1> Create() => ConfiguredFlowInstance = new FlowBuilder<TestCommand1>();



        public async Task RunConfiguredFlowInstance()
        {
            CommandInstance = await RunFlowInstance<TestCommand1>(ConfiguredFlowInstance);
        }

        public async Task RunEmbeddedHandlersFlowInstance()
        {
            CommandWithEmbededSteps = await new FlowBuilder<TestCommandEmbeddedSteps>().Run();
        }

        public async Task<TCommand> RunFlowInstance<TCommand>(FlowBuilder<TCommand> flowBuilder) 
            where TCommand : ICommand, new()
        {
            try
            {
                return await flowBuilder.Run();
            }
            catch (ValidatorException ex)
            {
                ValidationException = ex;
            }
            catch (Exception ex)
            {
                ExecutionException = ex;
            }

            return await Task.FromResult<TCommand>(default);
        }

       




        public bool IsExecutedSuccessfully() =>
            CommandInstance != null &&
            CommandInstance.Output == $"{CommandInstance.IntProp} - {CommandInstance.Status}";

        #region Initialization Step

        public void DefineInitialization() => ConfiguredFlowInstance.Init.By(cmd => cmd.IntProp = 2, p => p.Status = "3");
        public bool IsInitialized() => CommandInstance.IntProp == 2 && CommandInstance.Status == "3";

        #endregion

        #region Validation Step

        public void DefineValidValidation() => ConfiguredFlowInstance.Validate
                    .IfIsNot(cmd => cmd.Status == "3").Throw(cmd => new Exception("Status must be '3'"))
                    .If(cmd => cmd.Status == "1").Throw(cmd => new Exception("Status can not be '1'"));
        public bool IsValidated() => CommandInstance.Status == "3" && CommandInstance.Status != "1";

        public void DefineInValidValidation() => ConfiguredFlowInstance.Validate
                   .IfIsNot(cmd => cmd.Status == "Some Value").Throw(cmd => new Exception("Status must be 'Some Value'"))
                   .If(cmd => cmd.Status == "Invalid Status Value").Throw(cmd => new Exception("Status can not be 'Invalid Status Value'"));
        #endregion

        #region On Executed Step
        public void DefineOnExecutedHandler() => ConfiguredFlowInstance.On
            .ExecutedAsync(cmd => cmd.Status = "OnExecuted hanlder result",
                       cmd => cmd.IntProp = 100);

        public bool IsOnExecutedHandlerExectured() =>
            CommandInstance.Status == "OnExecuted hanlder result" &&
            CommandInstance.IntProp == 100 &&
            CommandInstance.Output == $"0 - "; // Output field's value will change in execution function while we are changing the values in OnExecuted event

        public bool IsInvalidated() =>
            ValidationException != null &&
            ValidationException.InnerExceptions.Any(e => e.Message == "Status must be 'Some Value'") &&
            ValidationException.InnerExceptions.Any(e => e.Message == "Status can not be 'Invalid Status Value'");
        #endregion

        #region On Exception Step

        public bool OnExceptionHandlerExecuted = false;
        public void DefineOnExceptionHandler() => ConfiguredFlowInstance.On
            .Exception<ApplicationException>((cmd, ex) =>
            {
                OnExceptionHandlerExecuted = true;
                return Task.CompletedTask;
            });
        public void InitializeCommandToThrowException() => ConfiguredFlowInstance.Init
            .By(cmd => cmd.ThrowException = true);

        public bool IsOnExceptionHandlerExecuted() => OnExceptionHandlerExecuted;

        #endregion

        #region IF Condition Step

        bool _executedValidIfConditionHandler, _executedInvalidIfConditionHandler = false;
        bool _executedElseConditionHandler = false;
        public void DefineValidIfConditionHandler() => ConfiguredFlowInstance
            .If(cmd => true).Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                              .On
                                                  .ExecutedAsync(cmd => _executedValidIfConditionHandler = true)
                                              .EndOn);

        public void DefineInvalidIfConditionHandler() => ConfiguredFlowInstance
            .If(cmd => true).Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                      .On
                                          .ExecutedAsync(cmd => _executedInvalidIfConditionHandler = true)
                                      .EndOn);

        public void DefineElseConditionHandler() => ConfiguredFlowInstance
                .If(cmd => false).Do<TestCommand2>(new FlowBuilder<TestCommand2>())
                .ElseIf(cmd => true).Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                              .On
                                                  .ExecutedAsync(cmd => _executedElseConditionHandler = true)
                                              .EndOn);
        public bool IsValidIfConditionHandlerExecuted() => _executedValidIfConditionHandler;

        public bool IsInvalidIfConditionHandlerExecuted() => _executedInvalidIfConditionHandler;

        public bool IsElseConditionHandlerExecuted() => _executedElseConditionHandler;

        #endregion

        #region Embedded Initialization Step


        //internal void DefineAnInstanceOfICommandInitializer() => _commandWithEmbededSteps = new TestCommandEmbeddedSteps();

        internal bool IsEmbededInitializationStepExecutedAtFirst() =>
                CommandWithEmbededSteps != null &&
                CommandWithEmbededSteps.ExecutionTracks.OrderBy(p => p.Time).First().MethodName == nameof(CommandWithEmbededSteps.Initialize) &&
                CommandWithEmbededSteps.ExecutionTracks.OrderBy(p => p.Time).Any(p => p.MethodName == nameof(CommandWithEmbededSteps.Execute));

        #endregion
    }

}
