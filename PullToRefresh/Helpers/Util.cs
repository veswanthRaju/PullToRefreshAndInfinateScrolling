using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PullToRefresh.Helpers
{
    public static class Util
    {
        public static void GoTo<T>(Context sender)
        {
            Intent intent = new Intent(sender, typeof(T));
            sender.StartActivity(intent);
        }
    }
}