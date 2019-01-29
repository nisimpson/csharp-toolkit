// ObservableListCollection.cs
// Source: https://hk.saowen.com/a/feb0e32976646bf7e8f6fed14f37fdb31a7847d20596ebadd98a6dd31faea06b

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Nerdshoe
{
    /// <summary>
    /// <see cref="ObservableCollection{T}"/> implementation for updating
    /// and adding items.
    /// </summary>
    public class ObservableListCollection<T>
        : ObservableCollection<T>
        , IOrderable
    {
        private readonly IEqualityComparer<T> equivalenceComparer;
        private readonly Func<T, T, bool> updater;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableListCollection{T}"/> class.
        /// </summary>
        public ObservableListCollection()
        {
            equivalenceComparer = EqualityComparer<T>.Default; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableListCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection of elements to be added.</param>
        /// <exception cref="ArgumentNullException">
        /// The collection parameter cannot be null.
        /// </exception>
        public ObservableListCollection(IEnumerable<T> collection,
            IEqualityComparer<T> comparer = null,
            Func<T, T, bool> updateCallback = null) : base(collection)
        {
            equivalenceComparer = comparer ?? EqualityComparer<T>.Default;
            updater = updateCallback;
        }

        public void UpdateRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            CheckReentrancy();
            int startIndex = Count;

            var updated = collection.Where(item1 => {
                return Items.Any(item2 => equivalenceComparer.Equals(item1, item2));
            }).ToList();

            bool anyItemUpdated = false;

            foreach(T item in updated) {
                var existing = Items.FirstOrDefault(obj => {
                    return equivalenceComparer.Equals(item, obj);
                });

                // TODO: We can fire NotifyCollectionChanged.Update if needed
                // depending on anyItemUpdated.

                anyItemUpdated = anyItemUpdated |
                    updater?.Invoke(existing, item) ?? false;
            }

            IEnumerable<T> newItems = collection.Where(item => {
                return !Items.Any(item2 => equivalenceComparer.Equals(item, item2));
            });

            if (!newItems.Any()) {
                return;
            }

            foreach (T item in newItems) {
                Items.Add(item);
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                newItems,
                startIndex));
        }

        #region IOrderable

        public event EventHandler OrderChanged;

        public void ChangeOrdinal(int oldIndex, int newIndex)
        {
            T changed = Items[oldIndex];
            if (newIndex < oldIndex) {

                // add one to where we delete because we're increasing
                // the index by inserting
                oldIndex += 1;

            } else {

                // add one to where we insert, because we haven't deleted
                // the original
                newIndex += 1;
            }

            Items.Insert(newIndex, changed);
            Items.RemoveAt(oldIndex);

            OrderChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }

    public class ObservableListGroup<TKey, TItem>
        : ObservableListCollection<TItem>
    {
        public ObservableListGroup(TKey key,
            IEnumerable<TItem> collection,
            IEqualityComparer<TItem> equivalence = null,
            Func<TItem, TItem, bool> updater = null)
            : base(Enumerable.Empty<TItem>(), equivalence, updater)
        {
            Key = key;
            UpdateRange(collection);
        }

        public TKey Key { get; }
    }

    public class ObservableListGroupCollection<TKey, TItem>
        : ObservableListCollection<ObservableListGroup<TKey, TItem>>
    {
        private readonly Func<TItem, TKey> groupKeySelector;
        private readonly IEqualityComparer<TItem> equality;
        private readonly Func<TItem, TItem, bool> updater;

        public ObservableListGroupCollection(
            IEnumerable<TItem> collection,
            Func<TItem, TKey> keySelector,
            IEqualityComparer<TItem> equivalence = null,
            Func<TItem, TItem, bool> updateCallback = null)
        {
            groupKeySelector = keySelector;
            equality = equivalence;
            updater = updateCallback;

            UpdateItems(collection);
        }

        public void UpdateItems(IEnumerable<TItem> items)
        {
            // First we group the items coming down the pipeline
            var itemGroup = items.GroupBy(groupKeySelector);

            var startIndex = Count;

            var newItems = new List<ObservableListGroup<TKey, TItem>>();

            foreach (var group in itemGroup) {
                var section = Items.FirstOrDefault(item => item.Key.Equals(group.Key));

                if (section == null) {

                    // Create the observable group if it does not exist
                    var newGroup = new ObservableListGroup<TKey, TItem>(group.Key,
                        group,
                        equality,
                        updater);

                    newItems.Add(newGroup);
                    Items.Add(newGroup);

                } else {

                    // Update the range of the existing group
                    section.UpdateRange(group);
                }
            }

            if (newItems.Any()) {
                // Raise list change events if only new sections were added.
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    newItems,
                    startIndex));
            }
        }
    }
}
