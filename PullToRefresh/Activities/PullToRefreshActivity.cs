using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using PullToRefresh.Adapters;
using PullToRefresh.Helpers;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using PullToRefresh.Managers;
using System.Threading.Tasks;
using PullToRefresh.Models;
using Android.Graphics;
namespace PullToRefresh.Activities
{
    [Activity(Label = "PullToRefresh", MainLauncher = true, Icon = "@drawable/icon")]
    public class PullToRefreshActivity : AppCompatActivity
    {
        private RecyclerView recycler;
        private RecyclerView.LayoutManager layoutMgr;
        private PhotoAlbumAdapter albumAdapter;
        private SwipeRefreshManager<Models.RefreshEventArgs, RefreshCompletedEventArgs> refreshMgr;
        private SwipeRefreshLayout swipeRefreshLayout;
        private PhotoAlbum photoAlbum;
        private ProgressBar progressbar;
        public PhotoAlbum Photoalbum
        {
            get
            {
                if (photoAlbum == null)
                    photoAlbum = new PhotoAlbum();
                return photoAlbum;
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.PullToRefreshView);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            //swipeRefreshLayout.Refresh += SwipeRefreshLayout_Refresh;
            refreshMgr = new SwipeRefreshManager<Models.RefreshEventArgs, RefreshCompletedEventArgs>(swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                                                    Android.Resource.Color.HoloGreenLight,
                                                    Android.Resource.Color.HoloOrangeLight, 
                                                    Android.Resource.Color.HoloRedLight);
            refreshMgr.Refresh += OnRefresh;
            refreshMgr.RefreshCompleted += OnRefreshComplete;
            photoAlbum = new PhotoAlbum();
            recycler = FindViewById<RecyclerView>(Resource.Id.placesRecycler);
            layoutMgr = new LinearLayoutManager(this);
            progressbar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            progressbar.IndeterminateDrawable.SetColorFilter(Color.Red, PorterDuff.Mode.SrcIn);
            progressbar.Visibility = Android.Views.ViewStates.Invisible;
            recycler.SetLayoutManager(layoutMgr);
            recycler.SetItemAnimator(new DefaultItemAnimator());
            recycler.AddOnScrollListener(new Customscrolllistener(layoutMgr,progressbar));
            albumAdapter = new PhotoAlbumAdapter(this, photoAlbum);
            albumAdapter.setOnLoadMoreListener(new LoadMoreClass(albumAdapter));
            albumAdapter.ItemClick += OnItemClick;
            recycler.SetAdapter(albumAdapter);
        }
        // Handle the refresh
        private Task<bool> OnRefresh(object sender, Models.RefreshEventArgs e)
        {
            Java.Lang.Thread.Sleep(3000);
            if (albumAdapter.ItemCount < 36)
            {
                var newPhotos = PhotoAlbum.PhotosToAdd;
                albumAdapter.AddPhotos(0, newPhotos);
                refreshMgr.RefreshCompletedEventArgs = new RefreshCompletedEventArgs { NewImagesCount = newPhotos.Count };
                //albumAdapter.NotifyItemRangeInserted(0, newPhotos.Count);
            }
            return Task.FromResult(true);
        }

        // Handle the refresh complete
        private void OnRefreshComplete(object sender, RefreshCompletedEventArgs e)
        {
            if (albumAdapter.ItemCount <= 36)
                albumAdapter.NotifyItemRangeInserted(0, e.NewImagesCount);

            // Scroll the view to the top
            recycler.SmoothScrollToPosition(0); // Dznt scroll smoothly. TODO make it smooth

            Toast.MakeText(this, "Refreshed!!", ToastLength.Long).Show();
        }
        void OnItemClick(object sender, int e)
        {
            Toast.MakeText(this, "Selection: #" + (e + 1), ToastLength.Short).Show();
        }
    }
    public class Customscrolllistener : RecyclerView.OnScrollListener
    {
        LinearLayoutManager linearLayoutManager;
        RecyclerView recyclerView;
       public ProgressBar progressbar;

        private PhotoAlbumAdapter adapter;
        public PhotoAlbumAdapter Adapter
        {
            get
            {
                if (adapter == null)
                    adapter = (recyclerView.GetAdapter() as PhotoAlbumAdapter);
                return adapter;
            }
        }

        public Customscrolllistener(RecyclerView.LayoutManager manager,ProgressBar prog)
        {
            linearLayoutManager = (LinearLayoutManager)manager;
            progressbar = prog;
        }

        public async override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            this.recyclerView = recyclerView;
            var visibleItemCount = recyclerView.ChildCount;
            var totalItemCount = recyclerView.GetAdapter().ItemCount;
            var pastVisiblesItems = linearLayoutManager.FindFirstVisibleItemPosition();
            if ((visibleItemCount + pastVisiblesItems) >= totalItemCount)
            {
                if (!Adapter.mOnLoadMoreListener.HasMoreItems() || progressbar.Visibility == Android.Views.ViewStates.Visible)
                    return;
                // IsBusy true and change the visibility of the progressbar here
                progressbar.Visibility = Android.Views.ViewStates.Visible;
                //progressbar.IndeterminateDrawable.SetColorFilter(, Android.Graphics.PorterDuff.Mode.Multiply);
                adapter.mOnLoadMoreListener.LoadingComplete = LoadingCompleted;
                await Adapter.mOnLoadMoreListener.onLoadMore();
            }
        }

        private bool LoadingCompleted()
        {
            // Reset your busy indicator
            // Note: UI thread may be needed.
            (recyclerView.Context as Activity).RunOnUiThread(() =>
            {
                if(progressbar!=null)
                Adapter.NotifyDataSetChanged();
                progressbar.Visibility = Android.Views.ViewStates.Invisible;
            });
            return true;
        }
    }
}


