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
        Func<TCommand?, Task<TCommand>> _actionInitializer;
        readonly FlowBuilder<TCommand> _currentCommandBuilder;

        // Methods
        internal Initializer(FlowBuilder<TCommand> currentCommandBuilder)
        {
            _currentCommandBuilder = currentCommandBuilder;
            _actionInitializer = cmd => Task.Run<TCommand>(() => cmd ?? new TCommand());
        }

        public FlowBuilder<TCommand> By(params Action<TCommand>[] initializers)
        {
            this._actionInitializer = async cmd =>
            {
                cmd ??= new TCommand();
                var initializersInvokes = initializers.ToList().Select(p => Task.Run(() => p(cmd)));
                await Task.WhenAll(initializersInvokes);
                return cmd;
            };

            return _currentCommandBuilder;
        }

        public FlowBuilder<TCommand> By(Func<TCommand> initializer)
        {
            this._actionInitializer = cmd => Task.Run<TCommand>(() => initializer());
            return _currentCommandBuilder;
        }

        public FlowBuilder<TCommand> By(Func<TCommand?, TCommand> initializer)
        {
            this._actionInitializer = cmd => Task.Run<TCommand>(() => initializer(cmd));
            return _currentCommandBuilder;
        }

        internal async Task<TCommand> Run(TCommand? cmd = default)
        {
            var command = await _actionInitializer(cmd);
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
