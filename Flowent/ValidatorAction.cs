using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowent
{
    public class ValidatorAction<TCommand> where TCommand : ICommand, new()
    {
        Validator<TCommand> _validator;
        Func<TCommand, Exception>? _action;


        internal ValidatorAction(Validator<TCommand> validator)
        {
            _validator = validator;
        }


        public Validator<TCommand> Throw<TException>(Func<TCommand, TException> action) where TException : Exception
        {
            _action = action;
            return _validator;
        }

        public Validator<TCommand> Throw(Func<TCommand, Exception> action) => Throw<Exception>(action);

        internal Exception Run(TCommand cmd) => _action?.Invoke(cmd) ?? throw new Exception("Invalid ValidatorAction instance. Specify a valid action for current VlidatorAction instance.");
        
    }
}
