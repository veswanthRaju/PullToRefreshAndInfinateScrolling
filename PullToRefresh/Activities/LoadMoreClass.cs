using PullToRefresh.Helpers;
using PullToRefresh.Adapters;
using System.Threading.Tasks;
using System;

namespace PullToRefresh.Activities
{
    public class LoadMoreClass : IOnLoadMoreListener
    {
        PhotoAlbum photoAlbum = new PhotoAlbum();
        PhotoAlbumAdapter albumAdapter;
       public int intialPhotoCount = 10;

        public Func<bool> LoadingComplete
        {
            get;
            set;
        }
        public LoadMoreClass(PhotoAlbumAdapter adapter)
        {
            albumAdapter = adapter;
        }
        public async Task onLoadMore()
        {
            //new Handler().PostDelayed(async () =>
            //{
            var task = Task.Run(async () => { await Task.Delay(5000); });
            await task.ContinueWith(GetMore);
            //}, 5000);
            intialPhotoCount += intialPhotoCount;
        }

        public async Task GetMore(Task get)
        {
            await Task.Run(() =>
            {
                var data = photoAlbum.MyPhotoAlbums.GetRange(intialPhotoCount, 10);
                albumAdapter.photoAlbum.AddNewPhotos(data);
                LoadingComplete();
            });
        }

        public bool HasMoreItems()
        {
            return photoAlbum.MyPhotoAlbums.Count > intialPhotoCount;
        }
    }
}

