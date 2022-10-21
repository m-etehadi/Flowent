using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Flowent
{
    public class Validator<TCommand> where TCommand : ICommand, new()
    {
        CommandBuilder<TCommand> _currentAction;
        private List<ValidatorActionPair> _validatorsIsNot;
        private List<ValidatorActionPair> _validatorsIs;


        public CommandBuilder<TCommand> EndValidate => _currentAction;



        internal Validator(CommandBuilder<TCommand> currentAction)
        {
            _currentAction = currentAction;
            _validatorsIsNot = new List<ValidatorActionPair>();
            _validatorsIs = new List<ValidatorActionPair>();
        }

        public ValidatorAction<TCommand> IfIsNot(params Func<TCommand, bool>[] validator)
        {
            var validatorActionPair = new ValidatorActionPair(validator, new ValidatorAction<TCommand>(this));
            _validatorsIsNot.Add(validatorActionPair);

            return validatorActionPair.Action;
        }
        public ValidatorAction<TCommand> If(params Func<TCommand, bool>[] validator)
        {
            var validatorActionPair = new ValidatorActionPair(validator, new ValidatorAction<TCommand>(this));
            _validatorsIs.Add(validatorActionPair);

            return validatorActionPair.Action;
        }

        public AggregateException? Run(TCommand cmd)
        {
            List<Exception> resultIsNotValidators = _validatorsIsNot.Where(p => p.Validators.Any(validator => validator(cmd) == false))
                .Select(p => p.Action.Run(cmd))
                .ToList();

            List<Exception> resultIsValidators = _validatorsIs.Where(p => p.Validators.Any(validator => validator(cmd) == false))
                .Select(p => p.Action.Run(cmd))
                .ToList();

            var validationExceptions = resultIsValidators.Union(resultIsNotValidators);

            return validationExceptions.Any() ? new AggregateException(validationExceptions) : null;
        }

        private class ValidatorActionPair
        {
            public Func<TCommand, bool>[] Validators { get; init; }
            public ValidatorAction<TCommand> Action { get; init; }

            public ValidatorActionPair(Func<TCommand, bool>[] validators, ValidatorAction<TCommand> action)
            {
                Validators = validators;
                Action = action;
            }
        }
    }
}
