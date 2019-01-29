using System;
namespace Nerdshoe
{
    public interface IApplication
    {
        /// <summary>
        /// Gets the property or default value
        /// </summary>
        /// <returns>The property or default.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="T">The property value type.</typeparam>
        T GetStateOrDefault<T>(string key, T defaultValue);

        /// <summary>
        /// Gets the property or default value
        /// </summary>
        /// <returns>The property or default.</returns>
        /// <param name="key">Key.</param>
        /// <typeparam name="T">The property value type.</typeparam>
        T GetStateOrDefault<T>(string key);

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The property value type.</typeparam>
        void SetState<T>(string key, T value);
    }
}
