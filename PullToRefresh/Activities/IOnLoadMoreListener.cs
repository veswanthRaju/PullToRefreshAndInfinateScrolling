using System;
using System.Threading.Tasks;

namespace PullToRefresh.Activities
{
    public interface IOnLoadMoreListener
    {
        bool HasMoreItems();
        Task onLoadMore();
        Func<bool> LoadingComplete { get; set; }
    }
}