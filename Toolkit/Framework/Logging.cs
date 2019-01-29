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

namespace Nerdshoe
{
    public interface ILogger
    {
        /// <summary>
        /// Logs an emergency message. System is unusable. A panic condition.
        /// </summary>
        /// <param name="message">The string message.</param>
        void Emergency(string message);

        /// <summary>
        /// Logs an emergency message. System is unusable. A panic condition.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message args.</param>
        void Emergency(string format, params object[] args);

        /// <summary>
        /// Logs a warning. Action must be taken immeadiately.
        /// </summary>
        /// <param name="message">The string message.</param>
        void Alert(string message);

        /// <summary>
        /// Logs a warning. Action must be taken immeadiately.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message args.</param>
        void Alert(string format, params object[] args);

        /// <summary>
        /// Logs a critical message.
        /// </summary>
        /// <param name="message">The string message.</param>
        void Critical(string message);

        /// <param name="format">The string format.</param>
        /// <param name="args">The message args.</param>
        void Critical(string format, params object[] args);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The string message.</param>
        void Error(string message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message args.</param>
        void Error(string format, params object[] args);

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="message">The string message.</param>
        void Warning(string message);

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message args.</param>
        void Warning(string format, params object[] args);

        /// <summary>
        /// Logs a notice. Normal but signifigant condition.
        /// </summary>
        /// <param name="message">The string message.</param>
        void Notice(string message);

        /// <summary>
        /// Logs a notice. Normal but signifigant condition.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message args.</param>
        void Notice(string format, params object[] args);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The string message.</param>
        void Info(string message);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message args.</param>
        void Info(string format, params object[] args);

        /// <summary>
        /// Logs a debug message. Contains information only useful for
        /// debugging purposes.
        /// </summary>
        /// <param name="message">The string message.</param>
        void Debug(string message);

        /// <summary>
        /// Logs a debug message. Contains information only useful for
        /// debugging purposes.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message args.</param>
        void Debug(string format, params object[] args);
    }

    /// <summary>
    /// A "No Operation" implementation of the logger interface.
    /// </summary>
    class NullLogger : ILogger
    {
        public void Alert(string message) { }
        public void Alert(string format, params object[] args) { }
        public void Critical(string message) { }
        public void Critical(string format, params object[] args) { }
        public void Debug(string message) { }
        public void Debug(string format, params object[] args) { }
        public void Emergency(string message) { }
        public void Emergency(string format, params object[] args) { }
        public void Error(string message) { }
        public void Error(string format, params object[] args) { }
        public void Info(string message) { }
        public void Info(string format, params object[] args) { }
        public void Notice(string message) { }
        public void Notice(string format, params object[] args) { }
        public void Warning(string message) { }
        public void Warning(string format, params object[] args) { }
    }
}
