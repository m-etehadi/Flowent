Feature: FlowBuilderTest

Test basic functions of FlowBuilder class
 


Scenario: Test initialization step
	Given initialization step is defined for the FlowBuilder instance
	* some validations are defined for the created FlowBuilder instance
	When running the FlowBuilder<TestCommand> instance
	Then the TestCommand should be initialized
	* the TestCommand properties must be validated before exectuion
	* the TestCommand should be executed successfully

Scenario: Test validation step
	Given initialization step is defined for the FlowBuilder instance
	* some invalid validations are defined for the created FlowBuilder instance
	When running the FlowBuilder<TestCommand> instance
	Then validation handler prevents exectuion, because the TestCommand properties are not valid
	* the TestCommand cannot be executed

Scenario: Test onExectured event handler step
	Given an onExecuted event handler is defined
	When running the FlowBuilder<TestCommand> instance
	Then check if the onExecuted event handler is executed

Scenario: Test onException event handler
	Given an Exception handler is defined
	* test command is Initialized in a way that causes an error
	When running the FlowBuilder<TestCommand> instance
	Then check if the onException event handler is executed
#TODO: test inheritent exception type handeling

Scenario: Test IF-Else condition handler
	Given an IF-Condition handler with valid condition
	* an IF-Condition handler with invalid condition
	* an Else handler is defined
	When running the FlowBuilder<TestCommand> instance
	Then check if the valid condition handler is executed
	* check if the invalid condition handler is not executed
	* check if the else hanlder is executed

Scenario: Test Embeded Initializer handler	
	When running a command that implemented ICommandInitializer
	Then check if the embedded initializer is executed

Scenario: Test Embeded Validator handler	
	When running an invalid command that implemented ICommandValidator
	Then check if the embedded validator is executed

Scenario: Test Embeded Exception handler	
	When running an invalid command that implemented ICommandExceptionHandler
	Then check if the embedded exception handler is executed