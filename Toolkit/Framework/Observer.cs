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
using System.Collections.Generic;

namespace Nerdshoe
{
    /// <summary>
    /// Base implementation of the observer-side of the Observer pattern.
    /// </summary>
    public abstract class Observer<T>
        : BaseObject
        , IObserver<T>
    {
        readonly IDictionary<IObservable<T>, IDisposable> subscriptions =
            new Dictionary<IObservable<T>, IDisposable>();

        /// <summary>
        /// Subscribe the specified subject.
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// <param name="subject">Subject.</param>
        public IDisposable Subscribe(IObservable<T> subject)
        { 
            var subscription = subject.Subscribe(this);
            subscriptions.AddOrUpdate(subject, subscription);
            return subscription;
        }

        public void Unsubscribe(IObservable<T> subject)
        {
            var subscription = subscriptions.GetValueOrDefault(subject);
            subscription?.Dispose();
            subscriptions.Remove(subject);
        }

        /// <summary>
        /// Notifies the observer that the provider has finished
        /// sending push-based notifications.
        /// </summary>
        public abstract void OnCompleted();

        /// <summary>
        /// Notifies the observer that the provider has experienced
        /// an error condition.
        /// </summary>
        /// <param name="error">Error.</param>
        public abstract void OnError(Exception error);

        /// <summary>
        /// Provides the observer with new data.
        /// </summary>
        /// <param name="value">Value.</param>
        public abstract void OnNext(T value);

    }

    /// <summary>
    /// An observer that invokes async delegates when notified by an
    /// <seealso cref="IObservable{T}"/>.
    /// </summary>
    class AsyncObserver<T> : Observer<T>
    {
        readonly Func<T, Task> next;
        readonly Func<Task> completed;
        readonly Func<Exception, Task> error;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncObserver{T}"/> class.
        /// </summary>
        /// <param name="next">callback for observer updates.</param>
        /// <param name="completed">callback for notification completion.</param>
        /// <param name="error">callback for error conditions.</param>
        public AsyncObserver(Func<T, Task> next,
            Func<Task> completed,
            Func<Exception, Task> error)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.completed = completed ?? throw new ArgumentNullException(nameof(completed));
            this.error = error ?? throw new ArgumentNullException(nameof(error));
        }

        public override async void OnCompleted()
        {
            await OnCompletedAsync();
        }

        public override async void OnError(Exception error)
        {
            await OnErrorAsync(error);
        }

        public override async void OnNext(T value)
        {
            await OnNextAsync(value);
        }

