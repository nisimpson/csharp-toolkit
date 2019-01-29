using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Nerdshoe.iOS;

[assembly:ExportEffect(typeof(ListViewSortableEffect), "ListViewSortableEffect")]
namespace Nerdshoe.iOS
{
    public class ListViewSortableEffect : PlatformEffect
    {
        internal UITableView TableView => Control as UITableView;

        protected override void OnAttached()
        {
            if (TableView != null) {
                var isSortable = ListViewSorting.GetIsSortable(Element);

                TableView.Source = new ListSortableTableSource(
                    source: TableView.Source,
                    element: Element as ListView);

                TableView.SetEditing(isSortable, animated: true);
            }
        }

        protected override void OnDetached()
        {
            var source = TableView.Source as ListSortableTableSource;
            TableView.Source = source?.OriginalSource;
            TableView.SetEditing(false, animated: true);
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            string propertyName = ListViewSorting.IsSortableProperty.PropertyName;
            if (args.PropertyName == propertyName) {
                TableView.SetEditing(
                    ListViewSorting.GetIsSortable(Element),
                    animated: true);
            }
        }
    }
}
