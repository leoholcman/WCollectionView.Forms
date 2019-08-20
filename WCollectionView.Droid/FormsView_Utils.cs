using System;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;

namespace Wapps.Forms.Controls.Droid
{
    internal static class FormsView_Utils
    {
        static Context _context;

        public static void Init(Context context)
        {
            _context = context;
        }

        public static AView ToAndroidView(this Xamarin.Forms.View view, Rectangle size)
        {
            var vRenderer = Platform.CreateRendererWithContext(view, _context);
            var aView = vRenderer.View;
            vRenderer.Tracker.UpdateLayout();
            view.Layout(size);

            var layoutParams = new ViewGroup.LayoutParams((int)size.Width, (int)size.Height);
            aView.LayoutParameters = layoutParams;
            aView.Layout(0, 0, (int)view.WidthRequest, (int)view.HeightRequest);
            return aView;
        }
    }
}

