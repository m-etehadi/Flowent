using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowent
{
    public class ConditionBuilder<TCommand> where TCommand : ICommand, new()
    {
        ConditionBuilder<TCommand>? _else;
        FlowBuilder<TCommand> _currentAction;
        FlowBuilder _doAction;
        Func<TCommand, Task<bool>> _condition;



        public ConditionBuilder(FlowBuilder<TCommand> currentAction, Func<TCommand, Task<bool>> condition)
        {
            _currentAction = currentAction;
            _condition = condition;
        }

        public ConditionBuilder<TCommand> Do<NextCommand>(FlowBuilder<NextCommand> nextCommand) where NextCommand : ICommand, new()
        {
            _doAction = nextCommand;
            return this;
        }

        public ConditionBuilder<TCommand> ElseIf(Func<TCommand, bool> condition)
        {
            // Implement Else<NextCommand>() by combination of ElseIf(true) and Do()
            Func<TCommand, Task<bool>> asyncCondition = command => Task.Run(() => condition(command));
            return this._else = new ConditionBuilder<TCommand>(_currentAction, asyncCondition);
        }

        public FlowBuilder<TCommand> EndIf => this._currentAction;
        public FlowBuilder<TCommand> EndThen => EndIf;


        public async Task Run(TCommand commandInstance)
        {
            if (await _condition(commandInstance))
                await _doAction.Run();
            else if (_else != null)
                await _else.Run(commandInstance);
        }
    }
}
