using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace PullToRefresh.Holders
{
    public class PhotoViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Caption { get; private set; }

        public PhotoViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.image);
            Caption = itemView.FindViewById<TextView>(Resource.Id.caption);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
}
