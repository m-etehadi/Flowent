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
        Func<TCommand, Exception> _action;


        public ValidatorAction(Validator<TCommand> validator)
        {
            _validator = validator;
        }


        public Validator<TCommand> Throw<TException>(Func<TCommand, TException> action) where TException : Exception
        {
            this._action = action;
            return this._validator;
        }

        public Validator<TCommand> Throw(Func<TCommand, Exception> action) => Throw<Exception>(action);

        public Exception Run(TCommand cmd) => _action(cmd);
        
    }
}
