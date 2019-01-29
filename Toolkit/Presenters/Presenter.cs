using System;
using Nerdshoe.Views;

namespace Nerdshoe.Presenters
{
    public abstract class Presenter : BaseObject
    {
        static IApplication application;

        public IApplication Application {
            get {
                if (application == null) {
                    application = IoC.Default.Resolve<IApplication>();
                    application = application
                        ?? throw new Exception("Resolve Application Model.");
                }
                return application;
            }
        }
    }

    public abstract class Presenter<TView> : Presenter where TView : IView
    {
        public Presenter(TView view)
        {
            View = view;
        }

        public TView View { get; private set; }
    }
}
