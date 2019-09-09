# WCollectionView.Forms

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

##### Custom Height For Item Delegate
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
Setting `IsGroupingEnabled = true` you can define a `GroupHeaderTemplate` and set the `ItemsSource` a IEnumerable<IEnumerable>.

You can handle the Group Header tapped event
```csharp
void WCollectionView_HeaderTapped(object sender, Wapps.Forms.Controls.WItemTappedEventArgs args)
{

}
```
