using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Flowent.Command;
using Microsoft.VisualBasic;

namespace Flowent
{
    public class Validator<TCommand> where TCommand : ICommand, new()
    {
        record class ValidatorActionPair(Func<TCommand, Task<bool>>[] Validators, ValidatorAction<TCommand> Action)
        {
            public async Task<Exception?> RunActionIfValidatorReturnsTrue(TCommand cmd) => await runActionIf(cmd, true);
            public async Task<Exception?> RunActionIfValidatorReturnsFalse(TCommand cmd) => await runActionIf(cmd, false);

            async Task<Exception?> runActionIf(TCommand cmd, bool validatorResult)
            {
                var validationsResult = await Task.WhenAll(Validators.Select(v => v.Invoke(cmd)));
                if (validationsResult.Any(p => p == validatorResult))
                    return await Action.Run(cmd);
                else
                    return null;
            }
        }

        FlowBuilder<TCommand> _currentAction;
        private List<ValidatorActionPair> _validatorsIsNot;
        private List<ValidatorActionPair> _validatorsIs;


        public FlowBuilder<TCommand> EndValidate => _currentAction;



        internal Validator(FlowBuilder<TCommand> currentAction)
        {
            _currentAction = currentAction;
            _validatorsIsNot = new List<ValidatorActionPair>();
            _validatorsIs = new List<ValidatorActionPair>();
        }

        public ValidatorAction<TCommand> IfIsNot(params Func<TCommand, bool>[] validators)
        {
            var validatorActionPair = new ValidatorActionPair(convertToAsyncDelegate(validators), new ValidatorAction<TCommand>(this));
            _validatorsIsNot.Add(validatorActionPair);

            return validatorActionPair.Action;
        }

        public ValidatorAction<TCommand> If(params Func<TCommand, bool>[] validators)
        {
            var validatorActionPair = new ValidatorActionPair(convertToAsyncDelegate(validators), new ValidatorAction<TCommand>(this));
            _validatorsIs.Add(validatorActionPair);

            return validatorActionPair.Action;
        }
        private static Func<TCommand, Task<bool>>[] convertToAsyncDelegate(Func<TCommand, bool>[] validators)
        {
            // convert to async call
            return validators.Select(validator =>
            {
                Func<TCommand, Task<bool>> result = cmd => Task.Run<bool>(() => validator(cmd));
                return result;
            }).ToArray();
        }

        /// <summary>
        /// Validates any configured Validators and returns <see cref="AggregateException"/> if any of them are invalid
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal async Task<AggregateException?> Run(TCommand cmd)
        {

            var resultIsNotValidators = await Task.WhenAll(_validatorsIsNot.Select(p => p.RunActionIfValidatorReturnsFalse(cmd)));
            var resultIsValidators = await Task.WhenAll(_validatorsIs.Select(p => p.RunActionIfValidatorReturnsTrue(cmd)));


            var validationExceptions = resultIsValidators.Union(resultIsNotValidators).Where(p => p != null)
                                                                     .Cast<Exception>().ToList();

            var icommandValidatorResult = await runCommandEmbeddedValidators(cmd);
            if (icommandValidatorResult != null)
                validationExceptions.AddRange(icommandValidatorResult.InnerExceptions);

            return validationExceptions.Any() ? new AggregateException(validationExceptions) : null;
        }


        private async Task<AggregateException?> runCommandEmbeddedValidators(TCommand command)
        {
            var commandValidator = command as ICommandValidator;
            if (commandValidator != null)
                return await commandValidator.Validate();
            else
                return null;
        }
    }
}
