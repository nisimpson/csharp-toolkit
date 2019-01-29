// Copyright(c) 2018 Nathan Simpson
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nerdshoe
{
    /// <summary>
    /// A command that can execute actions asynchronously.
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Execute the specified parameter.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        Task ExecuteAsync(object parameter);
    }

    /// <summary>
    /// Base implementation of <see cref="ICommand"/>.
    /// </summary>
    public abstract class Command
        : BaseObject
        , ICommand
    {
        string label;

        /// <summary>
        /// Occurs when can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label {
            get => Label;
            set => SetProperty(ref label, value);
        }

        /// <summary>
        /// Determines if this command can be executed with the given parameter.
        /// </summary>
        /// <returns><c>true</c>, if execution is permitted;
        /// <c>false</c> otherwise.</returns>
        /// <param name="parameter">The command parameter.</param>
        /// <param name="parameter">Parameter.</param>
        public abstract bool CanExecute(object parameter);
       
        /// <summary>
        /// Execute the specified parameter.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        public abstract void Execute(object parameter);
       
        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// A <see cref="Command"/> that invokes a specified
    /// <see cref="Action"/> upon execution.
    /// </summary>
    public class ActionCommand : Command
    {
        readonly Action<object> executeCmd;
        readonly Func<object, bool> canExecuteCmd;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:Nerdshoe.ActionCommand"/> class.
        /// </summary>
        /// <param name="execute">Execute.</param>
        /// <param name="canExecute">Can execute.</param>
        public ActionCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            executeCmd = execute
                ?? throw new ArgumentNullException(nameof(execute));

            canExecuteCmd = canExecute
                ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nerdshoe.ActionCommand"/> class.
        /// </summary>
        /// <param name="execute">Execute.</param>
        public ActionCommand(Action<object> execute)
            : this(obj => execute(obj), obj => true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nerdshoe.ActionCommand"/> class.
        /// </summary>
        /// <param name="execute">Execute.</param>
        /// <param name="canExecute">Can execute.</param>
        public ActionCommand(Action execute, Func<bool> canExecute)
            : this(obj => execute(), obj => canExecute()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nerdshoe.ActionCommand"/> class.
        /// </summary>
        /// <param name="execute">Execute.</param>
        public ActionCommand(Action execute)
            : this(obj => execute()) { }
            
        /// <summary>
        /// Cans the execute.
        /// </summary>
        /// <returns><c>true</c>, if execute was caned, <c>false</c> otherwise.</returns>
        /// <param name="parameter">Parameter.</param>
        public override bool CanExecute(object parameter)
        {
            return canExecuteCmd(parameter);
        }

        /// <summary>
        /// Execute the specified parameter.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        public override void Execute(object parameter)
        {
            executeCmd(parameter);
        }
    }

    /// <summary>
    /// A <see cref="Command"/> that invokes a specified
    /// <see cref="Action"/> upon execution.
    /// </summary>
    public class ActionCommand<T> : ActionCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nerdshoe.ActionCommand`1"/> class.
        /// </summary>
        /// <param name="execute">Execute.</param>
        /// <param name="canExecute">Can execute.</param>
        public ActionCommand(Action<T> execute, Func<T, bool> canExecute)
            : base(obj => execute((T)obj), obj => canExecute((T)obj))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nerdshoe.ActionCommand`1"/> class.
        /// </summary>
        /// <param name="execute">Execute.</param>
        public ActionCommand(Action<T> execute) : this(execute, _ => true) { }
    }

    /// <summary>
    /// A <see cref="Command"/> that performs an asynchronous <see cref="Task"/>
    /// upon execution.
    /// </summary>
    public class AsyncCommand
        : Command
        , IAsyncCommand
    {
        Func<object, Task> executeCmd;
        Func<object, bool> canExecuteCmd;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution action.</param>
        /// <param name="canExecute">The execution condition.</param>
        public AsyncCommand(Func<object, Task> execute,
            Func<object, bool> canExecute)
        {
            executeCmd = execute
                ?? throw new ArgumentNullException(nameof(execute));

            canExecuteCmd = canExecute
                ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution action.</param>
        public AsyncCommand(Func<object, Task> execute)
            : this(execute, _ => true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution action.</param>
        /// <param name="canExecute">The execution condition.</param>
        public AsyncCommand(Func<Task> execute, Func<bool> canExecute)
            : this(_ => execute(), _ => canExecute()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution action.</param>
        public AsyncCommand(Func<Task> execute)
            : this(_ => execute()) { }

        /// <summary>
        /// Determines if this command can be executed with the given parameter.
        /// </summary>
        /// <returns><c>true</c>, if execution is permitted;
        /// <c>false</c> otherwise.</returns>
        /// <param name="parameter">The command parameter.</param>
        public override bool CanExecute(object parameter)
        {
            return canExecuteCmd(parameter);
        }

        /// <summary>
        /// Execute the specified parameter.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        public override async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="parameter">Parameter.</param>
        public Task ExecuteAsync(object parameter)
        {
            return executeCmd(parameter);
        }
    }

    public class AsyncCommand<T> : AsyncCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nerdshoe.AsyncActionCommand`1"/> class.
        /// </summary>
        /// <param name="execute">Execute.</param>
        /// <param name="canExecute">Can execute.</param>
        public AsyncCommand(Func<T, Task> execute,
            Func<T, bool> canExecute)
            : base(obj => execute((T)obj), obj => canExecute((T)obj)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nerdshoe.AsyncActionCommand`1"/> class.
        /// </summary>
        /// <param name="execute">Execute.</param>
        public AsyncCommand(Func<T, Task> execute) : this(execute, _ => true) { }
    }
}
