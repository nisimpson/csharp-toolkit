using System;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace Nerdshoe.iOS
{
    public class ListSortableTableSource : UITableViewSource
    {
        public ListSortableTableSource(UITableViewSource source,
            ListView element)
        {
            OriginalSource = source;
            FormsElement = element;
        }

        public UITableViewSource OriginalSource { get; }

        public ListView FormsElement { get; }

        // We do not want the "-" icon near each row...or the "+" icon)
        public override UITableViewCellEditingStyle EditingStyleForRow(
            UITableView tableView,
            NSIndexPath indexPath) => UITableViewCellEditingStyle.None;
            
        // Rows should be editable.
        public override bool CanEditRow(UITableView tableView,
            NSIndexPath indexPath) => true;

        // Rows should be movable.
        public override bool CanMoveRow(UITableView tableView,
            NSIndexPath indexPath) => true;

        // Avoid weird indent for rows when they are in edit mode.
        public override bool ShouldIndentWhileEditing(UITableView tableView,
            NSIndexPath indexPath) => false;

        public override void MoveRow(UITableView tableView,
            NSIndexPath sourceIndexPath,
            NSIndexPath destinationIndexPath)
        {
            if (FormsElement.ItemsSource is IOrderable orderable) {
                int sourceIndex = sourceIndexPath.Row;
                int targetIndex = destinationIndexPath.Row;
                orderable.ChangeOrdinal(sourceIndex, targetIndex);
            }
        }

        #region Proxy

        public override UITableViewCell GetCell(UITableView tableView,
            NSIndexPath indexPath)
        {
            return OriginalSource.GetCell(tableView, indexPath);
        }

        public override nfloat GetHeightForHeader(UITableView tableView,
            nint section)
        {
            return OriginalSource.GetHeightForHeader(tableView, section);
        }

        public override UIView GetViewForHeader(UITableView tableView,
            nint section)
        {
            return OriginalSource.GetViewForHeader(tableView, section);
        }

        public override void HeaderViewDisplayingEnded(UITableView tableView,
            UIView headerView,
            nint section)
        {
            OriginalSource.HeaderViewDisplayingEnded(tableView,
                headerView,
                section);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return OriginalSource.NumberOfSections(tableView);
        }

        public override void RowDeselected(UITableView tableView,
            NSIndexPath indexPath)
        {
            OriginalSource.RowDeselected(tableView, indexPath);
        }

        public override void RowSelected(UITableView tableView,
            NSIndexPath indexPath)
        {
            OriginalSource.RowSelected(tableView, indexPath);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return OriginalSource.RowsInSection(tableview, section);
        }

        [Export("scrollViewDidScroll:")]
        public override void Scrolled(UIScrollView scrollView)
        {
            OriginalSource.Scrolled(scrollView);
        }

        public override string[] SectionIndexTitles(UITableView tableView)
        {
            return OriginalSource.SectionIndexTitles(tableView);
        }

        #endregion
    }
}
