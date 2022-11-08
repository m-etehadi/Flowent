using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowent.Command;

namespace Flowent
{
    public class Initializer<TCommand> where TCommand : ICommand, new()
    {
        Func<Task<TCommand>> _actionInitializer;
        readonly FlowBuilder<TCommand> _currentCommandBuilder;

        // Methods
        internal Initializer(FlowBuilder<TCommand> currentCommandBuilder)
        {
            _currentCommandBuilder = currentCommandBuilder;
            _actionInitializer = () => Task.Run<TCommand>(() => new TCommand());
        }

        public FlowBuilder<TCommand> By(params Action<TCommand>[] initializers)
        {
            this._actionInitializer = async () =>
            {
                var result = new TCommand();
                var initializersInvokes = initializers.ToList().Select(p => Task.Run(() => p(result)));
                await Task.WhenAll(initializersInvokes);
                return result;
            };

            return _currentCommandBuilder;
        }

        public FlowBuilder<TCommand> By(Func<TCommand> initializer)
        {
            this._actionInitializer = () => Task.Run<TCommand>(() => initializer());
            return _currentCommandBuilder;
        }

        internal async Task<TCommand> Run()
        {
            var command = await _actionInitializer();
            await runCommandInitializer(command);
            return command;
        }

        private async Task runCommandInitializer(TCommand command)
        {
            var commandInitializer = command as ICommandInitializer;
            if (commandInitializer != null)
                await commandInitializer.Initialize();
        }
    }
}
