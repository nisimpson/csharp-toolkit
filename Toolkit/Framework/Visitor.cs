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

namespace Nerdshoe.Toolkit
{
    /// <summary>
    /// An object that can accept a <see cref="Visitor"/>.
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="visitor">The visitor.</param>
        /// <returns>
        /// <c>true</c> if the subject was visited; <c>false</c> otherwise.
        /// </returns>
        bool AcceptVisitor(Visitor visitor);

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="visitor">The visitor.</param>
        /// <returns>
        /// A <see cref="Task" /> object representing an async visit operation.
        /// <seealso cref="Visitor{T}.VisitAsync" /> for details.
        /// </returns>
        Task<bool> AcceptVisitorAsync(Visitor visitor);
    }

    /// <summary>
    /// Defines an object that implements an operation to be performed
    /// on an <see cref="IVisitable" /> element of the defined type.
    /// </summary>
    /// <typeparam name="T">The defined type.</typeparam>
    public interface IVisitor<T> where T : IVisitable
    {
        /// <summary>
        /// Performs an operation on the specified object.
        /// </summary>
        /// <param name="visitable">The subject.</param>
        /// <returns>
        /// <c>true</c> if the operation was successful; <c>false</c> otherwise.
        /// </returns>
        bool Visit(T visitable);

        /// <summary>
        /// Performs an async operation on the specified object.
        /// </summary>
        /// <param name="visitable">The subject.</param>
        /// <returns>
        /// A <see cref="Task" /> object representing the async visit operation.
        /// </returns>
        Task<bool> VisitAsync(T visitable);
    }

    /// <summary>
    /// Wrapper class for <see cref="IVisitor{T}" /> objects.
    /// </summary>
    public sealed class Visitor
    {
        readonly object wrapped;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Visitor"/> class.
        /// </summary>
        /// <param name="wrapped">
        /// The wrapped <seealso cref="IVisitor{T}" /> object.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The wrapped parameter was null.
        /// </exception>
        internal Visitor(object wrapped)
        {
            this.wrapped = wrapped 
                ?? throw new ArgumentNullException(nameof(wrapped));
        }

        /// <summary>
        /// Performs an operation on the subject if and only if
        /// the wrapped visitor can perform operations on the subject.
        /// </summary>
        /// <param name="visitable">The subject.</param>
        /// <typeparam name="T">The subject type.</typeparam>
        /// <returns>
        /// <c>true</c> if the operation was successful; <c>false</c> otherwise.
        /// </returns>
        public bool Visit<T>(T subject) where T : IVisitable
        {
            var visitor = wrapped as IVisitor<T>;
            var visited = visitor?.Visit(subject);
            return visited.GetValueOrDefault(false);
        }

        /// <summary>
        /// Performs an async operation on the subject if and only if
        /// the wrapped visitor can perform operations on the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <typeparam name="T">The subject type.</typeparam>
        /// <returns>
        /// A <see cref="Task" /> object representing the async visit operation.
        /// </returns>
        public Task<bool> VisitAsync<T>(T subject) where T : IVisitable
        {
            var visitor = wrapped as IVisitor<T>;
            if (visitor != null)
            {
                try {
                    return visitor.VisitAsync(subject);
                } catch (Exception ex) {
                    return Task.FromException<bool>(ex);
                }
            }
            return Task.FromResult(false);
        }
    }

    public static class VisitorExtensions
    {
        /// <summary>
        /// Wraps and accepts the specified visitor.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="visitor">The visitor.</param>
        /// <returns>
        /// <c>true</c> if the subject was visited; <c>false</c> otherwise.
        /// </returns>
        public static bool AcceptVisitor(this IVisitable subject,
            object visitor) => subject.AcceptVisitor(new Visitor(visitor));

        /// <summary>
        /// Wraps and accepts the specified visitor.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="visitor">The visitor.</param>
        /// <returns>
        /// A <see cref="Task" /> object representing the async visit operation.
        /// </returns>
        /// <seealso cref="Visitor" />
        public static Task<bool> AcceptVisitorAsync(this IVisitable subject,
            object visitor) => subject.AcceptVisitorAsync(new Visitor(visitor));
    }
}
