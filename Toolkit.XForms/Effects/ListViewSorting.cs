using System.Linq;
using Xamarin.Forms;

namespace Nerdshoe
{
    public static class ListViewSorting
    {
        public static readonly BindableProperty IsSortableProperty =
        BindableProperty.CreateAttached("IsSortable",
            typeof(bool),
            typeof(ListViewSortableEffect),
            false,
            propertyChanged: OnIsSortableChanged);

        public static bool GetIsSortable(BindableObject view)
        {
            return (bool)view.GetValue(IsSortableProperty);
        }

        public static void SetIsSortable(BindableObject view, bool value)
        {
            view.SetValue(IsSortableProperty, value);
        }

        static void OnIsSortableChanged(BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (!(bindable is ListView view)) {
                return;
            }

            if (!view.Effects.Any(item => item is ListViewSortableEffect)) {
                view.Effects.Add(new ListViewSortableEffect());
            }
        }

        class ListViewSortableEffect : RoutingEffect
        {
            const string ResolutionName = "Nerdshoe.ListViewSortableEffect";
            public ListViewSortableEffect() : base(ResolutionName) { }
        }
    }
}
