using System;

namespace Nerdshoe
{
    /// <summary>
    /// Used by bound collections to expose ordering methods and events.
    /// </summary>
    public interface IOrderable
    {
        /// <summary>
        /// Event fired when items in the collection are re-ordered.
        /// </summary>
        event EventHandler OrderChanged;

        /// <summary>
        /// Used to change the item orders in an <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="oldIndex">The old index of an item.</param>
        /// <param name="newIndex">The new index of an item.</param>
        void ChangeOrdinal(int oldIndex, int newIndex);
    }
}