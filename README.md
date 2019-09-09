# WCollectionView.Forms

[![NuGet](https://img.shields.io/nuget/v/WCollectionView.Forms)](https://www.nuget.org/packages/WCollectionView.Forms/)

The WCollectionView is a Grid Collection View for Xamarin Forms, where you can define a custom height for each item,
getting the "Waterfall" effect. If you don't provide the `GetHeightForItemDelegate`, you can define a min. & max. heights, so the control will set a random height between those values.

## Features
* Column Count
* Custom item height
* Grouping
* Pull To Refresh
* Header
* Item Tapped Event
* Header Tapped Event

### Default Values
* `ColumnCount = 2`
* `MinCellHeight = 150`
* `MaxCellHeight = 250`
* `HeaderTemplateHeight = 250`
* `IsPullToRefreshEnabled = false`
* `IsGroupingEnabled = false`

##### Android 

On Android, in your main `Activity`'s `OnCreate (..)` implementation, call:

```csharp
Wapps.Forms.Controls.Droid.WCollectionViewRenderer.Init(this);
```

##### iOS

In your `AppDelegate`'s `FinishedLaunching (..)` implementation, call:

```csharp
Wapps.Forms.Controls.iOS.WCollectionViewRenderer.Init();
```

##### Custom Height For Item Delegate (GetHeightForItemDelegate)
```csharp
double Lv_HeightForCellDelegate(object item)
{
    //calculate the item height here
    return 0;
}
```

##### Item Tapped Event
```csharp
void WCollectionView_ItemTapped(object sender, Wapps.Forms.Controls.WItemTappedEventArgs args)
{

}
```

##### Groups
Setting `IsGroupingEnabled = true` you can define a `GroupHeaderTemplate` and set the `ItemsSource` a `IEnumerable<IEnumerable>`.

You can handle the Group Header tapped event
```csharp
void WCollectionView_HeaderTapped(object sender, Wapps.Forms.Controls.WItemTappedEventArgs args)
{

}
```
