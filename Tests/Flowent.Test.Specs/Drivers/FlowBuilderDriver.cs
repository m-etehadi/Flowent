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
        public FlowBuilder<TestCommand1> TestCommand1ConfiguredFlowInstance { get; private set; } = new FlowBuilder<TestCommand1>();
        public TestCommand1? TestCommand1CommandInstance { get; private set; }


        public FlowBuilder<TestCommandEmbeddedSteps> TestCommandEmbeddedStepsConfigFlowInstance { get; private set; } = new FlowBuilder<TestCommandEmbeddedSteps>();
        public TestCommandEmbeddedSteps? TestCommandEmbeddedStepsInstance { get; private set; }

        public AggregateException? ValidationException { get; private set; }
        public Exception? ExecutionException { get; set; }


        //Methods
        public async Task RunConfiguredFlowInstance(ICommand? cmd = default)
        {
            TestCommand1CommandInstance = await RunFlowInstance<TestCommand1>(TestCommand1ConfiguredFlowInstance, cmd);
        }

        public async Task RunEmbeddedHandlersFlowInstance(ICommand? cmd = default)
        {            
            TestCommandEmbeddedStepsInstance = await RunFlowInstance<TestCommandEmbeddedSteps>(TestCommandEmbeddedStepsConfigFlowInstance, cmd);
        }

        public async Task<TCommand?> RunFlowInstance<TCommand>(FlowBuilder<TCommand> flowBuilder, ICommand? cmd = default)
            where TCommand : ICommand, new()
        {
            try
            {
                return await flowBuilder.Run(cmd);
            }
            catch (AggregateException ex)
            {
                ValidationException = ex;
            }
            catch (Exception ex)
            {
                ExecutionException = ex;
            }

            return await Task.FromResult<TCommand?>(default);
        }


        public bool IsExecutedSuccessfully() =>
            TestCommand1CommandInstance != null &&
            TestCommand1CommandInstance.Output == $"{TestCommand1CommandInstance.IntProp} - {TestCommand1CommandInstance.Status}";

        #region Initialization Step

        public void DefineInitialization() => TestCommand1ConfiguredFlowInstance.Init.By(cmd => cmd.IntProp = 2, p => p.Status = "3");
        public bool IsInitialized() =>
            TestCommand1CommandInstance != null &&
            TestCommand1CommandInstance.IntProp == 2 &&
            TestCommand1CommandInstance.Status == "3";

        #endregion

        #region Validation Step

        public void DefineValidValidation() => TestCommand1ConfiguredFlowInstance.Validate
                    .IfIsNot(cmd => cmd.Status == "3").Throw(cmd => new Exception("Status must be '3'"))
                    .If(cmd => cmd.Status == "1").Throw(cmd => new Exception("Status can not be '1'"));
        public bool IsValidated() => TestCommand1CommandInstance!.Status == "3" && TestCommand1CommandInstance.Status != "1";

        public void DefineInValidValidation() => TestCommand1ConfiguredFlowInstance.Validate
                   .IfIsNot(cmd => cmd.Status == "Some Value").Throw(cmd => new Exception("Status must be 'Some Value'"))
                   .If(cmd => cmd.Status == "Invalid Status Value").Throw(cmd => new Exception("Status can not be 'Invalid Status Value'"));
        #endregion

        #region On Executed Step
        public void DefineOnExecutedHandler() => TestCommand1ConfiguredFlowInstance.On
            .ExecutedAsync(cmd => cmd.Status = "OnExecuted hanlder result",
                       cmd => cmd.IntProp = 100);

        public bool IsOnExecutedHandlerExectured() =>
            TestCommand1CommandInstance != null &&
            TestCommand1CommandInstance.Status == "OnExecuted hanlder result" &&
            TestCommand1CommandInstance.IntProp == 100 &&
            TestCommand1CommandInstance.Output == $"0 - "; // Output field's value will change in execution function while we are changing the values in OnExecuted event

        public bool IsInvalidated() =>
            ValidationException != null &&
            ValidationException.InnerExceptions.Any(e => e.Message == "Status must be 'Some Value'") &&
            ValidationException.InnerExceptions.Any(e => e.Message == "Status can not be 'Invalid Status Value'");
        #endregion

        #region On Exception Step

        public bool OnExceptionHandlerExecuted = false;
        public void DefineOnExceptionHandler() => TestCommand1ConfiguredFlowInstance.On
            .Exception<ApplicationException>((cmd, ex) =>
            {
                OnExceptionHandlerExecuted = true;
                return Task.CompletedTask;
            });
        public void InitializeCommandToThrowException() => TestCommand1ConfiguredFlowInstance.Init
            .By(cmd => cmd.ThrowException = true);

        public bool IsOnExceptionHandlerExecuted() => OnExceptionHandlerExecuted;

        #endregion

        #region IF Condition Step

        bool _executedValidIfConditionHandler, _executedInvalidIfConditionHandler = false;
        bool _executedElseConditionHandler = false;
        public void DefineValidIfConditionHandler() => TestCommand1ConfiguredFlowInstance
            !.If(cmd => true).Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                              .On
                                                  .ExecutedAsync(cmd => _executedValidIfConditionHandler = true)
                                              .EndOn);

        public void DefineInvalidIfConditionHandler() => TestCommand1ConfiguredFlowInstance
            !.If(cmd => true).Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                      .On
                                          .ExecutedAsync(cmd => _executedInvalidIfConditionHandler = true)
                                      .EndOn);

        public void DefineElseConditionHandler() => TestCommand1ConfiguredFlowInstance
                !.If(cmd => false).Do<TestCommand2>(new FlowBuilder<TestCommand2>())
                .ElseIf(cmd => true).Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                              .On
                                                  .ExecutedAsync(cmd => _executedElseConditionHandler = true)
                                              .EndOn);
        public bool IsValidIfConditionHandlerExecuted() => _executedValidIfConditionHandler;

        public bool IsInvalidIfConditionHandlerExecuted() => _executedInvalidIfConditionHandler;

        public bool IsElseConditionHandlerExecuted() => _executedElseConditionHandler;

        #endregion

        #region Embedded Initialization Step


        internal bool IsEmbededInitializationStepExecutedAtFirst() =>
                TestCommandEmbeddedStepsInstance != null &&
                TestCommandEmbeddedStepsInstance.ExecutionTracks.OrderBy(p => p.Time).First().MethodName == nameof(TestCommandEmbeddedStepsInstance.Initialize) &&
                TestCommandEmbeddedStepsInstance.ExecutionTracks.OrderBy(p => p.Time).Any(p => p.MethodName == nameof(TestCommandEmbeddedStepsInstance.Execute));

        #endregion

        #region Embeded Validator Step
        internal bool IsEmbededValidatorExectured() =>
                ValidationException != null &&
                ValidationException.InnerExceptions.Any(p => p.HResult == 1) &&
                ValidationException.InnerExceptions.Any(p => p.HResult == 2);

        #endregion


        #region Embedded Exception Handler
        internal bool IsEmbededExceptionHandlerExectured() =>
            TestCommandEmbeddedStepsInstance != null &&
            TestCommandEmbeddedStepsInstance.ExecutionTracks.Any(p => p.MethodName == nameof(TestCommandEmbeddedStepsInstance.ExceptionHandler));

        #endregion

    }

}
