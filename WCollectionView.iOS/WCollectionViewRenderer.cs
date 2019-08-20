using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UIKit;
using Wapps.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WCollectionView), typeof(Wapps.Forms.IOS.Controls.WCollectionViewRenderer))]
namespace Wapps.Forms.IOS.Controls
{
    public class WCollectionViewRenderer : ViewRenderer<WCollectionView, UICollectionView>
    {
        public static void Init()
        {

        }

        List<object> SourceList { get; set; } = new List<object>();

        WaterfallViewLayout _layout;

        List<int?> HeaderIndexes { get; set; } = new List<int?>();

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Element.Width) && Control == null)
            {
                CreateView();
            }
            else if (e.PropertyName == nameof(Element.ItemsSource) && Control != null)
            {
                RefreshItemsSource();
            }
            else if (e.PropertyName == nameof(Element.BackgroundColor) && Control != null)
            {
                Control.BackgroundColor = Element.BackgroundColor.ToUIColor();
            }
            else if (e.PropertyName == nameof(Element.Header) && Control != null)
            {
                RefreshHeader();
            }
        }

        void RefreshItemsSource()
        {
            InitSource();
            Device.BeginInvokeOnMainThread(() =>
            {
                var source = (WaterfallCollectionViewSource)Control.Source;

                if (Element.IsGroupingEnabled)
                {
                    source.HeaderIndexes = HeaderIndexes;
                    (_layout as GroupedWaterfallLayout).HeaderIndexes = HeaderIndexes;
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

        void RefreshHeader()
        {
            var source = (WaterfallCollectionViewSource)Control.Source;
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
                var layout = new GroupedWaterfallLayout();
                layout.ItemsSource = SourceList;
                layout.HeaderTemplateHeight = Element.HeaderTemplateHeight;

                _layout = layout;
            }
            else
            {
                _layout = new WaterfallLayout();
                _layout.ItemsSource = SourceList;
            }

            _layout.GetHeightForCellDelegate = Element.GetHeightForCellDelegate;
            _layout.ColumnCount = Element.ColumnCount;
            _layout.MinCellHeight = Element.MinCellHeight;
            _layout.MaxCellHeight = Element.MaxCellHeight;
            _layout.Header = Element.Header;

            var source = new WaterfallCollectionViewSource();
            source.ItemTemplate = Element.ItemTemplate;
            source.HeaderTemplate = Element.GroupHeaderTemplate;
            source.ItemClicked += Source_ItemClicked;
            source.IsGroupingEnabled = Element.IsGroupingEnabled;
            source.ItemsSource = SourceList;
            source.Header = Element.Header;

            var frame = new CGRect(0, 0, Element.Width, Element.Height);
            var collectionView = new UICollectionView(frame, _layout);
            collectionView.RegisterClassForCell(typeof(WFCell), WFCell.CELL_ID);
            collectionView.RegisterClassForCell(typeof(WFHeaderCell), WFHeaderCell.CELL_ID);
            collectionView.Source = source;
            collectionView.BackgroundColor = Element.BackgroundColor.ToUIColor();

            if (Element.Header != null)
                collectionView.RegisterClassForSupplementaryView(typeof(WFHeader), UICollectionElementKindSection.Header, WFHeader.CELL_ID);

            SetNativeControl(collectionView);
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
    }
}
