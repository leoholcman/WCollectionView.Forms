using Android.Content;
using Android.Support.V7.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using System.Collections;

[assembly: ExportRenderer(typeof(Wapps.Forms.Controls.WCollectionView), typeof(Wapps.Forms.Controls.Droid.WaterfallListViewRenderer))]
namespace Wapps.Forms.Controls.Droid
{
    public class WaterfallListViewRenderer : ViewRenderer<WCollectionView, RecyclerView>
    {
        static Context _context;
        public static void Init(Context context)
        {
            _context = context;
            DroidUtils.Init(context);
            FormsView_Utils.Init(context);
        }

        public WaterfallListViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == WCollectionView.ItemsSourceProperty.PropertyName && Control != null)
            {
                RefreshItemsSource();
            }
            else if (e.PropertyName == nameof(Element.BackgroundColor) && Control != null)
            {
                Control.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
            }

            if (Control == null && Element.Width > 0)
                CreateView();
        }

        void RefreshItemsSource()
        {
            var adapter = (WaterfallAdapter)Control.GetAdapter();
            adapter.ItemsSource = Element.ItemsSource;
            adapter.NotifyDataSetChanged();

            if (Element.ItemsSource != null && Element.ItemsSource.Count() > 0)
                Control.ScrollToPosition(0);
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
            SetNativeControl(recyclerView);
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
    }
}
