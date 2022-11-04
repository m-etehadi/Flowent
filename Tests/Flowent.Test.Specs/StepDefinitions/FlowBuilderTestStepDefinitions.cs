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


        [Given(@"define initialization for the created FlowBuilder instance")]
        public void GivenInitializeTheCreatedInstance()
        {
            _flowBuilderDriver.DefineInitialization();
        }

        [Given(@"define some validation for the created FlowBuilder instance")]
        public void GivenDefineSomeValidationForTheCreatedFlowBuilderInstance()
        {
            _flowBuilderDriver.DefineValidValidation();
        }

        [Given(@"define some invalid validation for the created FlowBuilder instance")]
        public void GivenDefineSomeInvalidValidationForTheCreatedFlowBuilderInstance()
        {
            _flowBuilderDriver.DefineInValidValidation();
        }

        [Given(@"define an onExecuted event handler")]
        public void GivenDefineAnOnExecutedEventHandler()
        {
            _flowBuilderDriver.DefineOnExecutedHandler();
        }




        [When(@"run flow instance of FlowBuilder<TestCommand>")]
        public void WhenRunFlowInstanceOfFlowBuilderTestCommand()
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


    }
}
