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
        public readonly record struct ValidatorActionPair(Func<TCommand, bool>[] Validators, ValidatorAction<TCommand> Action)
        { 
            
        }


        CommandBuilder<TCommand> _currentAction;
        private List<ValidatorActionPair<TCommand>> _validatorsIsNot;
        private List<ValidatorActionPair<TCommand>> _validatorsIs;
        

        public CommandBuilder<TCommand> EndValidate => _currentAction;



        internal Validator(CommandBuilder<TCommand> currentAction)
        {
            _currentAction = currentAction;
            _validatorsIsNot = new List<ValidatorActionPair<TCommand>>();
            _validatorsIs = new List<ValidatorActionPair<TCommand>>();
        }

        public ValidatorAction<TCommand> IfIsNot(params Func<TCommand, bool>[] validator)
        {
            var validatorActionPair = new ValidatorActionPair<TCommand>(validator, new ValidatorAction<TCommand>(this));
            _validatorsIsNot.Add(validatorActionPair);

            return validatorActionPair.Action;
        }
        public ValidatorAction<TCommand> If(params Func<TCommand, bool>[] validator)
        {
            var validatorActionPair = new ValidatorActionPair<TCommand>(validator, new ValidatorAction<TCommand>(this));
            _validatorsIs.Add(validatorActionPair);

            return validatorActionPair.Action;
        }

        public AggregateException Run(TCommand cmd)
        {
            List<Exception> resultIsNotValidators = _validatorsIsNot.Where(p => p.Validators.Any(validator => validator(cmd) == false))
                .Select(p => p.Action.Run(cmd))
                .ToList();

            List<Exception> resultIsValidators = _validatorsIs.Where(p => p.Validators.Any(validator => validator(cmd) == false))
                .Select(p => p.Action.Run(cmd))
                .ToList();

            return new AggregateException(resultIsValidators.Union(resultIsNotValidators));
        }

        private class ValidatorActionPair<TCommand> where TCommand : ICommand, new()
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
