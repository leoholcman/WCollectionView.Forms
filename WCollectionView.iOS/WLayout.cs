using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Xamarin.Forms;
using Wapps.Forms.Controls;

namespace Wapps.Forms.Controls.iOS
{
    internal class WLayout : WViewLayout
    {
        protected override void OnItemsSourcePropertyChanged()
        {
            _cache.Clear();
        }

        List<UICollectionViewLayoutAttributes> _cache = new List<UICollectionViewLayoutAttributes>();

        nfloat _contentHeight = 0;

        public override void PrepareLayout()
        {
            if (_cache.Count > 0 || CollectionView == null)
                return;

            _contentHeight = 0;
            var columnWidth = (float)CollectionView.Frame.Width / ColumnCount;
            var xOffsets = new List<float>();
            var yOffsets = new List<float>();
            for (var i = 0; i < ColumnCount; i++)
            {
                xOffsets.Add(i * columnWidth);
                yOffsets.Add(0);
            }

            var column = 0;

            if (Header != null)
            {
                var indexPath = NSIndexPath.FromRowSection(0, 0);
                var frame = new CGRect(0, 0, CollectionView.Frame.Width, Header.HeightRequest);

                var attrs = UICollectionViewLayoutAttributes.CreateForSupplementaryView(UICollectionElementKindSection.Header, indexPath);
                attrs.Frame = frame;
                _cache.Add(attrs);

                for (var i = 0; i < ColumnCount; i++)
                {
                    yOffsets[i] = (float)frame.Height;
                }
            }

            var itemsCount = CollectionView.NumberOfItemsInSection(0);
            for (var i = 0; i < itemsCount; i++)
            {
                var indexPath = NSIndexPath.FromRowSection(i, 0);

                double cellHeight;
                if (GetHeightForCellDelegate != null)
                    cellHeight = GetHeightForCellDelegate(ItemsSource[i]);
                else
                    cellHeight = new Random().Next((int)MinCellHeight, (int)MaxCellHeight);

                var frame = new CGRect(xOffsets[column], yOffsets[column], columnWidth, cellHeight);

                var attrs = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
                attrs.Frame = frame;
                _cache.Add(attrs);

                _contentHeight = Math.Max((float)_contentHeight, (float)frame.GetMaxY());
                yOffsets[column] = (float)(yOffsets[column] + cellHeight);

                column = column == ColumnCount - 1 ? 0 : column + 1;
            }
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            var visibleLayoutAttributes = new List<UICollectionViewLayoutAttributes>();

            foreach (var attr in _cache)
            {
                if (attr.Frame.IntersectsWith(rect))
                    visibleLayoutAttributes.Add(attr);
            }

            return visibleLayoutAttributes.ToArray();
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
        {
            return _cache[indexPath.Row];
        }

        public override CGSize CollectionViewContentSize
        {
            get { return new CGSize(CollectionView.Frame.Width, _contentHeight); }
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForSupplementaryView(NSString kind, NSIndexPath indexPath)
        {
            var attrs = UICollectionViewLayoutAttributes.CreateForSupplementaryView(kind, indexPath);
            attrs.Frame = new CGRect(0, 0, CollectionView.Frame.Width, Header.HeightRequest);
            return attrs;
        }
    }

    internal abstract class WViewLayout : UICollectionViewLayout
    {
        public int ColumnCount { get; set; }

        public double MinCellHeight { get; set; }

        public double MaxCellHeight { get; set; }

        public View Header { get; set; }

        public GetHeightForItemDelegate GetHeightForCellDelegate { get; set; }

        List<object> _itemsSource = new List<object>();
        public virtual List<object> ItemsSource
        {
            get { return _itemsSource; }
            set
            {
                _itemsSource = value;
                OnItemsSourcePropertyChanged();
                InvalidateLayout();
            }
        }

        protected virtual void OnItemsSourcePropertyChanged()
        {

        }
    }
}