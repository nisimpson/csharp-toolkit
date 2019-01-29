// LongPressEffect.cs
// reference: https://alexdunn.org/2017/12/27/xamarin-tip-xamarin-forms-long-press-effect/

using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Nerdshoe
{
    public class LongPressEffect : RoutingEffect
    {
        const string Keyword = "Nerdshoe.LongPressEffect";

        public LongPressEffect() : base(Keyword) { }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.CreateAttached("Command",
                typeof(ICommand),
                typeof(LongPressEffect),
                defaultValue: null);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.CreateAttached("CommandParameter",
                typeof(object),
                typeof(LongPressEffect),
                defaultValue: null);

        public static readonly BindableProperty IsEnabledProperty =
            BindableProperty.CreateAttached("IsEnabled",
                typeof(bool),
                typeof(LongPressEffect),
                defaultValue: false,
                propertyChanged: OnIsEnabledChanged);

        public static bool GetIsEnabled(BindableObject view)
        {
            return (bool)view.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(BindableObject view, bool value)
        {
            view.SetValue(IsEnabledProperty, value);
        }

        static void OnIsEnabledChanged(BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (!(bindable is Element element)) return;

            if (!element.Effects.Any(item => item is LongPressEffect)) {
                element.Effects.Add(Resolve(Keyword));
            }
        }

        public static void SetCommand(BindableObject view, ICommand value)
        {
            view.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(BindableObject view)
        {
            return view.GetValue(CommandProperty) as ICommand;
        }

        public static void SetCommandParameter(BindableObject view, object value)
        {
            view.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(BindableObject view)
        {
            return view.GetValue(CommandParameterProperty);
        }
    }
}
