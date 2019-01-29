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
    /// <summary>
    /// An object used in <seealso cref="IoC"/> to generate instances
    /// of dependency types with concrete implementations. 
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Registers a dependency type with its implementation.
        /// </summary>
        /// <param name="lifecycle">Lifecycle.</param>
        /// <typeparam name="TResolve">
        /// The dependency type, usually an interface type.</typeparam>
        /// <typeparam name="TConcrete">
        /// The concrete type, usually an interface implementation.
        /// </typeparam>
        void Register<TResolve, TConcrete>(ObjectLifecycle lifecycle)
            where TResolve : class
            where TConcrete : class, TResolve;

        /// <summary>
        /// Resolves the dependency type with a registered implementation.
        /// </summary>
        /// <returns>The resolve.</returns>
        /// <typeparam name="TResolve">
        /// The dependency type, usually an interface.
        /// </typeparam>
        TResolve Resolve<TResolve>() where TResolve : class;
    }

    /// <summary>
    /// A simple Inversion of Control (IoC) implementation.
    /// Uses a <see cref="IDependencyResolver"/> to generate 
    /// dependencies during runtime.
    /// </summary>
    public static class IoC
    {
        static IDependencyResolver container;

        /// <summary>
        /// Gets or sets the default dependency resolver.
        /// </summary>
        /// <value>The default.</value>
        public static IDependencyResolver Default {
            get => container ?? throw new System.Exception("Create IoC container.");
            set => container = value;
        } 
    }

    /// <summary>
    /// Defines the scope in which resolved dependencies will live.
    /// </summary>
    public enum ObjectLifecycle
    {
        /// <summary>
        /// Persists in the scope of the calling function. Subsequent calls
        /// to <see cref="IDependencyResolver.Resolve{TResolve}"/> will create
        /// a new instance.
        /// </summary>
        New,

        /// <summary>
        /// Persists throughout the lifetime of the application.
        /// </summary>
        Singleton
    }
}
