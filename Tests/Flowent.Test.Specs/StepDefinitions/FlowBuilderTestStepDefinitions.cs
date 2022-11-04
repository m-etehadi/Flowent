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
            _flowBuilderDriver.Create();
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

        [When(@"running the FlowBuilder<TestCommand> instance")]
        public void WhenRunningTheFlowBuilderTestCommandInstance()
        {
            _flowBuilderDriver.RunFlow();
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


    }
}
