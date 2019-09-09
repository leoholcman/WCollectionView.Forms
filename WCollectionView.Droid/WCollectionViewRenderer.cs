using Android.Content;
using Android.Support.V7.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using System.Collections;
using Android.Support.V4.Widget;

[assembly: ExportRenderer(typeof(Wapps.Forms.Controls.WCollectionView), typeof(Wapps.Forms.Controls.Droid.WCollectionViewRenderer))]
namespace Wapps.Forms.Controls.Droid
{
    public class WCollectionViewRenderer : ViewRenderer<WCollectionView, WRefreshLayout>
    {
        static Context _context;
        public static void Init(Context context)
        {
            _context = context;
            DroidUtils.Init(context);
            FormsView_Utils.Init(context);
        }

        public WCollectionViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == WCollectionView.ItemsSourceProperty.PropertyName && Control != null)
            {
                UpdateItemsSource();
            }
            else if (e.PropertyName == nameof(Element.BackgroundColor) && Control != null)
            {
                Control.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
            }
            else if (e.PropertyName == nameof(Element.IsPullToRefreshEnabled))
            {
                UpdateIsPullToRefreshEnabled();
            }
            else if (e.PropertyName == nameof(Element.IsRefreshing))
            {
                UpdateIsRefreshing();
            }

            if (Control == null && Element.Width > 0)
                CreateView();
        }

        void UpdateItemsSource()
        {
            var adapter = (WaterfallAdapter)Control.RecyclerView.GetAdapter();
            adapter.ItemsSource = Element.ItemsSource;
            adapter.NotifyDataSetChanged();

            if (Element.ItemsSource != null && Element.ItemsSource.Count() > 0)
                Control.RecyclerView.ScrollToPosition(0);
        }

        void CreateView()
        {
            var recyclerView = new RecyclerView(_context);
            var layoutManager = new StaggeredGridLayoutManager(Element.ColumnCount, StaggeredGridLayoutManager.Vertical);
            recyclerView.SetLayoutManager(layoutManager);

            var adapter = new WaterfallAdapter();
            adapter.Element = Element;
            adapter.ItemsSource = Element.ItemsSource;
            adapter.ItemClicked += Adapter_ItemClicked;
            adapter.ItemWidth = Element.Width / Element.ColumnCount;

            recyclerView.SetAdapter(adapter);

            var refreshLayout = new WRefreshLayout(_context, recyclerView);
            refreshLayout.Refresh += RefreshLayout_Refresh;
            SetNativeControl(refreshLayout);
        }

        void Adapter_ItemClicked(object sender, object item)
        {
            if (Element.IsGroupingEnabled && item is IEnumerable)
            {
                Element.InvokeHeaderTapped(item);
                return;
            }

            Element.InvokeItemTapped(item);
        }

        #region Pull To Refresh

        void UpdateIsPullToRefreshEnabled()
        {
            Control.Enabled = Element.IsPullToRefreshEnabled;
        }

        void UpdateIsRefreshing()
        {
            if (!Element.IsPullToRefreshEnabled)
                return;

            Control.Refreshing = Element.IsRefreshing;
        }

        void RefreshLayout_Refresh(object sender, System.EventArgs e)
        {
            Element.IsRefreshing = Control.Refreshing;
        }

        #endregion
    }

    public class WRefreshLayout : SwipeRefreshLayout
    {
        public RecyclerView RecyclerView { get; private set; }

        public WRefreshLayout(Context context, RecyclerView recyclerView) : base(context)
        {
            AddView(recyclerView);
            RecyclerView = recyclerView;
        }
    }

    internal static class Utils
    {
        public static int Count(this IEnumerable enumerable)
        {
            var count = 0;
            foreach (var item in enumerable)
                count++;

            return count;
        }
    }
}
