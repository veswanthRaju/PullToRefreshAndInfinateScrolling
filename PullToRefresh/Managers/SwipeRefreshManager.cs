using System;
using Android.Support.V4.Widget;
using System.ComponentModel;
using System.Threading.Tasks;

namespace PullToRefresh.Managers
{
    public class SwipeRefreshManager<TRefreshEventArgs, TRefreshCompletedEventArgs>
    {
        #region Events...

        // Return true if refresh is completed
        public delegate Task<bool> RefreshEventHandler(object sender, TRefreshEventArgs e);
        public event RefreshEventHandler Refresh;

        public delegate void RefreshCompletedEventHandler(object sender, TRefreshCompletedEventArgs e);
        public event RefreshCompletedEventHandler RefreshCompleted;

        #endregion Events.

        #region Fields & Properties...

        private SwipeRefreshLayout swipeRefreshLayout;

        private TRefreshEventArgs _refreshEventArgs;
        private TRefreshCompletedEventArgs _refreshCompletedEventArgs;

        public TRefreshEventArgs RefreshEventArgs
        {
            get
            {
                return _refreshEventArgs;
            }

            set
            {
                _refreshEventArgs = value;
            }
        }
        public TRefreshCompletedEventArgs RefreshCompletedEventArgs
        {
            get
            {
                //if (_refreshCompletedEventArgs == null)
                //    _refreshCompletedEventArgs = EventArgs.Empty;

                //return _refreshCompletedEventArgs;

                return _refreshCompletedEventArgs;
            }

            set
            {
                _refreshCompletedEventArgs = value;
            }
        }

        #endregion Fields & Properties.

        #region Constructors...

        public SwipeRefreshManager(SwipeRefreshLayout _swipeRefreshLayout)
        {
            swipeRefreshLayout = _swipeRefreshLayout;
            swipeRefreshLayout.Refresh += SwipeRefreshLayout_Refresh;
        }

        #endregion Constructors.

        #region Thread creation and events raising...

        private void SwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swipeRefreshLayout.Refreshing = false;
            //Toast.MakeText(this, "Refreshed!!", ToastLength.Long).Show();
            RefreshCompleted?.Invoke(this, RefreshCompletedEventArgs);
        }

        private async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Runs on a separate thread
            //Thread.Sleep(3000);
            var result = await Refresh?.Invoke(this, RefreshEventArgs);

            if(!result)
            {
                throw new Exception("The refresh could not be completed!!");
            }
        }

        #endregion Thread creation and events raising.
    }
}