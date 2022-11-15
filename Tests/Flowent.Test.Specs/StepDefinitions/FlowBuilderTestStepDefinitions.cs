using Flowent.Test.Specs.Drivers;
using System;
using TechTalk.SpecFlow;

namespace Flowent.Test.Specs.StepDefinitions
{
    [Binding]
    public class FlowBuilderTestStepDefinitions
    {
        private readonly FlowBuilderDriver _flowBuilderDriver;

        public FlowBuilderTestStepDefinitions(FlowBuilderDriver flowBuilderDriver)
        {
            _flowBuilderDriver = flowBuilderDriver;
        }

        [Given(@"initialization step is defined for the FlowBuilder instance")]
        public void GivenInitializationStepIsDefinedForTheFlowBuilderInstance()
        {
            _flowBuilderDriver.DefineInitialization();
        }

        [Given(@"some validations are defined for the created FlowBuilder instance")]
        public void GivenSomeValidationsAreDefinedForTheCreatedFlowBuilderInstance()
        {
            _flowBuilderDriver.DefineValidValidation();
        }

        [Given(@"some invalid validations are defined for the created FlowBuilder instance")]
        public void GivenSomeInvalidValidationsAreDefinedForTheCreatedFlowBuilderInstance()
        {
            _flowBuilderDriver.DefineInValidValidation();
        }


        [Given(@"an onExecuted event handler is defined")]
        public void GivenAnOnExecutedEventHandlerIsDefined()
        {
            _flowBuilderDriver.DefineOnExecutedHandler();
        }


        [Given(@"an Exception handler is defined")]
        public void GivenAnExceptionHandlerIsDefined()
        {
            _flowBuilderDriver.DefineOnExceptionHandler();
        }

        [Given(@"test command is Initialized in a way that causes an error")]
        public void GivenTestCommandIsInitializedInAWayThatCausesAnError()
        {
            _flowBuilderDriver.InitializeCommandToThrowException();
        }


        [Given(@"an IF-Condition handler with valid condition")]
        public void GivenAnIF_ConditionHandlerWithValidCondition()
        {
            _flowBuilderDriver.DefineValidIfConditionHandler();
        }

        [Given(@"an IF-Condition handler with invalid condition")]
        public void GivenAnIF_ConditionHandlerWithInvalidCondition()
        {
            _flowBuilderDriver.DefineInvalidIfConditionHandler();
        }

        [Given(@"an Else handler is defined")]
        public void GivenAnElseHandlerIsDefined()
        {
            _flowBuilderDriver.DefineElseConditionHandler();
        }


        [When(@"running the FlowBuilder<TestCommand> instance")]
        public void WhenRunningTheFlowBuilderTestCommandInstance()
        {
            _flowBuilderDriver.RunConfiguredFlowInstance().Wait();
        }

        [When(@"running a command that implemented ICommandInitializer")]
        public void WhenRunningACommandThatImplementedICommandInitializer()
        {
            _flowBuilderDriver.RunEmbeddedHandlersFlowInstance().Wait();
        }

        [When(@"running an invalid command that implemented ICommandValidator")]
        public void WhenRunningAnInvalidCommandThatImplementedICommandValidator()
        {
            _flowBuilderDriver
                .RunEmbeddedHandlersFlowInstance(new TestCommandEmbeddedSteps() { IsValid1 = false, IsValid2 = false })
                .Wait();
        }

        [When(@"running an invalid command that implemented ICommandExceptionHandler")]
        public void WhenRunningAnInvalidCommandThatImplementedICommandExceptionHandler()
        {
            _flowBuilderDriver
                .RunEmbeddedHandlersFlowInstance(new TestCommandEmbeddedSteps() { ThrowExceptionOnExcution = true })
                .Wait();
        }

        [Then(@"the TestCommand should be initialized")]
        public void ThenTheTestCommandShouldBeInitializedBeforeExecution()
        {
            _flowBuilderDriver.IsInitialized().Should().BeTrue();
        }

        [Then(@"validation handler prevents exectuion, because the TestCommand properties are not valid")]
        public void ThenValidationHandlerPreventsExectuionBecauseTheTestCommandPropertiesAreNotValid()
        {
            _flowBuilderDriver.IsInvalidated();
        }


        [Then(@"the TestCommand should be executed successfully")]
        public void ThenTheTestCommandShouldBeExecutedSuccessfully()
        {
            _flowBuilderDriver.IsExecutedSuccessfully().Should().BeTrue();
        }

        [Then(@"the TestCommand cannot be executed")]
        public void ThenTheTestCommandCannotBeExecuted()
        {
            _flowBuilderDriver.IsExecutedSuccessfully().Should().BeFalse();
        }


        [Then(@"the TestCommand properties must be validated before exectuion")]
        public void ThenTheTestCommandPropertiesMustBeValidatedBeforeExectuion()
        {
            _flowBuilderDriver.IsValidated().Should().BeTrue();
        }

        [Then(@"check if the onExecuted event handler is executed")]
        public void ThenCheckIfTheOnExecutedEventHandlerIsExecuted()
        {
            _flowBuilderDriver.IsOnExecutedHandlerExectured().Should().BeTrue();
        }

        [Then(@"check if the onException event handler is executed")]
        public void ThenCheckIfTheOnExceptionEventHandlerIsExecuted()
        {
            _flowBuilderDriver.IsOnExceptionHandlerExecuted().Should().BeTrue();
        }

        [Then(@"check if the valid condition handler is executed")]
        public void ThenCheckIfTheValidConditionHandlerIsExecuted()
        {
            _flowBuilderDriver.IsValidIfConditionHandlerExecuted().Should().BeTrue();
        }

        [Then(@"check if the invalid condition handler is not executed")]
        public void ThenCheckIfTheInvalidConditionHandlerIsNotExecuted()
        {
            _flowBuilderDriver.IsInvalidIfConditionHandlerExecuted().Should().BeTrue();
        }

        [Then(@"check if the else hanlder is executed")]
        public void ThenCheckIfTheElseHanlderIsExecuted()
        {
            _flowBuilderDriver.IsElseConditionHandlerExecuted().Should().BeTrue();
        }

        [Then(@"check if the embedded initializer is executed")]
        public void ThenCheckIfTheEmbeddedInitializerIsExecuted()
        {
            _flowBuilderDriver.IsEmbededInitializationStepExecutedAtFirst().Should().BeTrue();
        }

        [Then(@"check if the embedded validator is executed")]
        public void ThenCheckIfTheEmbeddedValidatorIsExecuted()
        {
            _flowBuilderDriver.IsEmbededValidatorExectured().Should().BeTrue();
        }

        [Then(@"check if the embedded exception handler is executed")]
        public void ThenCheckIfTheEmbeddedExceptionHandlerIsExecuted()
        {
            _flowBuilderDriver.IsEmbededExceptionHandlerExectured().Should().BeTrue();
        }

    }
}
