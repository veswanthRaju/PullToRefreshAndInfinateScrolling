using System;

namespace PullToRefresh.Models
{
    public class RefreshCompletedEventArgs : EventArgs
    {
        public int NewImagesCount = 0;
    }
}