        public Task OnNextAsync(T value) => next(value);
        public Task OnCompletedAsync() => completed();
        public Task OnErrorAsync(Exception e) => error(e);
    }

    /// <summary>
    /// An observer that invokes delegates when notified by an
    /// <seealso cref="IObservable{T}"/>.
    /// </summary>
    class ActionObserver<T> : AsyncObserver<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionObserver{T}"/> class.
        /// </summary>
        /// <param name="next">callback for observer updates.</param>
        /// <param name="completed">callback for notification completion.</param>
        /// <param name="error">callback for error conditions.</param>
        public ActionObserver(Action<T> next,
            Action completed,
            Action<Exception> error)
            : base(o => ToTask(next, o), () => ToCompletedTask(completed), e => ToTask(error, e)) {}

        /// <summary>
        /// Wraps the action in a Task.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="action">Action.</param>
        /// <param name="value">Value.</param>
        protected static Task ToTask<U>(Action<U> action, U value)
        {
            try { 
                action(value);
                return Task.CompletedTask;

            } catch (Exception e){
                return Task.FromException(e);
            }
        }

        /// <summary>
        /// Wraps the action in a Task.
        /// </summary>
        /// <returns>The completed task.</returns>
        /// <param name="action">Action.</param>
        static Task ToCompletedTask(Action action)
        {
            try {
                action();
                return Task.CompletedTask;

            } catch (Exception e) {
                return Task.FromException(e);
            }
        }
    }

    /// <summary>
    /// Simple implementation of the provider-side of the Observer pattern.
    /// </summary>
    public class Observable<T>
        : BaseObject
        , IObservable<T>
    {
        readonly List<IObserver<T>> observers = new List<IObserver<T>>();

        /// <summary>
        /// Notifies the provider that an observer is to receive notifications.
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows observers to stop
        /// receiving notifications before the provider has finished sending
        /// them.
        /// </returns>
        /// <param name="observer">Observer.</param>
        public IDisposable Subscribe(IObserver<T> observer) => Register(observer);

        /// <summary>
        /// Publishes the specified data to subscribed observers.
        /// </summary>
        /// <returns>The publish.</returns>
        /// <param name="data">Data.</param>
        public async Task Publish(T data)
        {
            foreach (var observer in observers) {
                if (observer is AsyncObserver<T>) {
                    await (observer as AsyncObserver<T>).OnNextAsync(data);
                } else {
                    observer.OnNext(data);
                }
            }
        }

        /// <summary>
        /// Ends this subject's activity, notifying all subscribed observers
        /// and removing all subscriptions.
        /// </summary>
        /// <returns>The shutdown.</returns>
        public async Task Shutdown()
        {
            foreach (var observer in observers) {
                if (observer is AsyncObserver<T>) {
                    await (observer as AsyncObserver<T>).OnCompletedAsync();
                } else {
                    observer.OnCompleted();
                }
            }
            observers.Clear();
        }

        /// <summary>
        /// Reports an error to all subscribed observers. 
        /// </summary>
        /// <returns>The error.</returns>
        /// <param name="error">Error.</param>
        public async Task Error(Exception error)
        {
            foreach (var observer in observers) {
                if (observer is AsyncObserver<T>) {
                    await (observer as AsyncObserver<T>).OnErrorAsync(error);
                } else {
                    observer.OnError(error);
                }
            }
        }

        /// <summary>
        /// Registers the specified observer to this instance.
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="observer">Observer.</param>
        protected virtual IDisposable Register(IObserver<T> observer)
        {
            if (!observers.Contains(observer)) {
                observers.Add(observer);
            }
            return new Subscription(observer, this);
        }

        /// <summary>
        /// Unregisters the specified observer from this instance.
        /// </summary>
        /// <param name="observer">Observer.</param>
        protected void Unregister(IObserver<T> observer) => observers.Remove(observer);

        /// <summary>
        /// Encapsulates the registration of an <see cref="IObserver{T}"/> to
        /// an <see cref="IObservable{T}"/>. When this object is disposed of,
        /// The observer is unregistered from the subject (observer).
        /// </summary>
        sealed class Subscription : IDisposable
        {
            readonly IObserver<T> observer;
            readonly Observable<T> subject;

            /// <summary>
            /// Initializes a new instance of the 
            /// <see cref="T:Nerdshoe.Observable`1.Subscription"/> class.
            /// </summary>
            /// <param name="observer">Observer.</param>
            /// <param name="subject">Subject.</param>
            public Subscription(IObserver<T> observer, Observable<T> subject)
            {
                this.observer = observer;
                this.subject = subject;
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            private void Dispose(bool disposing)
            {
                if (!disposedValue) {
                    if (disposing) subject.Unregister(observer);
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in
                // Dispose(bool disposing) above.
                Dispose(true);
            }
            #endregion
        }
    }

    public static class ObserverExtensions
    {
        /// <summary>
        /// Subscribes to the subject.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> that, when disposed, allows observers
        /// to stop receiving notifications from the subject.
        /// </returns>
        /// <param name="subject">Subject.</param>
        /// <param name="next">Data notification handler.</param>
        /// <param name="completed">Completed notification handler.</param>
        /// <param name="error">Error notification parameter.</param>
        /// <typeparam name="T">The subject data type.</typeparam>
        public static IDisposable Subscribe<T>(this object obj,
            IObservable<T> subject,
            Action<T> next,
            Action completed = null,
            Action<Exception> error = null)
        {
            var observer = new ActionObserver<T>(next,
                completed ?? new Action(() => {}),
                error ?? new Action<Exception>(_ => {}));

            return subject.Subscribe(observer);
        }

        /// <summary>
        /// Subscribes to the subject.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> that, when disposed, allows observers
        /// to stop receiving notifications from the subject.
        /// </returns>
        /// <param name="subject">Subject.</param>
        /// <param name="next">Data notification handler.</param>
        /// <param name="completed">Completed notification handler.</param>
        /// <param name="error">Error notification parameter.</param>
        /// <typeparam name="T">The subject data type.</typeparam>
        public static IDisposable Subscribe<T>(this object obj,
            IObservable<T> subject,
            Func<T, Task> next,
            Func<Task> completed = null,
            Func<Exception, Task> error = null)
        {
            var observer = new AsyncObserver<T>(next,
                completed ?? new Func<Task>(() => Task.CompletedTask),
                error ?? new Func<Exception, Task>((e) => Task.FromResult(e)));

            return subject.Subscribe(observer);
        }
    }
}