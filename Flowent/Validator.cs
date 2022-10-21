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
        private List<ValidatorValidatorActionPair<TCommand>> _validatorsIsNot;
        private List<ValidatorValidatorActionPair<TCommand>> _validatorsIs;
        

        public CommandBuilder<TCommand> EndValidate => _currentAction;



        internal Validator(CommandBuilder<TCommand> currentAction)
        {
            _currentAction = currentAction;
            _validatorsIsNot = new List<ValidatorValidatorActionPair<TCommand>>();
            _validatorsIs = new List<ValidatorValidatorActionPair<TCommand>>();
        }

        public ValidatorAction<TCommand> IfIsNot(params Func<TCommand, bool>[] validator)
        {
            var validatorValidatorActionPair = new ValidatorValidatorActionPair<TCommand>(validator, new ValidatorAction<TCommand>(this));
            _validatorsIsNot.Add(validatorValidatorActionPair);

            return validatorValidatorActionPair.Action;
        }
        public ValidatorAction<TCommand> If(params Func<TCommand, bool>[] validator)
        {
            var validatorValidatorActionPair = new ValidatorValidatorActionPair(validator, new ValidatorAction<TCommand>(this));
            _validatorsIs.Add(validatorValidatorActionPair);

            return validatorValidatorActionPair.Action;
        }

        public AggregateException Run(TCommand cmd)
        {

            var a=_validatorsIsNot.Select(p => Task.Run<bool>(() => p.Validators(cmd)));
            List<Exception> resultIsNotValidators = _validatorsIsNot.Where(p => p.Validators.Any(validator => validator(cmd) == false))
                .Select(p => p.Action.Run(cmd))
                .ToList();

            List<Exception> resultIsValidators = _validatorsIs.Where(p => p.Validators.Any(validator => validator(cmd) == false))
                .Select(p => p.Action.Run(cmd))
                .ToList();

            return new AggregateException(resultIsValidators.Union(resultIsNotValidators));
        }

        private class ValidatorValidatorActionPair<TCommand> where TCommand : ICommand, new()
        {
            public Func<TCommand, bool>[] Validators { get; init; }
            public ValidatorAction<TCommand> Action { get; init; }

            public ValidatorValidatorActionPair(Func<TCommand, bool>[] validators, ValidatorAction<TCommand> action)
            {
                Validators = validators;
                Action = action;
            }
        }
    }
}
