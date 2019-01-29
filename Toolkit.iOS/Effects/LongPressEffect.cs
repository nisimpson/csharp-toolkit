using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Nerdshoe.iOS;
using System.Reactive.Subjects;
using System;
using System.Reactive.Linq;

[assembly: ExportEffect(typeof(IOSLongPressEffect), "LongPressEffect")]
namespace Nerdshoe.iOS
{
    public class IOSLongPressEffect : PlatformEffect
    {
        private bool _attached;
        private Cell cell;
        private readonly UILongPressGestureRecognizer _longPressRecognizer;
        private readonly Subject<string> longPressSubject;
        private IDisposable longPressSubscription;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="IOSLongPressEffect"/> class.
        /// </summary>
        public IOSLongPressEffect()
        {
            _longPressRecognizer = new UILongPressGestureRecognizer(HandleLongClick);
            longPressSubject = new Subject<string>();

            // long press detection events can potentially fire several times
            // within the span of a user click, so we must add debounce safety.
            longPressSubscription = longPressSubject
                .Throttle(TimeSpan.Zero)
                .Subscribe(HandleLongClick);
        }

        /// <summary>
        /// For when cells, particularly text cells, don't attach
        /// automatically when this effect is added.
        /// </summary>
        /// <param name="formCell">Cell.</param>
        /// <param name="view">View.</param>
        internal void AttachCell(Cell formCell, UIView view)
        {
            cell = formCell;
            OnAttached(view);

            void disppearingHandler(object sender, EventArgs args)
            {
                formCell.Disappearing -= disppearingHandler;
                OnDetached(view);
            }

            formCell.Disappearing += disppearingHandler;
        }

        /// <summary>
        /// Apply the handler
        /// </summary>
        protected override void OnAttached()
        {
            OnAttached(Container);
        }

        /// <summary>
        /// Clean the event handler on detach
        /// </summary>
        protected override void OnDetached() => OnDetached(Container);

        private void OnAttached(UIView view)
        {
            // because an effect can be detached immediately after attached
            // (happens in listview), only attach the handler one time
            if (!_attached) {
                view.AddGestureRecognizer(_longPressRecognizer);
                _attached = true;
            }
        }

        private void OnDetached(UIView view)
        {
            if (_attached) {
                view.RemoveGestureRecognizer(_longPressRecognizer);
                longPressSubscription.Dispose();
                _attached = false;
            }
        }

        /// <summary>
        /// Invoke the command if there is one
        /// </summary>
        private void HandleLongClick()
        {
            // have the subject handle it -- debounce is built in.
            longPressSubject.OnNext(null);
        }

        private void HandleLongClick(string arg)
        {
            Element element = cell ?? Element;
            var command = LongPressEffect.GetCommand(element);
            command?.Execute(LongPressEffect.GetCommandParameter(element));
        }
    }
}
