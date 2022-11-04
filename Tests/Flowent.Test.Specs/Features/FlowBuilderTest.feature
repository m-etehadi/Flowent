Feature: FlowBuilderTest

Test basic functions of FlowBuilder class
 


Scenario: Test initialization step
	Given define initialization for the created FlowBuilder instance
	* define some validation for the created FlowBuilder instance
	When run flow instance of FlowBuilder<TestCommand>
	Then the TestCommand should be initialized
	* the TestCommand properties must be validated before exectuion
	* the TestCommand should be executed successfully

Scenario: Test validation step
	Given define initialization for the created FlowBuilder instance
	* define some invalid validation for the created FlowBuilder instance
	When run flow instance of FlowBuilder<TestCommand>
	Then validation handler prevents exectuion, because the TestCommand properties are not valid
	* the TestCommand cannot be executed



Scenario: Test onExectured event handler step
	Given define an onExecuted event handler
	When run flow instance of FlowBuilder<TestCommand>
	Then check if the onExecuted event handler is executed