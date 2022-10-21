using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowent
{
    public class ConditionalBuilder<TCommand> where TCommand : ICommand, new()
    {
        ConditionalBuilder<TCommand>? _else;
        CommandBuilder<TCommand> _currentAction;
        CommandBuilder? _doAction;
        Func<TCommand, bool> _condition;



        public ConditionalBuilder(CommandBuilder<TCommand> currentAction, Func<TCommand, bool> condition)
        {
            _currentAction = currentAction;
            _condition = condition;
        }

        public ConditionalBuilder<TCommand> Run<NextCommand>(CommandBuilder<NextCommand> nextCommand) where NextCommand : ICommand, new()
        {
            _doAction = nextCommand;
            return this;
        }

        public ConditionalBuilder<TCommand> ElseIf(Func<TCommand, bool> condition)
        {
            // Implement Else<NextCommand>() by combination of ElseIf(true) and Do()
            return this._else = new ConditionalBuilder<TCommand>(_currentAction, condition);
        }

        public CommandBuilder<TCommand> EndCondition => this._currentAction;
    }
}
