using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace WinRT.Framework.Utils.Utils
{
    public static class DispatcherHelper
    {
        public static CoreDispatcher UiDispatcher
        {
            get;
            private set;
        }

        public static void CheckBeginInvokeOnUi(Action action)
        {
            if (UiDispatcher.HasThreadAccess)
            {
                action();
                return;
            }
            UiDispatcher.RunAsync(0, () => action());
        }

        public static IAsyncAction RunAsync(Action action)
        {
            return UiDispatcher.RunAsync(0, () => action());
        }

        public static void Initialize()
        {
            if (UiDispatcher != null)
            {
                return;
            }
            UiDispatcher = Window.Current.Dispatcher;
        }

    }
}
