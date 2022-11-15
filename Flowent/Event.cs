using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowent.Command;

namespace Flowent
{
    public class Event<TCommand> where TCommand : ICommand, new()
    {
        FlowBuilder<TCommand> _currentAction;
        List<Func<TCommand, Task>> _onExecuted;
        List<(Type exceptionType, Func<TCommand, Exception, Task> handler)> _onException;

        public FlowBuilder<TCommand> EndOn => this._currentAction;

        //Methods

        internal async Task Run(TCommand commandInstance)
        {
            try
            {
                await commandInstance.Execute();
                await Task.WhenAll(_onExecuted.Select(e => e(commandInstance)));
            }
            catch (Exception exception)
            {
                var exceptionHandlers = _onException.Where(e => e.exceptionType.IsAssignableFrom(exception.GetType())).Select(e => e.handler(commandInstance, exception))
                                 .Union(embeddedExceptions(commandInstance).Select(p => p(commandInstance, exception)));

                await Task.WhenAll(exceptionHandlers);
            }
        }

        private List<Func<TCommand, Exception, Task>> embeddedExceptions(TCommand cmdInstance)
        {
            List<Func<TCommand, Exception, Task>> result = new();
            ICommandExceptionHandler? cmdInstanceCommandHandler = cmdInstance as ICommandExceptionHandler;
            if (cmdInstanceCommandHandler != null)
                result.Add((TCommand command, Exception ex) => cmdInstanceCommandHandler.ExceptionHandler(ex));

            return result;
        }

        internal Event(FlowBuilder<TCommand> currentAction)
        {
            _currentAction = currentAction;
            _onExecuted = new List<Func<TCommand, Task>>();
            _onException = new List<(Type, Func<TCommand, Exception, Task>)>();

        }

        /// <summary>
        /// Assign a deligation to an exception type. In runtime we invoke all assigned deletions to an exception type
        /// </summary>
        /// <param name="exceptionAction"></param>
        /// <param name="rethrow">If "True", call throw command in exception handeling block </param>
        /// <returns></returns>
        public Event<TCommand> Exception(Func<TCommand, Exception, Task> exceptionAction)
        {
            this._onException.Add((typeof(Exception), exceptionAction));
            return this;
        }

        /// <summary>
        /// <see cref="Exception(Action{TCommand})"/>
        /// </summary>
        /// <typeparam name="ExceptionType"></typeparam>
        /// <param name="exceptionAction"></param>
        /// <returns></returns>
        public Event<TCommand> Exception<ExceptionType>(Func<TCommand, ExceptionType, Task> exceptionAction)
            where ExceptionType : Exception
        {
            Func<TCommand, Exception, Task> exceptionActionBase = (cmd, ex) => exceptionAction(cmd, (ExceptionType)ex);

            this._onException.Add((typeof(ExceptionType), exceptionActionBase));
            return this;
        }

        public Event<TCommand> Executed(params Func<TCommand, Task>[] completedAction)
        {
            this._onExecuted.AddRange(completedAction);
            return this;
        }

        public Event<TCommand> ExecutedAsync(params Action<TCommand>[] completedAction)
        {
            foreach (var action in completedAction)
            {
                Func<TCommand, Task> asyncAction = command => Task.Run(() => action(command));
                this.Executed(asyncAction);
            }

            return this;
        }


    }
}
