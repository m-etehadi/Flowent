using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowent
{
    public class CommandBuilder
    {

    }

    public class CommandBuilder<TCommand> : CommandBuilder where TCommand : ICommand, new()
    {
        ConditionalBuilder<TCommand>? _conditionalNextAction;

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

        public CommandBuilder()
        {
            _initializer = new Initializer<TCommand>(this);
            _events = new Event<TCommand>(this);
            _validators = new Validator<TCommand>(this);
        }


        public ConditionalBuilder<TCommand> Then() => If(command => true);

        public ConditionalBuilder<TCommand> If(Func<TCommand, bool> condition)
        {
            this._conditionalNextAction = new ConditionalBuilder<TCommand>(this, condition);
            return this._conditionalNextAction;
        }

        public void Run()
        {
            var commandInstance = Init.Run();

            // validate
            var validatorException = _validators.Run(commandInstance);
            if (validatorException != null)
            {
                throw validatorException;
            }

            // run
            try
            {
                commandInstance.Execute();
            }
            catch (Exception ex)
            {

                throw;
            }


        }
    }
}
