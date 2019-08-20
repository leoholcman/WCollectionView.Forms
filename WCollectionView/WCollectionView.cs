using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wapps.Forms.Controls
{
    public class WCollectionView : ContentView, INotifyPropertyChanged
    {
        public event Action<object, WItemTappedEventArgs> ItemTapped;

        public event Action<object, WItemTappedEventArgs> HeaderTapped;

        public int ColumnCount { get; set; } = 2;

        public double MinCellHeight { get; set; } = 150;

        public double MaxCellHeight { get; set; } = 250;

        public double HeaderTemplateHeight { get; set; } = 250;

        public GetHeightForItemDelegate GetHeightForCellDelegate { get; set; }
        
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(WCollectionView), null);
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(View), typeof(WCollectionView), null);
        public View Header
        {
            get => (View)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(WCollectionView), null, validateValue: (b, v) => true);
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty IsGroupingEnabledProperty = BindableProperty.Create(nameof(IsGroupingEnabled), typeof(bool), typeof(WCollectionView), false);
        public bool IsGroupingEnabled
        {
            get { return (bool)GetValue(IsGroupingEnabledProperty); }
            set { SetValue(IsGroupingEnabledProperty, value); }
        }

        public static readonly BindableProperty GroupHeaderTemplateProperty = BindableProperty.Create(nameof(GroupHeaderTemplate), typeof(DataTemplate), typeof(WCollectionView), null);
        public DataTemplate GroupHeaderTemplate
        {
            get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
            set { SetValue(GroupHeaderTemplateProperty, value); }
        }

        public static readonly BindableProperty ItemTappedCommandProperty = BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(WCollectionView), null);
        public ICommand ItemTappedCommand
        {
            get { return (ICommand)GetValue(ItemTappedCommandProperty); }
            set { SetValue(ItemTappedCommandProperty, value); }
        }

        public static readonly BindableProperty HeaderTappedCommandProperty = BindableProperty.Create(nameof(HeaderTappedCommand), typeof(ICommand), typeof(WCollectionView), null);
        public ICommand HeaderTappedCommand
        {
            get { return (ICommand)GetValue(HeaderTappedCommandProperty); }
            set { SetValue(HeaderTappedCommandProperty, value); }
        }

        public WCollectionView()
        {
            PropertyChanged += WaterfallListView_PropertyChanged;
            IsClippedToBounds = true;
        }

        void WaterfallListView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        public void InvokeItemTapped(object item)
        {
            if (item == null)
                return;

            ItemTapped?.Invoke(this, new WItemTappedEventArgs(item));
            ItemTappedCommand?.Execute(item);
        }

        public void InvokeHeaderTapped(object item)
        {
            if (item == null)
                return;

            HeaderTapped?.Invoke(this, new WItemTappedEventArgs(item));
            HeaderTappedCommand?.Execute(item);
        }

        List<object> _sourceList = new List<object>();
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == ItemsSourceProperty.PropertyName)
            {
                _sourceList.Clear();
                if (ItemsSource != null)
                {
                    foreach (var obj in ItemsSource)
                        _sourceList.Add(obj);
                }
            }
        }
    }

    public delegate double GetHeightForItemDelegate(object item);

    public class WItemTappedEventArgs : EventArgs
    {
        public object Item { get; private set; }

        internal WItemTappedEventArgs(object item)
        {
            Item = item;
        }
    }

    public static class Utils
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
