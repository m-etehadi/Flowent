using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowent.Command;

namespace Flowent
{
    public class ConditionBuilder<TCommand> where TCommand : ICommand, new()
    {
        ConditionBuilder<TCommand>? _else;
        FlowBuilder<TCommand> _currentAction;
        List<FlowBuilder> _doActions;
        Func<TCommand, Task<bool>> _condition;



        internal ConditionBuilder(FlowBuilder<TCommand> currentAction, Func<TCommand, Task<bool>> condition)
        {
            _currentAction = currentAction;
            _condition = condition;
            _doActions = new List<FlowBuilder>();
        }

        public ConditionBuilder<TCommand> Do<NextCommand>(FlowBuilder<NextCommand> nextCommand) where NextCommand : ICommand, new()
        {
            _doActions.Add(nextCommand);
            return this;
        }

        public ConditionBuilder<TCommand> ElseIf(Func<TCommand, bool> condition)
        {
            Func<TCommand, Task<bool>> asyncCondition = command => Task.Run(() => condition(command));
            return this._else = new ConditionBuilder<TCommand>(_currentAction, asyncCondition);
        }

        public FlowBuilder<TCommand> EndIf => this._currentAction;
        public FlowBuilder<TCommand> EndThen => EndIf;


        internal async Task Run(TCommand commandInstance)
        {
            if (await _condition(commandInstance))
                await Task.WhenAll(_doActions.Select(p => p.Run()));
            else if (_else != null)
                await _else.Run(commandInstance);
        }
    }
}
