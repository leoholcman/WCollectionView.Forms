using System;
using System.Collections;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;
using ViewGroup = Android.Views.ViewGroup;

namespace Wapps.Forms.Controls.Droid
{
    internal class WaterfallAdapter : RecyclerView.Adapter
    {
        public event EventHandler<object> ItemClicked;

        public Wapps.Forms.Controls.WCollectionView Element { get; set; }

        public double ItemWidth { get; set; }

        IEnumerable _itemsSource;
        public IEnumerable ItemsSource
        {
            get { return _itemsSource; }
            set
            {
                _itemsSource = value;
                OnItemsSourceSetted();
            }
        }

        List<int> _heights;

        List<object> SourceList { get; set; } = new List<object>();

        Dictionary<int, bool> _headerIndexes = new Dictionary<int, bool>();

        const int HEADER_VIEW_TYPE = 0;

        const int GROUP_HEADER_VIEW_TYPE = 1;

        const int CELL_VIEW_TYPE = 2;

        public override int ItemCount
        {
            get
            {
                if (Element.Header != null)
                    return SourceList.Count + 1;

                return SourceList.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var isHeader = holder is WFGroupHeaderViewHolder;

            var itemView = holder.ItemView;
            var element = (itemView as VisualElementRenderer<View>).Element;

            var dpHeight = 0;
            int pxHeight;
            var dpWidth = (int)ItemWidth;

            if (holder is WFHeaderViewHolder)
            {
                dpHeight = (int)Element.Header.HeightRequest;

                element.Layout(new Rectangle(0, 0, Element.Width, dpHeight));
                pxHeight = DroidUtils.DpToPixel(dpHeight);

                var pxWidth = DroidUtils.DpToPixel((int)Element.Width);
                var layoutParams = new ViewGroup.LayoutParams(pxWidth, pxHeight);
                itemView.LayoutParameters = layoutParams;
                itemView.Layout(0, 0, pxWidth, pxHeight);

                var lParams = new StaggeredGridLayoutManager.LayoutParams(itemView.LayoutParameters);
                lParams.FullSpan = true;
                itemView.LayoutParameters = lParams;
                return;
            }

            if (Element.Header != null)
                position--; //because the first index is the Header View

            element.BindingContext = SourceList[position];

            if (holder is WFGroupHeaderViewHolder)
            {
                (holder as WFGroupHeaderViewHolder).BindingContext = element.BindingContext;

                dpHeight = (int)Element.HeaderTemplateHeight;

                element.Layout(new Rectangle(0, 0, Element.Width, dpHeight));
                pxHeight = DroidUtils.DpToPixel(dpHeight);

                var pxWidth = DroidUtils.DpToPixel((int)Element.Width);
                var layoutParams = new ViewGroup.LayoutParams(pxWidth, pxHeight);
                itemView.LayoutParameters = layoutParams;
                itemView.Layout(0, 0, pxWidth, pxHeight);

                var lParams = new StaggeredGridLayoutManager.LayoutParams(itemView.LayoutParameters);
                lParams.FullSpan = true;
                itemView.LayoutParameters = lParams;
            }
            else
            {
                (holder as WFViewHolder).BindingContext = element.BindingContext;

                if (Element.GetHeightForCellDelegate != null)
                    dpHeight = (int)Element.GetHeightForCellDelegate(SourceList[position]);
                else
                    dpHeight = _heights[position];

                element.Layout(new Rectangle(0, 0, dpWidth, dpHeight));
                pxHeight = DroidUtils.DpToPixel(dpHeight);

                var pxWidth = DroidUtils.DpToPixel((int)ItemWidth);
                var layoutParams = new ViewGroup.LayoutParams(pxWidth, pxHeight);
                itemView.LayoutParameters = layoutParams;
                itemView.Layout(0, 0, pxWidth, pxHeight);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == GROUP_HEADER_VIEW_TYPE)
                return new WFGroupHeaderViewHolder(CreateGroupHeaderView(), OnClick);

            if (viewType == HEADER_VIEW_TYPE)
            {
                var nativeView = Element.Header.ToAndroidView(new Rectangle(0, 0, ItemWidth, 0));
                return new WFHeaderViewHolder(nativeView);
            }

            return new WFViewHolder(CreateItemView(), OnClick);
        }

        public override int GetItemViewType(int position)
        {
            if (position == 0 && Element.Header != null)
                return HEADER_VIEW_TYPE;

            if (Element.IsGroupingEnabled && _headerIndexes.ContainsKey(position))
                return GROUP_HEADER_VIEW_TYPE;

            return CELL_VIEW_TYPE;
        }

        void InitHeights()
        {
            _heights = new List<int>();
            foreach (var obj in SourceList)
            {
                var random = new Random();
                int height = random.Next((int)Element.MinCellHeight, (int)Element.MaxCellHeight);
                _heights.Add(height);
            }
        }

        AView CreateItemView()
        {
            var view = (View)Element.ItemTemplate.CreateContent();
            var nativeView = view.ToAndroidView(new Rectangle(0, 0, ItemWidth, 0));
            return nativeView;
        }

        AView CreateGroupHeaderView()
        {
            var view = (View)Element.GroupHeaderTemplate.CreateContent();
            var nativeView = view.ToAndroidView(new Rectangle(0, 0, ItemWidth, 0));
            return nativeView;
        }

        void OnClick(object item)
        {
            if (ItemClicked != null)
                ItemClicked(this, item);
        }

        void OnItemsSourceSetted()
        {
            SourceList.Clear();

            if (_itemsSource == null)
                return;

            if (Element.IsGroupingEnabled)
            {
                var index = 0;
                foreach (var group in _itemsSource)
                {
                    _headerIndexes[index] = true;
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
                foreach (var item in _itemsSource)
                    SourceList.Add(item);
            }

            if (Element != null)
                InitHeights();
        }
    }

    class WFViewHolder : RecyclerView.ViewHolder
    {
        public object BindingContext { get; set; }

        public WFViewHolder(AView view, Action<object> listener) : base(view)
        {
            ItemView = view;
            view.Click += (sender, e) =>
            {
                listener(BindingContext);
            };
        }
    }

    class WFGroupHeaderViewHolder : RecyclerView.ViewHolder
    {
        public object BindingContext { get; set; }

        public WFGroupHeaderViewHolder(AView view, Action<object> listener) : base(view)
        {
            ItemView = view;
            view.Click += (sender, e) =>
            {
                listener(BindingContext);
            };
        }
    }

    class WFHeaderViewHolder : RecyclerView.ViewHolder
    {
        public WFHeaderViewHolder(AView view) : base(view)
        {
            ItemView = view;

            var lParams = new StaggeredGridLayoutManager.LayoutParams(ItemView.LayoutParameters);
            lParams.FullSpan = true;
            ItemView.LayoutParameters = lParams;
        }
    }
}
