using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UIKit;
using Wapps.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WCollectionView), typeof(Wapps.Forms.Controls.iOS.WCollectionViewRenderer))]
namespace Wapps.Forms.Controls.iOS
{
    public class WCollectionViewRenderer : ViewRenderer<WCollectionView, UICollectionView>
    {
        public static void Init()
        {

        }

        List<object> SourceList { get; set; } = new List<object>();

        WViewLayout _layout;

        List<int?> HeaderIndexes { get; set; } = new List<int?>();

        UIRefreshControl RefreshControl { get; set; }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Element.Width) && Control == null)
            {
                CreateView();
            }
            else if (e.PropertyName == nameof(Element.ItemsSource) && Control != null)
            {
                UpdateItemsSource();
            }
            else if (e.PropertyName == nameof(Element.BackgroundColor) && Control != null)
            {
                Control.BackgroundColor = Element.BackgroundColor.ToUIColor();
            }
            else if (e.PropertyName == nameof(Element.Header) && Control != null)
            {
                UpdateHeader();
            }
            else if (e.PropertyName == nameof(Element.IsPullToRefreshEnabled))
            {
                UpdateIsPullToRefreshEnabled();
            }
            else if (e.PropertyName == nameof(Element.IsRefreshing))
            {
                UpdateIsRefreshing();
            }
        }

        void UpdateItemsSource()
        {
            InitSource();
            Device.BeginInvokeOnMainThread(() =>
            {
                var source = (WCollectionViewSource)Control.Source;

                if (Element.IsGroupingEnabled)
                {
                    source.HeaderIndexes = HeaderIndexes;
                    (_layout as GroupedWLayout).HeaderIndexes = HeaderIndexes;
                }
                else
                {
                    source.ItemsSource = SourceList;
                }

                _layout.ItemsSource = SourceList;
                _layout.GetHeightForCellDelegate = Element.GetHeightForCellDelegate;

                Control.ReloadData();

                if (SourceList.Count > 0)
                    Control.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.Top, false);
            });
        }

        void UpdateHeader()
        {
            var source = (WCollectionViewSource)Control.Source;
            source.Header = Element.Header;
            _layout.Header = Element.Header;
        }

        void CreateView()
        {
            InitSource();

            if (Element.ColumnCount < 1)
                Element.ColumnCount = 2;


            if (Element.IsGroupingEnabled)
            {
                var layout = new GroupedWLayout();
                layout.ItemsSource = SourceList;
                layout.HeaderTemplateHeight = Element.HeaderTemplateHeight;

                _layout = layout;
            }
            else
            {
                _layout = new WLayout();
                _layout.ItemsSource = SourceList;
            }

            _layout.GetHeightForCellDelegate = Element.GetHeightForCellDelegate;
            _layout.ColumnCount = Element.ColumnCount;
            _layout.MinCellHeight = Element.MinCellHeight;
            _layout.MaxCellHeight = Element.MaxCellHeight;
            _layout.Header = Element.Header;

            var source = new WCollectionViewSource();
            source.ItemTemplate = Element.ItemTemplate;
            source.HeaderTemplate = Element.GroupHeaderTemplate;
            source.ItemClicked += Source_ItemClicked;
            source.IsGroupingEnabled = Element.IsGroupingEnabled;
            source.ItemsSource = SourceList;
            source.Header = Element.Header;

            var frame = new CGRect(0, 0, Element.Width, Element.Height);
            var collectionView = new UICollectionView(frame, _layout);
            collectionView.RegisterClassForCell(typeof(WFCell), WFCell.CELL_ID);
            collectionView.RegisterClassForCell(typeof(WHeaderCell), WHeaderCell.CELL_ID);
            collectionView.Source = source;
            collectionView.BackgroundColor = Element.BackgroundColor.ToUIColor();
            collectionView.AlwaysBounceVertical = true;

            if (Element.Header != null)
                collectionView.RegisterClassForSupplementaryView(typeof(WHeader), UICollectionElementKindSection.Header, WHeader.CELL_ID);

            var refreshControl = new UIRefreshControl();
            refreshControl.TintColor = UIColor.Gray;
            refreshControl.ValueChanged += RefreshControl_ValueChanged;

            if (Element.IsPullToRefreshEnabled)
            {
                collectionView.AddSubview(refreshControl);
            }

            SetNativeControl(collectionView);
            RefreshControl = refreshControl;
        }

        void Source_ItemClicked(int row)
        {
            object item = SourceList[row];

            if (Element.IsGroupingEnabled && HeaderIndexes.Contains(row))
            {
                Element.InvokeHeaderTapped(item);
                return;
            }

            Element.InvokeItemTapped(item);
        }

        void InitSource()
        {
            SourceList.Clear();
            HeaderIndexes.Clear();

            if (Element.ItemsSource == null)
                return;

            SourceList.Clear();

            if (Element.IsGroupingEnabled)
            {
                var index = 0;
                foreach (var group in Element.ItemsSource)
                {
                    HeaderIndexes.Add(index);
                    var headerIndex = index;

                    SourceList.Add(group);
                    index++;

                    var groupList = group as IEnumerable;
                    foreach (var item in groupList)
                    {
                        SourceList.Add(item);
                        index++;
                    }
                }
            }
            else
            {
                foreach (var item in Element.ItemsSource)
                    SourceList.Add(item);
            }
        }

        #region Pull To Refresh

        void UpdateIsPullToRefreshEnabled()
        {
            if (Element.IsPullToRefreshEnabled)
                Control.AddSubview(RefreshControl);
            else
                RefreshControl.RemoveFromSuperview();
        }

        void UpdateIsRefreshing()
        {
            if (!Element.IsPullToRefreshEnabled)
                return;

            if (Element.IsRefreshing)
            {
                if (!RefreshControl.Refreshing)
                    Control.SetContentOffset(new CGPoint(0, RefreshControl.Frame.Height * -1), true);

                RefreshControl.BeginRefreshing();
            }
            else
            {
                RefreshControl.EndRefreshing();
            }
        }

        void RefreshControl_ValueChanged(object sender, System.EventArgs e)
        {
            Element.IsRefreshing = RefreshControl.Refreshing;
        }

        #endregion
    }
}
