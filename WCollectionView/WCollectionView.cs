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

        public event EventHandler Refreshing;

        public int ColumnCount { get; set; } = 2;

        public double MinCellHeight { get; set; } = 150;

        public double MaxCellHeight { get; set; } = 250;

        public double HeaderTemplateHeight { get; set; } = 250;

        public GetHeightForItemDelegate GetHeightForCellDelegate { get; set; }

        public static readonly BindableProperty IsPullToRefreshEnabledProperty = BindableProperty.Create("IsPullToRefreshEnabled", typeof(bool), typeof(WCollectionView), false);
        public bool IsPullToRefreshEnabled
        {
            get => (bool)GetValue(IsPullToRefreshEnabledProperty);
            set => SetValue(IsPullToRefreshEnabledProperty, value);
        }

        public static readonly BindableProperty IsRefreshingProperty = BindableProperty.Create("IsRefreshing", typeof(bool), typeof(WCollectionView), false, BindingMode.TwoWay);
        public bool IsRefreshing
        {
            get => (bool)GetValue(IsRefreshingProperty);
            set => SetValue(IsRefreshingProperty, value);
        }

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
        
        public static readonly BindableProperty RefreshCommandProperty = BindableProperty.Create(nameof(RefreshCommand), typeof(ICommand), typeof(WCollectionView), null);
        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        public WCollectionView()
        {
            PropertyChanged += WCollectionView_PropertyChanged;
            IsClippedToBounds = true;
        }

        void WCollectionView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        public void InvokeItemTapped(object item)
        {
            if (item == null)
                return;

            ItemTapped?.Invoke(this, new WItemTappedEventArgs(item));
            if (ItemTappedCommand != null && ItemTappedCommand.CanExecute(item))
			{
				ItemTappedCommand.Execute(item);
			}
        }

        public void InvokeHeaderTapped(object item)
        {
            if (item == null)
                return;

            HeaderTapped?.Invoke(this, new WItemTappedEventArgs(item));            
            if (HeaderTappedCommand != null && HeaderTappedCommand.CanExecute(item))
			{
				HeaderTappedCommand.Execute(item);
			}
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
            else if (propertyName == IsRefreshingProperty.PropertyName)
            {
                if (!IsPullToRefreshEnabled)
                    return;

                Refreshing?.Invoke(this, new EventArgs());

                if (IsRefreshing && RefreshCommand != null && RefreshCommand.CanExecute(null))
                {
                    RefreshCommand.Execute(null);
                }
            }
        }

        #region Public Members

        public void BeginRefresh()
        {
            if (IsPullToRefreshEnabled)
                IsRefreshing = true;
        }

        public void EndRefresh()
        {
            IsRefreshing = false;
        }

        #endregion
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
}
