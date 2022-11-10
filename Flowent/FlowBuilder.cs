using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowent.Command;

namespace Flowent
{
    public abstract class FlowBuilder
    {
        public abstract Task Run(ICommand? cmd = default);
    }

    public class FlowBuilder<TCommand> : FlowBuilder where TCommand : ICommand, new()
    {
        List<ConditionBuilder<TCommand>> _conditionalNextActions;
        Initializer<TCommand> _initializer;
        Event<TCommand> _events;
        Validator<TCommand> _validators;

        public Initializer<TCommand> Init
        {
            get => this._initializer;
        }
        public Event<TCommand> On
        {
            get => this._events;
        }
        public Validator<TCommand> Validate
        {
            get => this._validators;
        }


        // Methods

        public FlowBuilder()
        {
            _conditionalNextActions = new List<ConditionBuilder<TCommand>>();
            _initializer = new Initializer<TCommand>(this);
            _events = new Event<TCommand>(this);
            _validators = new Validator<TCommand>(this);
        }

        /// <summary>
        /// Execute provided flow imidiatly after successfull exection
        /// </summary>
        /// <returns></returns>
        public ConditionBuilder<TCommand> Then() => If(command => true);

        public ConditionBuilder<TCommand> If(Func<TCommand, bool> condition)
        {
            Func<TCommand, Task<bool>> asyncCondition = command => Task.Run<bool>(() => condition(command));
            var conditionalNextAction = new ConditionBuilder<TCommand>(this, asyncCondition);

            _conditionalNextActions.Add(conditionalNextAction);

            return conditionalNextAction;
        }

        public override async Task<TCommand> Run(ICommand? cmd = default)
        {
            if (cmd != default && !cmd.GetType().Equals(typeof(TCommand)))
                throw new Exception($"Provided invalid command type: {cmd.GetType().FullName}");

            var commandInstance = await Init.Run((TCommand?)cmd);

            // validate
            var validatorException = await _validators.Run(commandInstance);
            if (validatorException != null)
            {
                throw validatorException;
            }

            // run
            await On.Run(commandInstance);

            // conditions
            Task.WaitAll(_conditionalNextActions.Select(p => p.Run(commandInstance)).ToArray());

            return commandInstance;

        }
    }
}
