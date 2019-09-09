using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Wapps.Forms.Controls.iOS
{
    internal class WCollectionViewSource : UICollectionViewSource
    {
        public bool IsGroupingEnabled { get; set; }

        public event Action<int> ItemClicked;

        public List<object> ItemsSource { get; set; } = new List<object>();

        public List<int?> HeaderIndexes { get; set; } = new List<int?>();

        public DataTemplate ItemTemplate { get; set; }

        public DataTemplate HeaderTemplate { get; set; }

        public View Header { get; set; }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var isHeader = HeaderIndexes.Contains(indexPath.Row);

            WFCell cell;
            if (isHeader)
                cell = (WFCell)collectionView.DequeueReusableCell(WHeaderCell.CELL_ID, indexPath);
            else
                cell = (WFCell)collectionView.DequeueReusableCell(WFCell.CELL_ID, indexPath);

            if (cell.RendererView == null)
            {
                View formsView;
                if (isHeader)
                    formsView = (View)HeaderTemplate.CreateContent();
                else
                    formsView = (View)ItemTemplate.CreateContent();

                var nativeView = formsView.ToUIView(new CGRect(0, 0, cell.Frame.Width, cell.Frame.Height));
                cell.RendererView = nativeView;
            }

            var subview = cell.ContentView.Subviews[0];
            var frame = subview.Frame;
            //force height to the cell height in case the reusable cell subview have the old height
            subview.Frame = new CGRect(frame.X, frame.Y, cell.Frame.Width, cell.Frame.Height);

            var element = (cell.RendererView as VisualElementRenderer<VisualElement>).Element;
            element.BindingContext = ItemsSource[indexPath.Row];
            element.Layout(new Xamarin.Forms.Rectangle(0, 0, cell.Frame.Width, cell.Frame.Height));

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return ItemsSource.Count;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ItemClicked?.Invoke(indexPath.Row);
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            var header = (WHeader)collectionView.DequeueReusableSupplementaryView(elementKind, WHeader.CELL_ID, indexPath);
            var nativeView = Header.ToUIView(new CGRect(0, 0, header.Frame.Width, header.Frame.Height));
            header.RendererView = nativeView;
            return header;
        }
    }

    internal class WFCell : UICollectionViewCell
    {
        public const string CELL_ID = "CELL_ID";

        [Export("initWithFrame:")]
        public WFCell(RectangleF frame) : base(frame)
        {

        }

        UIView _rendererView;
        public UIView RendererView
        {
            get { return _rendererView; }
            set
            {
                _rendererView = value;
                if (ContentView.Subviews.Length > 0)
                {
                    ContentView.Subviews[0].RemoveFromSuperview();
                }

                var subview = new UIView(new CGRect(0, 0, Frame.Width, Frame.Height));
                _rendererView.Frame = subview.Frame;
                subview.Add(_rendererView);

                ContentView.AddSubview(subview);
            }
        }
    }

    internal class WHeaderCell : WFCell
    {
        public new const string CELL_ID = "HEADER_CELL_ID";

        [Export("initWithFrame:")]
        public WHeaderCell(RectangleF frame) : base(frame)
        {

        }
    }

    internal class WHeader : UICollectionReusableView
    {
        public const string CELL_ID = "HEADER_ID";

        [Export("initWithFrame:")]
        public WHeader(RectangleF frame) : base(frame)
        {

        }

        UIView _rendererView;
        public UIView RendererView
        {
            get { return _rendererView; }
            set
            {
                _rendererView = value;
                if (Subviews.Length > 0)
                {
                    Subviews[0].RemoveFromSuperview();
                }

                var subview = new UIView(new CGRect(0, 0, Frame.Width, Frame.Height));
                _rendererView.Frame = subview.Frame;
                subview.Add(_rendererView);

                AddSubview(subview);
            }
        }
    }
}
