using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;

namespace Wapps.Forms.IOS.Controls
{
    internal class GroupedWaterfallLayout : WaterfallViewLayout
    {
        public double HeaderTemplateHeight { get; set; }

        public List<int?> HeaderIndexes { get; set; } = new List<int?>();

        List<UICollectionViewLayoutAttributes> _cache = new List<UICollectionViewLayoutAttributes>();

        nfloat _contentHeight = 0;

        protected override void OnItemsSourcePropertyChanged()
        {
            _cache.Clear();
        }

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

            var column = 0;
            for (var r = 0; r < ItemsSource.Count; r++)
            {
                var isHeader = HeaderIndexes.Contains(r);

                var width = columnWidth;
                double cellHeight;
                if (GetHeightForCellDelegate != null)
                    cellHeight = GetHeightForCellDelegate(ItemsSource[r]);
                else
                    cellHeight = new Random().Next((int)MinCellHeight, (int)MaxCellHeight);

                var y = yOffsets[column];
                if (isHeader) // HEADER
                {
                    width = (float)CollectionView.Frame.Width;
                    cellHeight = HeaderTemplateHeight;
                    y = (float)_contentHeight;
                    column = 0;
                }

                var indexPath = NSIndexPath.FromRowSection(r, 0);
                var frame = new CGRect(xOffsets[column], y, width, cellHeight);

                var attrs = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
                attrs.Frame = frame;

                _contentHeight = Math.Max((float)_contentHeight, (float)frame.GetMaxY());

                if (isHeader)
                {
                    for (var i = 0; i < ColumnCount; i++)
                    {
                        yOffsets[i] = (float)_contentHeight;
                    }

                    column = 0;
                }
                else
                {
                    yOffsets[column] = (float)(yOffsets[column] + cellHeight);
                    column = column == ColumnCount - 1 ? 0 : column + 1;
                }
                _cache.Add(attrs);
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
}
