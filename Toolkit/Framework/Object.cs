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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nerdshoe.Services;

namespace Nerdshoe
{
    /// <summary>
    /// Extends the base <see cref="Object"/> class with features such as logging
    /// and client notification of property updates.
    /// </summary>
    public abstract class BaseObject
        : INotifyPropertyChanged
        , INotifyPropertyChanging
    {
        static ILogger logger;
       
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when property changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Gets the log service.
        /// </summary>
        /// <value>The log service.</value>
        public static ILogger Log {
            get {
                if (logger == null) {
                    var service = IoC.Default.Resolve<ILogService>();
                    logger = service?.GetLogger() ?? new NullLogger();
                }
                return logger;
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected void NotifyPropertyChanged(
            [CallerMemberName]string propertyName = null)
        {
            var args = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected void NotifyPropertyChanging(
            [CallerMemberName]string propertyName = null)
        {
            var args = new PropertyChangingEventArgs(propertyName);
            PropertyChanging?.Invoke(this, args);
        }

        /// <summary>
        /// Sets the storage member to the specified value. If the member is
        /// updated, the <seealso cref="PropertyChanged" /> event is raised.
        /// </summary>
        /// <param name="store">The property storage member.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">
        /// The property name. Leave null when invoking with a property's setter.
        /// </param>
        /// <typeparam name="T">The property value type.</typeparam>
        /// <returns>
        /// <c>true</c> if the property value was updated; <c>false</c> otherwise.
        /// </returns>
        protected bool SetProperty<T>(ref T store,
            T value,
            [CallerMemberName] string propertyName = null)
        {
            bool result = true;
            if (ReferenceEquals(store, value))
            {
                result = false;
            }
            else
            {
                NotifyPropertyChanging(propertyName);
                store = value;
                NotifyPropertyChanged(propertyName);
                result = true;
            }
            return result;
        }
    }
}
