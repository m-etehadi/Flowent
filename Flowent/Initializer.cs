using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowent
{
    public class Initializer<TCommand> where TCommand : ICommand, new()
    {
        Func<TCommand> _actionInitializer;
        readonly CommandBuilder<TCommand> _currentCommandBuilder;

        // Methods
        public Initializer(CommandBuilder<TCommand> currentCommandBuilder)
        {
            _currentCommandBuilder = currentCommandBuilder;
            _actionInitializer = () => new TCommand();
        }

        public CommandBuilder<TCommand> By(params Action<TCommand>[] initializers)
        {
            this._actionInitializer = () =>
            {
                var result = new TCommand();
                initializers.ToList().ForEach(p => p.Invoke(result));
                return result;
            };

            return _currentCommandBuilder;
        }

        public CommandBuilder<TCommand> By(Func<TCommand> initializer)
        {
            this._actionInitializer = initializer;
            return _currentCommandBuilder;
        }

        public TCommand Run() => _actionInitializer();
    }
}
