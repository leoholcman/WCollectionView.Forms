using System;
using Android.Content;
using Android.Util;

namespace Wapps.Forms.Controls.Droid
{
    internal static class DroidUtils
    {
        static Context _context;

        public static void Init(Context context)
        {
            _context = context;
        }

        public static int DpToPixel(int dp)
        {
            int pixel = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, _context.Resources.DisplayMetrics);
            return pixel;
        }
    }
}
