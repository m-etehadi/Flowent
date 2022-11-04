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
        readonly FlowBuilder<TCommand> _currentCommandBuilder;

        // Methods
        public Initializer(FlowBuilder<TCommand> currentCommandBuilder)
        {
            _currentCommandBuilder = currentCommandBuilder;
            _actionInitializer = () => new TCommand();
        }

        public FlowBuilder<TCommand> By(params Action<TCommand>[] initializers)
        {
            this._actionInitializer = () =>
            {
                var result = new TCommand();
                initializers.ToList().ForEach(p => p.Invoke(result));
                return result;
            };

            return _currentCommandBuilder;
        }

        public FlowBuilder<TCommand> By(Func<TCommand> initializer)
        {
            this._actionInitializer = initializer;
            return _currentCommandBuilder;
        }

        internal TCommand Run() => _actionInitializer();
    }
}
