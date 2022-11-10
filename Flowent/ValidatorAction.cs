using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowent.Command;
using Flowent.Exceptions;
using static System.Collections.Specialized.BitVector32;

namespace Flowent
{
    public class ValidatorAction<TCommand> where TCommand : ICommand, new()
    {
        Validator<TCommand> _validator;
        Func<TCommand, Exception> _action;


        internal ValidatorAction(Validator<TCommand> validator)
        {
            _validator = validator;
            _action = cmd => new ValidatorException();
        }


        public Validator<TCommand> Throw<TException>(Func<TCommand, TException> action) where TException : Exception
        {
            _action = action;
            return _validator;
        }

        public Validator<TCommand> Throw(Func<TCommand, Exception> action) => Throw<Exception>(action);

        internal async Task<Exception> Run(TCommand cmd) => await Task.Run(() => _action(cmd));

    }
}
