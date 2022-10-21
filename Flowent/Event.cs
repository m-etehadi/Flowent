using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowent
{
    public class Event<TCommand> where TCommand : ICommand, new()
    {
        CommandBuilder<TCommand> _currentAction;
        List<Func<TCommand, Task>> _onExecuted;
        List<(Type exceptionType, Func<TCommand, Exception, Task> handler)> _onException;

        public CommandBuilder<TCommand> EndOn => this._currentAction;

        //Methods

        internal async Task Run(TCommand commandInstance)
        {
            try
            {
                await commandInstance.Execute();
            }
            catch (Exception ex)
            {
                _onException.Where(e => e.exceptionType.Equals(ex.GetType()))
                    .ToList()
                    .ForEach(async e => await e.handler(commandInstance, ex));
            }

            _onExecuted.ForEach(e => e(commandInstance));
        }

        internal Event(CommandBuilder<TCommand> currentAction)
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
